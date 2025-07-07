using Microsoft.AspNetCore.Mvc;

namespace Demo.Controllers;

public class HomeController : Controller
{
    public IActionResult Index()
    {
        return View();
    }

    public IActionResult Demo1(string name, int age)
    {
        ViewBag.Name = name;
        ViewBag.Age = age;
        return View();
    }

    public IActionResult Demo2(string name, int age)
    {
        var m = new
        {
            Name = name,
            Age = age,
        };
        return View(m);
    }

    [Route("ABC")]
    public IActionResult Demo3()
    {
        return View("123");
    }
}
