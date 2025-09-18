public class JobExpiredEventHandler : IEventHandler<JobExpiredEvent>
{
    private readonly INotificationService _notificationService;

    public JobExpiredEventHandler(INotificationService notificationService)
    {
        _notificationService = notificationService;
    }

    public async Task HandleAsync(JobExpiredEvent e)
    {
        string title = "职位已过期";
        string content = $"您发布的职位 {e.JobId} 已过期，请考虑重新发布。";

        await _notificationService.CreateNotificationAsync(
            e.EmployerId, null, title, content,
            NotificationType.System, e.JobId
        );
    }
}
