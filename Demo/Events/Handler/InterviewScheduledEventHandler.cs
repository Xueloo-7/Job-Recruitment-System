public class InterviewScheduledEventHandler : IEventHandler<InterviewScheduledEvent>
{
    private readonly INotificationService _notificationService;

    public InterviewScheduledEventHandler(INotificationService notificationService)
    {
        _notificationService = notificationService;
    }

    public async Task HandleAsync(InterviewScheduledEvent domainEvent)
    {
        string title = "面试邀请";
        string content = $"雇主 {domainEvent.EmployerId} 邀请你面试职位 {domainEvent.JobId}，时间：{domainEvent.InterviewTime:g}";

        await _notificationService.CreateNotificationAsync(
            domainEvent.JobseekerId,    // 通知接收者 = Jobseeker
            domainEvent.EmployerId,    // 触发者 = Employer
            title,
            content,
            NotificationType.Interview,
            domainEvent.JobId
        );
    }
}
