public class JobAppliedEvent : IDomainEvent
{
    public string JobId { get; }
    public string JobseekerId { get; }
    public string EmployerId { get; }

    public JobAppliedEvent(string jobId, string jobseekerId, string employerId)
    {
        JobId = jobId;
        JobseekerId = jobseekerId;
        EmployerId = employerId;
    }
}