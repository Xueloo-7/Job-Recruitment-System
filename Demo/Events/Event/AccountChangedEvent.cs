public class AccountChangedEvent : IDomainEvent
{
    public string UserId { get; }
    public string ChangeType { get; } // Password / Email
    public DateTime OccurredAt { get; } = DateTime.UtcNow;

    public AccountChangedEvent(string userId, string changeType)
    {
        UserId = userId;
        ChangeType = changeType;
    }
}
