using Demo.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Security.Claims;


public class EmployerController : BaseController
{
    private readonly DB db;
    private readonly Helper hp;

    public EmployerController(DB context, Helper hp)
    {
        this.db = context;
        this.hp = hp;
    }

    #region Authentication
    // GET: Employer/CheckEmail
    public bool CheckEmail(string email)
    {
        return !db.Users.Any(u => u.Email == email);
    }

    public IActionResult AccessDenied()
    {
        ViewBag.Message = "You don’t have permission to view this page. Please switch to a Employer account.";
        return View();
    }

    public IActionResult Register()
    {
        return View();
    }

    [HttpPost]
    public IActionResult Register(RegisterVM vm)
    {
        if (ModelState.IsValid("Email") &&
            db.Users.Any(u => u.Email == vm.Email))
        {
            ModelState.AddModelError("Email", "Duplicated Email.");
        }

        if (ModelState.IsValid("Photo") && vm.Photo != null)
        {
            var err = hp.ValidatePhoto(vm.Photo);
            if (err != "") ModelState.AddModelError("Photo", err);
        }

        if (ModelState.IsValid)
        {
            // Insert member
            var newUser = new User
            {
                Id = Helper.GenerateId(db.Users, "U"),
                Name = GenerateUsername(vm.Email),
                PasswordHash = hp.HashPassword(vm.Password),
                Email = vm.Email,
                PhoneNumber = "",
                Role = Role.Employer,
            };

            db.Users.Add(newUser);
            db.SaveChanges();

            // audit log
            var log = new AuditLog
            {
                UserId = newUser.Id,
                TableName = "Users",
                ActionType = "Create",
                RecordId = newUser.Id,
                Changes = $"Employer registered: {newUser.Email}"
            };
            db.AuditLogs.Add(log);
            db.SaveChanges();

            SetFlashMessage(FlashMessageType.Info, "Register successfully. Please login.");
            return RedirectToAction("Login");
        }
        else
        {
            DebugModelStateErrors();
        }

        return View(vm);
    }

    public IActionResult Login(string? returnURL, string? email)
    {
        if (User.Identity != null && User.Identity.IsAuthenticated)
        {
            return RedirectToAction("Index", "Employer");
        }
        var vm = new LoginVM
        {
            // can be null
            Email = email!
        };
        ViewBag.ReturnURL = returnURL;
        return View(vm);
    }

    [HttpPost]
    public IActionResult Login(LoginVM vm, string? returnURL)
    {
        var user = db.Users.Where(u => u.Email == vm.Email).FirstOrDefault();

        if (user == null)
        {
            ModelState.AddModelError("Email", "This email is not registered.");
            return View(vm);
        }
        if (user.Role != Role.Employer)
        {
            ModelState.AddModelError("Email", "This email is already registered as " + user.Role.ToString());
            return View(vm);
        }
        if (user == null || !hp.VerifyPassword(user.PasswordHash, vm.Password))
        {
            ModelState.AddModelError("Password", "Invalid credentials");
        }
        else if (ModelState.IsValid)
        {
            SetFlashMessage(FlashMessageType.Info, "Login Successfully.");

            hp.SignIn(user, vm.RememberMe);

            // audit log
            var log = new AuditLog
            {
                UserId = user.Id,
                TableName = "Users",
                ActionType = "Login",
                RecordId = user.Id,
                Changes = $"Employer login: {user.Email}"
            };
            db.AuditLogs.Add(log);
            db.SaveChanges();

            if (string.IsNullOrEmpty(returnURL))
            {
                return RedirectToAction("Index", "Employer");
            }
        }

        DebugModelStateErrors();

        return View(vm);
    }

    public IActionResult Logout(string? returnURL)
    {
        SetFlashMessage(FlashMessageType.Info, "Logout Successfully");

        // audit log
        var userId = User.GetUserId();
        var log = new AuditLog
        {
            UserId = userId,
            TableName = "Users",
            ActionType = "Logout",
            RecordId = userId,
            Changes = $"Employer logout: {userId}"
        };
        db.AuditLogs.Add(log);
        db.SaveChanges();

        hp.SignOut();

        return RedirectToAction("Index", "Home");
    }

    private void DebugModelStateErrors()
    {
        foreach (var entry in ModelState)
        {
            foreach (var error in entry.Value.Errors)
            {
                Debug.WriteLine($"=============Error in {entry.Key}: {error.ErrorMessage}");
            }
        }
    }


    private string GenerateUsername(string userEmail)
    {
        if (string.IsNullOrEmpty(userEmail))
            return "";

        int atIndex = userEmail.IndexOf('@');
        if (atIndex > 0)
            return userEmail.Substring(0, atIndex);

        return userEmail; // 如果没有@就原样返回
    }

    public IActionResult CheckUserId(string id)
    {
        bool exists = db.Users.Any(u => u.Id == id);
        if (exists)
            return Json($"ID {id} 已存在");
        return Json(true);
    }
    #endregion

    [Authorize(Roles = "Employer")]
    public IActionResult Index()
    {
        string userId = User.GetUserId();

        // Get User Info
        var user = db.Users
            .Where(u => u.Id == userId)
            .Select(u => new
            {
                u.Name,
                u.FirstName,
                u.LastName,
                u.Location
            })
            .FirstOrDefault();

        // Combine FirstName & LastName
        string userName = "Unknown";
        if (user != null)
        {
            userName = string.IsNullOrWhiteSpace(user.FirstName) && string.IsNullOrWhiteSpace(user.LastName)
                ? user.Name
                : $"{user.FirstName} {user.LastName}".Trim();
        }

        // Get Jobs & Drafts
        var jobs = db.Jobs
            .Where(j => j.UserId == userId)
            .Select(j => new
            {
                j.Id,
                j.Title,
                j.Status,
                j.User.Location
            })
            .ToList();

        var drafts = db.JobDrafts
            .Where(d => d.UserId == userId)
            .Select(d => new EmployerDraftVM
            {
                Id = d.Id.ToString(),
                JobId = d.JobId,
                Title = d.Title ?? "Unknown"
            })
            .ToList();

        // Get Application Stats
        var appStats = db.Applications
            .Where(a => a.Job.UserId == userId)
            .GroupBy(a => a.JobId)
            .Select(g => new
            {
                JobId = g.Key,
                Total = g.Count(),
                Hired = g.Count(a => a.Status == ApplicationStatus.Hired)
            })
            .ToList();

        int totalApplications = appStats.Sum(s => s.Total);
        int totalHires = appStats.Sum(s => s.Hired);

        // Build Job ViewModels (Avoid N+1 Query)
        var jobVMs = jobs
            .Select(j => new EmployerJobVM
            {
                Id = j.Id,
                Title = j.Title,
                Location = j.Location,
                Status = j.Status,
                CandidatesCount = appStats.FirstOrDefault(s => s.JobId == j.Id)?.Total ?? 0,
                HiredCount = appStats.FirstOrDefault(s => s.JobId == j.Id)?.Hired ?? 0
            })
            .ToList();

        // Build Final ViewModel
        var vm = new EmployerDashboardVM
        {
            EmployerName = userName,
            TotalJobs = jobs.Count,
            TotalApplications = totalApplications,
            TotalHires = totalHires,
            Jobs = jobVMs,
            Drafts = drafts
        };

        return View(vm);
    }

}
