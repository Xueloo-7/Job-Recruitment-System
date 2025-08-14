using Demo.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace Demo.Controllers;
public class ProfileController : Controller
{
    private readonly DB db;

    User? currentUser = null;

    // 依赖注入
    public ProfileController(DB context)
    {
        db = context;
    }

    // 模拟数据
    private static User demoUser = new User
    {
        FirstName = "Xueloo",
        LastName = "Chan",
        Location = "Kuala Lumpur",
        PhoneNumber = "012-3456789"
    };
    public IActionResult Index(string? userId = "")
    {
        var user = db.Users.Find(userId);
        if(user != null)
        {
            currentUser = user;
        }
        else
        {
            // 如果没有传入 userId，则使用模拟数据
            currentUser = demoUser;
        }

        return View(user);
    }

    public IActionResult EditPartial()
    {
        return PartialView("_Edit", currentUser);
    }

    [HttpPost]
    public IActionResult Edit(User updatedUser)
    {
        // 更新逻辑
        currentUser = updatedUser;

        // 返回更新后的 Profile 视图
        return PartialView("_Profile", currentUser);
    }
     
    public IActionResult Profile()
    {
        Debug.WriteLine(currentUser);
        return PartialView("_Profile", currentUser);
    }

    public IActionResult Summary()
    {
        return PartialView("_Summary");
    }

    public IActionResult CareerHistory()
    {
        return PartialView("_CareerHistory");
    }

    public IActionResult Education()
    {
        return PartialView("_Education");
    }
}
