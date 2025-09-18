public class JobReviewedEvent : IDomainEvent
{
    public string JobId { get; }
    public string EmployerId { get; }
    public string Status { get; } // Approved / Rejected
    public string? Reason { get; }
    public DateTime OccurredAt { get; } = DateTime.UtcNow;

    public JobReviewedEvent(string jobId, string employerId, string status, string? reason = null)
    {
        JobId = jobId;
        EmployerId = employerId;
        Status = status;
        Reason = reason;
    }
}
