using Demo.Models;
using Microsoft.AspNetCore.Mvc;

namespace Demo.Controllers;
public class ProfileController : Controller
{
    public IActionResult Index()
    {
        return View();
    }

    public IActionResult Profile()
    {
        return PartialView("_Profile");
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
