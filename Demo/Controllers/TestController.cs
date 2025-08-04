using Demo.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

public class TestController : Controller
{
    private readonly DB _db;

    public TestController(DB context)
    {
        _db = context;
    }

    // 首页：查看所有表数据总览（数据条数）
    public IActionResult Index()
    {
        var model = new TestDashboardViewModel
        {
            UserCount = _db.Users.Count(),
            JobCount = _db.Jobs.Count(),
            ApplicationCount = _db.Applications.Count(),
            CategoryCount = _db.Categories.Count(),
            QualificationCount = _db.Qualifications.Count(),
            InstitutionCount = _db.Institutions.Count(),
            NotificationCount = _db.Notifications.Count()
            // 你可以继续添加其它表的数据量
        };

        return View(model);
    }

    // User =============================================================================================================== User
    #region User
    #region GET
    public IActionResult Users()
    {
        var users = _db.Users.ToList();
        return View("User/Index", users);
    }

    public IActionResult CreateUser()
    {
        return View("User/Create");
    }

    public IActionResult EditUser(string id)
    {
        var user = _db.Users.Find(id);
        if (user == null) return NotFound();
        return View("User/Edit", user);
    }

    public IActionResult DeleteUser(string id)
    {
        var user = _db.Users.Find(id);
        if (user == null) return NotFound();
        return View("User/Delete", user);
    }

    #endregion

    #region POST
    [HttpPost]
    public IActionResult CreateUser(UserVM vm)
    {
        // Model验证
        if (ModelState.IsValid)
        {
            var user = new User
            {
                Id = GenerateUserId(),
                Name = GenerateUsername(vm.Email),
                PasswordHash = vm.PasswordHash,
                Email = vm.Email,
                PhoneNumber = vm.PhoneNumber,
                Role = vm.Role
            };

            _db.Users.Add(user);
            _db.SaveChanges();
            Debug.WriteLine($"User created: {user.Id} - {user.Name}");
            return RedirectToAction("Users");
        }
        else
        {
            foreach (var entry in ModelState)
            {
                foreach (var error in entry.Value.Errors)
                {
                    Debug.WriteLine($"Error in {entry.Key}: {error.ErrorMessage}");
                }
            }
            return View("User/Create", vm);
        }
    }

    [HttpPost]
    public IActionResult EditUser(User user)
    {
        if (ModelState.IsValid)
        {
            _db.Users.Update(user);
            _db.SaveChanges();
            return RedirectToAction("Users");
        }
        return View("User/Edit", user);
    }

    [HttpPost, ActionName("DeleteUser")]
    public IActionResult DeleteUserConfirmed(string id)
    {
        var user = _db.Users.Find(id);
        if (user != null)
        {
            _db.Users.Remove(user);
            _db.SaveChanges();
        }
        return RedirectToAction("Users");
    }

    #endregion

    #region Functions
    private string GenerateUserId()
    {
        // 查询当前已有的最大编号
        var lastUser = _db.Users
            .Where(u => u.Id.StartsWith("U"))
            .OrderByDescending(u => u.Id)
            .FirstOrDefault();

        int nextNumber = 1;
        if (lastUser != null)
        {
            string lastNumberStr = lastUser.Id.Substring(3); // 去掉 "JOB"
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
        bool exists = _db.Users.Any(u => u.Id == id);
        if (exists)
            return Json($"ID {id} 已存在");
        return Json(true);
    }

    #endregion
    #endregion

    // Category =============================================================================================================== Category
    //#region Category
    //#region GET
    //public IActionResult Categories()
    //{
    //    var categories = _db.Categories.ToList();
    //    return View("Category/Index", categories);
    //}

    //public IActionResult CreateCategory()
    //{
    //    return View("Category/Create");
    //}

    //public IActionResult EditCategory(string id)
    //{
    //    var user = _db.Categories.Find(id);
    //    if (user == null) return NotFound();
    //    return View("Category/Edit", user);
    //}

    //public IActionResult DeleteCategory(string id)
    //{
    //    var user = _db.Categories.Find(id);
    //    if (user == null) return NotFound();
    //    return View("Category/Delete", user);
    //}

    //#endregion

    //#region POST
    //[HttpPost]
    //public IActionResult CreateCategory(UserVM vm)
    //{
    //    // Model验证
    //    if (ModelState.IsValid)
    //    {
    //        var user = new User
    //        {
    //            Id = GenerateUserId(),
    //            Name = GenerateUsername(vm.Email),
    //            PasswordHash = vm.PasswordHash,
    //            Email = vm.Email,
    //            PhoneNumber = vm.PhoneNumber,
    //            Role = vm.Role
    //        };

    //        _db.Users.Add(user);
    //        _db.SaveChanges();
    //        Debug.WriteLine($"User created: {user.Id} - {user.Name}");
    //        return RedirectToAction("Users");
    //    }
    //    else
    //    {
    //        foreach (var entry in ModelState)
    //        {
    //            foreach (var error in entry.Value.Errors)
    //            {
    //                Debug.WriteLine($"Error in {entry.Key}: {error.ErrorMessage}");
    //            }
    //        }
    //        return View("User/Create", vm);
    //    }
    //}

    //[HttpPost]
    //public IActionResult EditCategory(User user)
    //{
    //    if (ModelState.IsValid)
    //    {
    //        _db.Users.Update(user);
    //        _db.SaveChanges();
    //        return RedirectToAction("Users");
    //    }
    //    return View("User/Edit", user);
    //}

    //[HttpPost, ActionName("DeleteUser")]
    //public IActionResult DeleteCategoryConfirmed(string id)
    //{
    //    var user = _db.Users.Find(id);
    //    if (user != null)
    //    {
    //        _db.Users.Remove(user);
    //        _db.SaveChanges();
    //    }
    //    return RedirectToAction("Users");
    //}

    //#endregion

    //#region Functions
    //private string GenerateCategoryId()
    //{
    //    // 查询当前已有的最大编号
    //    var lastCategory = _db.Categories
    //        .Where(c => c.Id.StartsWith("U"))
    //        .OrderByDescending(c => c.Id)
    //        .FirstOrDefault();

    //    int nextNumber = 1;
    //    if (lastCategory != null)
    //    {
    //        string lastNumberStr = lastCategory.Id.Substring(3); // 去掉 "JOB"
    //        if (int.TryParse(lastNumberStr, out int lastNumber))
    //        {
    //            nextNumber = lastNumber + 1;
    //        }
    //    }

    //    return $"U{nextNumber.ToString("D3")}";  // 例如 JOB001、JOB002
    //}

    //private string GenerateUsername(string userEmail)
    //{
    //    if (string.IsNullOrEmpty(userEmail))
    //        return "";

    //    int atIndex = userEmail.IndexOf('@');
    //    if (atIndex > 0)
    //        return userEmail.Substring(0, atIndex);

    //    return userEmail; // 如果没有@就原样返回
    //}

    //public IActionResult CheckUserId(string id)
    //{
    //    bool exists = _db.Users.Any(u => u.Id == id);
    //    if (exists)
    //        return Json($"ID {id} 已存在");
    //    return Json(true);
    //}

    //#endregion
    //#endregion

}

public class TestDashboardViewModel
{
    public int UserCount { get; set; }
    public int JobCount { get; set; }
    public int ApplicationCount { get; set; }
    public int CategoryCount { get; set; }
    public int QualificationCount { get; set; }
    public int InstitutionCount { get; set; }
    public int NotificationCount { get; set; }
}