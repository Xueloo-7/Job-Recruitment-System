using Demo;
using Demo.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
public class JobController : Controller
{
    private readonly DB db;

    public JobController(DB context)
    {
        db = context;
    }

    public IActionResult Index()
    {
        return View();
    }

    public IActionResult Subscription()
    {
        var promotions = db.Promotions.ToList();

        if (Request.isAjax())
        {
            return PartialView("_PromotionCard", promotions);
        }

        return View(promotions);
    }

    public IActionResult Details(string id)
    {
        var job = db.Jobs
            .Include(j => j.Category)
            .Include(j => j.User)
            .FirstOrDefault(j => j.Id == id);

        if (job == null) return NotFound();

        return PartialView("_JobDetails", job);
    }
}

