using Demo.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Diagnostics;

public class NotificationController : BaseController
{
    private readonly DB _db;

    public NotificationController(DB context)
    {
        _db = context;
    }

    public IActionResult Create()
    {
        var users = _db.Users
                      .Select(u => new { u.Id, u.Name }) // 假设你的User类有 Id 和 UserName
                      .ToList();

        ViewBag.Users = new SelectList(users, "Id", "UserName");

        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Create(Notification notification)
    {
        if (ModelState.IsValid)
        {
            notification.CreatedAt = DateTime.Now; // 如果有时间字段
            _db.Notifications.Add(notification);
            _db.SaveChanges();
            return RedirectToAction(nameof(Index));
        }

        ViewBag.Users = new SelectList(_db.Users, "Id", "UserName", notification.UserId);
        return View(notification);
    }

    public IActionResult Index(string? userId = "")
    {
        if (userId == null || userId == "")
            userId = User.GetUserId(); // 获取当前用户 ID

        var notifications = _db.Notifications
            .Where(n => n.UserId == userId)
            .OrderByDescending(n => n.CreatedAt)
            .ToList();

        ViewBag.UnreadCount = notifications.Count(n => !n.IsRead);

        return View(notifications);
    }

    [HttpPost]
    public IActionResult MarkAsRead(string id)
    {
        var notification = _db.Notifications.Find(id);
        if (notification != null && notification.UserId == User.GetUserId())
        {
            notification.IsRead = true;
            _db.SaveChanges();
        }
        return RedirectToAction("Index");
    }


    #region Functions
    private string GenerateNotificationId()
    {
        var last = _db.Notifications
            .Where(n => n.Id.StartsWith("N"))
            .OrderByDescending(n => n.Id)
            .FirstOrDefault();

        int next = 1;
        if (last != null)
        {
            string numberStr = last.Id.Substring(1);
            if (int.TryParse(numberStr, out int lastNumber))
            {
                next = lastNumber + 1;
            }
        }

        return $"N{next.ToString("D3")}";
    }

    public IActionResult CheckNotificationId(string id)
    {
        bool exists = _db.Notifications.Any(n => n.Id == id);
        if (exists)
            return Json($"ID {id} Already exists");
        return Json(true);
    }

    #endregion
}