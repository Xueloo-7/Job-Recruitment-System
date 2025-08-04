using Demo.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Demo.Controllers;

public class HomeController : Controller
{
    private readonly DB db;

    public HomeController(DB context)
    {
        db = context;
    }

    public IActionResult Index(string keyword = "")
    {
        ViewBag.Active = "Search";

        var jobs = db.Jobs
            .Include(j => j.Category)
            .Where(j => j.IsOpen && (string.IsNullOrEmpty(keyword) || j.Title.Contains(keyword)))
            .ToList();

        return View(jobs);
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

    //public IActionResult JobDetails(int id)
    //{
    //    var job = db.Jobs
    //        .Include(j => j.Category)
    //        .FirstOrDefault(j => j.Id == id);

    //    if (job == null) return NotFound();

    //    return PartialView("_JobDetails", job);
    //}
}
