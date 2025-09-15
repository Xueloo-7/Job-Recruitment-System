using Demo.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Security.Claims;

public class EmployerController : Controller
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

        if (ModelState.IsValid("Photo"))
        {
            var err = hp.ValidatePhoto(vm.Photo);
            if (err != "") ModelState.AddModelError("Photo", err);
        }

        if (ModelState.IsValid)
        {
            // Insert member
            // TODO
            db.Users.Add(new User
            {
                Id = Helper.GenerateId(db.Users, "U"),
                Name = GenerateUsername(vm.Email),
                PasswordHash = hp.HashPassword(vm.Password),
                Email = vm.Email,
                PhoneNumber = "",
                Role = Role.Employer,
            });

            db.SaveChanges();

            TempData["Info"] = "Register successfully. Please login.";
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

        if (user == null || !hp.VerifyPassword(user.PasswordHash, vm.Password))
        {
            ModelState.AddModelError("Password", "Invalid credentials");
        }
        else if (ModelState.IsValid)
        {
            TempData["Info"] = "Login Successfully.";

            hp.SignIn(user, vm.RememberMe);

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
        TempData["Info"] = "Logout Successfully.";

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

    public IActionResult Index()
    {
        string? userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        var user = db.Users
                     .Where(u => u.Id == userId && u.Role == Role.Employer)
                     .FirstOrDefault();

        if (user == null)
            return NotFound("Employer not found.");
        if (user.Role != Role.Employer)
            return BadRequest("User is not an employer.");

        var jobs = db.Jobs
                     .Where(j => j.UserId == userId)
                     .ToList();

        int totalApplications = db.Applications
                                  .Where(a => a.Job.UserId == userId)
                                  .Count();

        int totalHires = db.Applications
                           .Where(a => a.Job.UserId == userId && a.Status == ApplicationStatus.Hired)
                           .Count();



        var vm = new EmployerDashboardVM
        {
            EmployerName = string.IsNullOrWhiteSpace(user.FirstName) && string.IsNullOrWhiteSpace(user.LastName)
                            ? user.Name
                            : $"{user.FirstName} {user.LastName}".Trim(),
            TotalJobs = jobs.Count,
            TotalApplications = totalApplications,
            TotalHires = totalHires,
            Jobs = jobs.Select(j => new JobDashboardVM
            {
                JobId = j.Id,
                Title = j.Title,
                Location = j.Location,
                Status = j.Status.ToString(),
                Candidates = db.Applications.Where(a => a.JobId == j.Id).Count(),
                Hired = db.Applications.Where(a => a.JobId == j.Id && a.Status == ApplicationStatus.Hired).Count()
            }).ToList()
        };

        return View(vm);
    }

}
