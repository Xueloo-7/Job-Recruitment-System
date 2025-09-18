public class JobReviewedEventHandler : IEventHandler<JobReviewedEvent>
{
    private readonly INotificationService _notificationService;

    public JobReviewedEventHandler(INotificationService notificationService)
    {
        _notificationService = notificationService;
    }

    public async Task HandleAsync(JobReviewedEvent e)
    {
        string title = e.Status == "Approved" ? "职位审核通过" : "职位审核未通过";
        string content = e.Status == "Approved"
            ? $"你发布的职位 {e.JobId} 已审核通过，可以正式上线。"
            : $"你发布的职位 {e.JobId} 审核未通过，原因：{e.Reason ?? "请联系管理员"}。";

        await _notificationService.CreateNotificationAsync(
            e.EmployerId, null,
            title, content,
            NotificationType.System,
            e.JobId
        );
    }
}
