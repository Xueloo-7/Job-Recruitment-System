using Demo.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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

