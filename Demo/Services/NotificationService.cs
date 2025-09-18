using Microsoft.EntityFrameworkCore;

public interface INotificationService
{
    Task CreateNotificationAsync(string userId, string? fromUserId, string title, string content, NotificationType type, string? relatedEntityId = null);
    Task<int> GetUnreadCountAsync(string userId);
    Task<List<Notification>> GetUserNotificationsAsync(string userId);
    Task MarkAsReadAsync(string notificationId);
}

public class NotificationService : INotificationService
{
    private readonly DB _db;

    public NotificationService(DB db)
    {
        _db = db;
    }

    public async Task CreateNotificationAsync(string userId, string? fromUserId, string title, string content, NotificationType type, string? relatedEntityId = null)
    {
        var id = $"N{(_db.Notifications.Count() + 1):D3}"; // 简单生成ID
        var notification = new Notification
        {
            Id = id,
            UserId = userId,
            FromUserId = fromUserId,
            Title = title,
            Content = content,
            Type = type,
            RelatedEntityId = relatedEntityId
        };

        _db.Notifications.Add(notification);
        await _db.SaveChangesAsync();
    }

    public async Task<int> GetUnreadCountAsync(string userId)
    {
        return await _db.Notifications
            .CountAsync(n => n.UserId == userId && !n.IsRead);
    }

    public async Task<List<Notification>> GetUserNotificationsAsync(string userId)
    {
        return await _db.Notifications
            .Where(n => n.UserId == userId)
            .OrderByDescending(n => n.CreatedAt)
            .ToListAsync();
    }

    public async Task MarkAsReadAsync(string notificationId)
    {
        var notification = await _db.Notifications.FindAsync(notificationId);
        if (notification != null)
        {
            notification.IsRead = true;
            await _db.SaveChangesAsync();
        }
    }
}
