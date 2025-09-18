public class JobExpiredEvent : IDomainEvent
{
    public string JobId { get; }
    public string EmployerId { get; }
    public DateTime OccurredAt { get; } = DateTime.UtcNow;

    public JobExpiredEvent(string jobId, string employerId)
    {
        JobId = jobId;
        EmployerId = employerId;
    }
}
