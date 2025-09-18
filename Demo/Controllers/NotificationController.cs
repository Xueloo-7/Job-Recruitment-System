using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Diagnostics;

public class NotificationController : BaseController
{
    private readonly DB _db;
    private readonly INotificationService _notificationService;

    public NotificationController(DB context, INotificationService notificationService)
    {
        _db = context;
        _notificationService = notificationService;
    }

    [Authorize]
    [HttpGet("Notification/UnreadCount")]
    public async Task<IActionResult> GetUnreadCount()
    {
        string userId = User.GetUserId(); // 自己封装的扩展方法
        int count = await _notificationService.GetUnreadCountAsync(userId);
        return Ok(new { count });
    }

    [Authorize]
    [HttpGet("Notification")]
    public async Task<IActionResult> GetNotifications()
    {
        string userId = User.GetUserId();
        var notifications = await _notificationService.GetUserNotificationsAsync(userId);
        return View("NotificationList", notifications);
        }

    [Authorize]
    [HttpPost("Notification/MarkAsRead/{id}")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> MarkAsRead(string id)
    {
        await _notificationService.MarkAsReadAsync(id);
        return RedirectToAction("GetNotifications");
    }

    #endregion
}