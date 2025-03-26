namespace Greenhouse.MessageBus.Abstractions
{
    public interface IMonitorMessageHandler
    {
        Task Handle(IDictionary<string, object?> metadata, object payload, CancellationToken cancellationToken = default);
    }
}
