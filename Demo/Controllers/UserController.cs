using Microsoft.AspNetCore.Mvc;
using Demo.Models;

public class UserController : Controller
{

    // 模拟数据库读取
    private UserProfile GetUserProfile()
    {
        var profile = new UserProfile
        {
            Name = "Xueloo",
            Summary = "Software Developer",
            SelfIntroduction = "I enjoy building web applications.",
            CareerHistory = "Intern at XYZ",
            Education = "Bachelor of Computer Science"
        };

        return profile;
    }
    public IActionResult Profile()
    {
        var model = GetUserProfile();
        return PartialView("_Profile", model);
    }

    public IActionResult Summary()
    {
        var model = GetUserProfile();
        return PartialView("_Summary", model);
    }

    public IActionResult CareerHistory()
    {
        var model = GetUserProfile();
        return PartialView("_CareerHistory", model);
    }

    public IActionResult Education()
    {
        var model = GetUserProfile();
        return PartialView("_Education", model);
    }
}