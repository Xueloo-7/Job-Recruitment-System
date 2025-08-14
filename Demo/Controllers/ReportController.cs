using Microsoft.AspNetCore.Mvc;

public class ReportController : Controller
{
    private readonly DB _db;

    public ReportController(DB context)
    {
        _db = context;
    }

    public IActionResult Index()
    {
        return View();
    }
}
