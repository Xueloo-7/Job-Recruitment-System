using Demo.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace Demo.Controllers;
public class ProfileController : Controller
{
    private readonly DB db;

    public ProfileController(DB context)
    {
        db = context;
    }

    private static User demoUser = new User
    {
        FirstName = "Xueloo",
        LastName = "Chan",
        Location = "Kuala Lumpur",
        PhoneNumber = "012-3456789"
    };

    private User GetCurrentUser(string? userId)
    {
        Debug.WriteLine($"GetCurrentUser called with userId: {userId}");
        if (string.IsNullOrEmpty(userId))
            return demoUser;

        var user = db.Users.Find(userId);
        return user ?? demoUser;
    }

    public IActionResult Index(string? userId = "")
    {
        User user = GetCurrentUser(userId);

        return View(user);
    }

    public IActionResult EditPartial(string? userId = "")
    {
        return PartialView("_Edit", GetCurrentUser(userId));
    }

    [HttpPost]
    public IActionResult Edit(User updatedUser)
    {
        // 更新数据库
        db.Users.Update(updatedUser);
        db.SaveChanges();

        // 返回更新后的 Profile
        return PartialView("_Profile", updatedUser);
    }

    public IActionResult Profile(string? userId)
    {
        return PartialView("_Profile", GetCurrentUser(userId));
    }

    public IActionResult Summary(string? userId)
    {
        return PartialView("_Summary", GetCurrentUser(userId));
    }

    public IActionResult CareerHistory(string? userId)
    {
        return PartialView("_CareerHistory", GetCurrentUser(userId));
    }

    public IActionResult Education(string? userId)
    {
        return PartialView("_Education", GetCurrentUser(userId));
    }
}
