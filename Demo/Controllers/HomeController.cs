using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Demo.Controllers;

public class HomeController : Controller
{
    public IActionResult Index()
    {
        ViewBag.Active = "Search";
        return View();
    }

    public IActionResult LoadJobs(string company)
    {
        // put database future
        var jobs = new List<string>();

        switch (company?.ToLower())
        {
            case "intel":
                jobs = new List<string> { "Software Engineer", "QA Tester", "Intern Developer" };
                break;
            case "lazada":
                jobs = new List<string> { "Frontend Developer", "UI/UX Designer", "System Analyst" };
                break;
            case "google":
                jobs = new List<string> { "AI Researcher", "Cloud Architect", "Technical Writer" };
                break;
        }

        ViewBag.Company = company;
        return PartialView("~/Views/Job/_JobListPartial.cshtml", jobs);
    }
}
