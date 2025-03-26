using RabbitMQ.Client;

namespace Greenhouse.MessageBus.RabbitMQ
{
    public interface IRabbitMQPersistentConnection : IDisposable
    {
        bool IsConnected { get; }

        Task<bool> TryConnectAsync(CancellationToken cancellationToken = default);

        Task<IChannel> CreateChannelAsync(CancellationToken cancellationToken = default);
    }
}
