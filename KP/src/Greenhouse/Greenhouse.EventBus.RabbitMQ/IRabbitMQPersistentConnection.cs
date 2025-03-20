using RabbitMQ.Client;

namespace Greenhouse.EventBus.RabbitMQ;

public interface IRabbitMQPersistentConnection
    : IDisposable
{
    bool IsConnected { get; }

    bool TryConnect();

    Task<IChannel> CreateCannelAsync(CancellationToken cancellationToken = default);
}
