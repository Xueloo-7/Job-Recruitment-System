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

        if (user == null || !hp.VerifyPassword(user.PasswordHash, vm.Password))
        {
            ModelState.AddModelError("", "Invalid credentials");
        }

        if (ModelState.IsValid)
        {
            TempData["Info"] = "Login Successfully.";

            hp.SignIn(user!.Email, user.Role.ToString(), vm.RememberMe);

            if (string.IsNullOrEmpty(returnURL))
            {
                return RedirectToAction("Index", "Home");
            }
        }

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
                Debug.WriteLine($"Error in {entry.Key}: {error.ErrorMessage}");
            }
        }
    }

    //// GET: Account/UpdatePassword
    //// TODO
    //public IActionResult UpdatePassword()
    //{
    //    return View();
    //}

    //// POST: Account/UpdatePassword
    //// TODO
    //[HttpPost]
    //public IActionResult UpdatePassword(UpdatePasswordVM vm)
    //{
    //    // Get user (admin or member) record based on email (PK)
    //    // TODO
    //    var u = db.Users.Find("TODO");
    //    if (u == null) return RedirectToAction("Index", "Home");

    //    // If current password not matched
    //    // TODO
    //    if (true)
    //    {
    //        ModelState.AddModelError("Current", "Current Password not matched.");
    //    }

    //    if (ModelState.IsValid)
    //    {
    //        // Update user password (hash)
    //        // TODO

    //        TempData["Info"] = "Password updated.";
    //        return RedirectToAction();
    //    }

    //    return View();
    //}

    //// GET: Account/UpdateProfile
    //// TODO
    //public IActionResult UpdateProfile()
    //{
    //    // Get member record based on email (PK)
    //    // TODO
    //    var m = db.Members.Find("TODO");
    //    if (m == null) return RedirectToAction("Index", "Home");

    //    var vm = new UpdateProfileVM
    //    {
    //        // TODO
    //        Email = "TODO",
    //        Name = "TODO",
    //        PhotoURL = m.PhotoURL,
    //    };

    //    return View(vm);
    //}

    //// POST: Account/UpdateProfile
    //// TODO
    //[HttpPost]
    //public IActionResult UpdateProfile(UpdateProfileVM vm)
    //{
    //    // Get member record based on email (PK)
    //    // TODO
    //    var m = db.Members.Find("TODO");
    //    if (m == null) return RedirectToAction("Index", "Home");

    //    if (vm.Photo != null)
    //    {
    //        var err = hp.ValidatePhoto(vm.Photo);
    //        if (err != "") ModelState.AddModelError("Photo", err);
    //    }

    //    if (ModelState.IsValid)
    //    {
    //        // TODO
    //        var TODO = vm.Name;

    //        if (vm.Photo != null)
    //        {
    //            hp.DeletePhoto(m.PhotoURL, "photos");
    //            m.PhotoURL = hp.SavePhoto(vm.Photo, "photos");
    //        }

    //        db.SaveChanges();

    //        TempData["Info"] = "Profile updated.";
    //        return RedirectToAction();
    //    }

    //    // TODO
    //    vm.Email = "TODO";
    //    vm.PhotoURL = m.PhotoURL;
    //    return View(vm);
    //}

    //// GET: Account/ResetPassword
    //public IActionResult ResetPassword()
    //{
    //    return View();
    //}

    //// POST: Account/ResetPassword
    //[HttpPost]
    //public IActionResult ResetPassword(ResetPasswordVM vm)
    //{
    //    var u = db.Users.Find(vm.Email);

    //    if (u == null)
    //    {
    //        ModelState.AddModelError("Email", "Email not found.");
    //    }

    //    if (ModelState.IsValid)
    //    {
    //        // Generate random password
    //        // TODO
    //        string password = "TODO";

    //        // Update user (admin or member) record
    //        // TODO

    //        // TODO: Send reset password email

    //        TempData["Info"] = $"Password reset to <b>{password}</b>.";
    //        return RedirectToAction();
    //    }

    //    return View();
    //}







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
