public interface IEventDispatcher
{
    Task DispatchAsync(IDomainEvent domainEvent);
}

public class EventDispatcher : IEventDispatcher
{
    private readonly IServiceProvider _provider;

    public EventDispatcher(IServiceProvider provider)
    {
        _provider = provider;
    }

    public async Task DispatchAsync(IDomainEvent domainEvent)
    {
        var handlerType = typeof(IEventHandler<>).MakeGenericType(domainEvent.GetType());
        var handlers = _provider.GetServices(handlerType);

        foreach (var handler in handlers ?? Enumerable.Empty<object>())
        {
            if (handler == null) continue; // 防御性检查
            await ((dynamic)handler).HandleAsync((dynamic)domainEvent);
        }
    }
}
