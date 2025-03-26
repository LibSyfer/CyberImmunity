namespace Greenhouse.MessageBus.Abstractions
{
    public interface IMonitorMessageBus
    {
        Task ResendAsync(string destination, ReadOnlyMemory<byte> payload, CancellationToken cancellationToken = default);
    }
}
