using Microsoft.AspNetCore.Mvc;

namespace Demo.Controllers;

public class HomeController : Controller
{
    public IActionResult Index()
    {
        ViewBag.Active = "Search";
        return View();
    }

    public IActionResult Profile()
    {
        ViewBag.Active = "Profile";
        return View();
    }

    public IActionResult Employer()
    {
        ViewBag.Active = "Employer";
        return View();
    }

    public IActionResult SignIn()
    {
        ViewBag.Active = "SignIn";
        return View();
    }

    public IActionResult EsignIn()
    {
        ViewBag.Active = "EsignIn";
        return View();
    }
}
