public class ApplicationStatusChangedEvent : IDomainEvent
{
    public string ApplicationId { get; }
    public string JobId { get; }
    public string EmployerId { get; }
    public string JobseekerId { get; }
    public string Status { get; } // Offer / Rejected
    public DateTime OccurredAt { get; } = DateTime.UtcNow;

    public ApplicationStatusChangedEvent(string applicationId, string jobId, string employerId, string jobseekerId, string status)
    {
        ApplicationId = applicationId;
        JobId = jobId;
        EmployerId = employerId;
        JobseekerId = jobseekerId;
        Status = status;
    }
}
