using RabbitMQ.Client;

namespace Greenhouse.EventBus.RabbitMQ;

public interface IRabbitMQPersistentConnection
    : IDisposable
{
    bool IsConnected { get; }

    Task<bool> TryConnectAsync(CancellationToken cancellationToken = default);

    Task<IChannel> CreateCannelAsync(CancellationToken cancellationToken = default);
}
