namespace Greenhouse.MessageBus.Abstractions
{
    public interface IMonitorMessageBus
    {
        Task ResendAsync(string destination, string actionName, ReadOnlyMemory<byte> payload, CancellationToken cancellationToken = default);
    }
}
