using Demo.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

//---Put in first because no database in Models---


//public IActionResult SearchAjax(string? keyword, string? location)
//{
//    keyword = keyword?.Trim().ToLower() ?? "";
//    location = location?.Trim().ToLower() ?? "";

//    var result = db.Jobs
//        .Where(j =>
//            (string.IsNullOrEmpty(keyword) || j.Title.ToLower().Contains(keyword)) &&
//            (string.IsNullOrEmpty(location) || j.Location.ToLower().Contains(location)))
//        .ToList();

//    return PartialView("_JobListPartial", result);
//}

public class JobController : Controller
{
    private readonly DB _context;

    public JobController(DB context)
    {
        _context = context;
    }



    public IActionResult Index()
    {
        return View();
    }

    public async Task<IActionResult> Subscription()
    {
        var promotions = await _context.Promotions.ToListAsync(); // 从数据库取 Promotion
        return View(promotions); // ✅ 正确，传入 Model
    }
}

