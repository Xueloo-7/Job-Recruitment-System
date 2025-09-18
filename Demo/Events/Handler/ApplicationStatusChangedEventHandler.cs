public class ApplicationStatusChangedEventHandler : IEventHandler<ApplicationStatusChangedEvent>
{
    private readonly INotificationService _notificationService;

    public ApplicationStatusChangedEventHandler(INotificationService notificationService)
    {
        _notificationService = notificationService;
    }

    public async Task HandleAsync(ApplicationStatusChangedEvent e)
    {
        string title = e.Status == "Offer" ? "录用通知" : "申请结果";
        string content = e.Status == "Offer"
            ? $"恭喜！雇主 {e.EmployerId} 录用了你申请的职位 {e.JobId}。"
            : $"很遗憾，你申请的职位 {e.JobId} 未通过。";

        await _notificationService.CreateNotificationAsync(
            e.JobseekerId, e.EmployerId,
            title, content,
            NotificationType.Application,
            e.ApplicationId
        );
    }
}
