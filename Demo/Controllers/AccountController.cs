using Demo.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

public class AccountController : Controller
{
    private readonly DB db;
    private readonly Helper hp;

    public AccountController(DB context, Helper hp)
    {
        this.db = context;
        this.hp = hp;
    }

    // GET: Account/CheckEmail
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
                Id = GenerateUserId(),
                Name = GenerateUsername(vm.Email),
                PasswordHash = hp.HashPassword(vm.Password),
                Email = vm.Email,
                PhoneNumber = "",
                Role = Role.JobSeeker,
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

    public IActionResult Login(string? returnURL)
    {
        if (User.Identity != null && User.Identity.IsAuthenticated)
        {
            return RedirectToAction("Index", "Home");
        }
        ViewBag.ReturnURL = returnURL;
        return View();
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
        if (!hp.VerifyPassword(user.PasswordHash, vm.Password))
        {
            ModelState.AddModelError("Password", "Invalid password.");
            return View(vm);
        }
        else if (ModelState.IsValid)
        {
            TempData["Info"] = "Login Successfully.";

            //hp.SignIn(user!.Email, user.Role.ToString(), vm.RememberMe);
            hp.SignIn(user, vm.RememberMe);

            if (!string.IsNullOrEmpty(returnURL))
                return Redirect(returnURL);

            return user.Role == Role.Employer
                ? RedirectToAction("Index", "Employer")
                : RedirectToAction("Index", "Profile");

        }
        return View(vm);
    }

    public IActionResult Logout(string? returnURL)
    {
        TempData["Info"] = "Logout Successfully.";

        hp.SignOut();

        return RedirectToAction("Index", "Home");
    }

    public IActionResult AccessDenied()
    {
        ViewBag.Message = "You don’t have permission to view this page. Please switch to a Jobseeker account.";
        return View();
    }

    private void DebugModelStateErrors()
    {
        foreach (var entry in ModelState)
        {
            foreach (var error in entry.Value.Errors)
            {
                Debug.WriteLine($"Error in {entry.Key}: {error.ErrorMessage}");
            }
        }
    }



    private string GenerateUserId()
    {
        // 查询当前已有的最大编号
        var lastUser = db.Users
            .Where(u => u.Id.StartsWith("U"))
            .OrderByDescending(u => u.Id)
            .FirstOrDefault();

        int nextNumber = 1;
        if (lastUser != null)
        {
            string lastNumberStr = lastUser.Id.Substring(1);
            if (int.TryParse(lastNumberStr, out int lastNumber))
            {
                nextNumber = lastNumber + 1;
            }
        }

        return $"U{nextNumber.ToString("D3")}";  // 例如 JOB001、JOB002
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
}
