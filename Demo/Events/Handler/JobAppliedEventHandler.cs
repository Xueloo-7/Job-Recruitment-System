public class JobAppliedEventHandler : IEventHandler<JobAppliedEvent>
{
    private readonly INotificationService _notificationService;

    public JobAppliedEventHandler(INotificationService notificationService)
    {
        _notificationService = notificationService;
    }

    public bool CanHandle(IDomainEvent domainEvent) => domainEvent is JobAppliedEvent;

    public async Task HandleAsync(JobAppliedEvent domainEvent)
    {
        var e = domainEvent as JobAppliedEvent;
        string title = "新的职位申请";
        string content = $"用户 {e.JobseekerId} 申请了职位 {e.JobId}";

        await _notificationService.CreateNotificationAsync(
            e.EmployerId,
            e.JobseekerId,
            title,
            content,
            NotificationType.Application,
            e.JobId
        );
    }
}
