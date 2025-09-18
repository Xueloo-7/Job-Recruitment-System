public class InterviewScheduledEvent : IDomainEvent
{
    public string JobId { get; }
    public string EmployerId { get; }
    public string JobseekerId { get; }
    public DateTime InterviewTime { get; }
    public DateTime OccurredAt { get; } = DateTime.UtcNow;

    public InterviewScheduledEvent(string jobId, string employerId, string jobseekerId, DateTime interviewTime)
    {
        JobId = jobId;
        EmployerId = employerId;
        JobseekerId = jobseekerId;
        InterviewTime = interviewTime;
    }
}
