namespace Greenhouse.MessageBus.Abstractions
{
    public interface IMonitorMessageHandler
    {
        Task Handle(IDictionary<string, object?> metadata, ReadOnlyMemory<byte> payload, CancellationToken cancellationToken = default);
    }
}
