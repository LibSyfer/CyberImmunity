using Microsoft.Extensions.Logging;
using RabbitMQ.Client;

namespace Greenhouse.EventBus.RabbitMQ
{
    class DefaultRabbitMQPersistentConnection : IRabbitMQPersistentConnection
    {
        private readonly IConnectionFactory _connectionFactory;
        private readonly ILogger<DefaultRabbitMQPersistentConnection> _logger;
        private readonly int _retryCount;
        private IConnection? _connection;
        private bool _disposed;

        private readonly object _sync = new();
        
        public DefaultRabbitMQPersistentConnection(IConnectionFactory connectionFactory, ILogger<DefaultRabbitMQPersistentConnection> logger, int retryCount = 5)
        {
            _connectionFactory = connectionFactory ?? throw new ArgumentNullException(nameof(connectionFactory));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _retryCount = retryCount;
            _disposed = false;
        }

        public bool IsConnected => _connection != null && _connection.IsOpen && !_disposed;

        public async Task<IChannel> CreateCannelAsync(CancellationToken cancellationToken = default)
        {
            if (!IsConnected)
            {
                throw new InvalidOperationException("No RabbitMQ connections are available to perform this action");
            }

            return await _connection!.CreateChannelAsync(cancellationToken: cancellationToken);
        }

        public void Dispose()
        {
            if (_disposed) return;

            _disposed = true;
            _connection.Dispose();
        }

        public bool TryConnect()
        {
            throw new NotImplementedException();
        }
    }
}
