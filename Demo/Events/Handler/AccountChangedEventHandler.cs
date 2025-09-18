public class AccountChangedEventHandler : IEventHandler<AccountChangedEvent>
{
    private readonly INotificationService _notificationService;

    public AccountChangedEventHandler(INotificationService notificationService)
    {
        _notificationService = notificationService;
    }

    public async Task HandleAsync(AccountChangedEvent e)
    {
        string title, content;

        if (e.ChangeType == "Password")
        {
            title = "密码已修改";
            content = "你的账号密码已成功修改。如果不是本人操作，请尽快联系管理员。";
        }
        else if (e.ChangeType == "Email")
        {
            title = "邮箱已修改";
            content = "你的账号邮箱已成功修改。如果不是本人操作，请尽快联系管理员。";
        }
        else
        {
            title = "账号变更";
            content = "你的账号资料已修改。";
        }

        await _notificationService.CreateNotificationAsync(
            e.UserId, null,
            title, content,
            NotificationType.Account,
            null
        );
    }
}
