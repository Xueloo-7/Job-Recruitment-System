using Microsoft.AspNetCore.Mvc;
using Demo.Models;
using System.Diagnostics;

public class NotificationController : BaseController
{
    private readonly DB _db;

    public NotificationController(DB context)
    {
        _db = context;
    }

    public IActionResult CreateNotification(string toUserId, string title, string content, string fromUserId = "")
    {
        var notification = new Notification
        {
            Id = GenerateNotificationId(), // 你已有 ID 生成函数
            UserId = toUserId,
            FromUserId = fromUserId,
            Title = title,
            Content = content,
            CreatedAt = DateTime.UtcNow,
            IsRead = false
        };

        _db.Notifications.Add(notification);
        _db.SaveChanges();

        return View();
    }

    public IActionResult Index()
    {
        var userId = GetCurrentUserId();

        var notifications = _db.Notifications
            .Where(n => n.UserId == userId)
            .OrderByDescending(n => n.CreatedAt)
            .ToList();

        return View(notifications);
    }

    [HttpPost]
    public IActionResult MarkAsRead(string id)
    {
        var notification = _db.Notifications.Find(id);
        if (notification != null && notification.UserId == GetCurrentUserId())
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
            return Json($"ID {id} 已存在");
        return Json(true);
    }

    #endregion
}