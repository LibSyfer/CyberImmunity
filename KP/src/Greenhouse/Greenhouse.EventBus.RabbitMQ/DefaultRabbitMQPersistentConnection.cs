using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

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
            _connection?.Dispose();
        }

        public async Task<bool> TryConnectAsync(CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("RabbitMQ Client is trying to connect");

            var newConnection = await _connectionFactory.CreateConnectionAsync(cancellationToken: cancellationToken);

            lock(_sync)
            {
                _connection = newConnection;

                if (IsConnected) {
                    _connection.ConnectionShutdownAsync += OnConnectionShutdownAsync;
                    _connection.CallbackExceptionAsync += OnCallbackExceptionAsync;
                    _connection.ConnectionBlockedAsync += OnConnectionBlocked;

                    _logger.LogInformation("RabbitMQ Client acquired a persistent connection to '{HostName}' and is subscribed to failure events", _connection.Endpoint.HostName);

                    return true;
                }
                else
                {
                    _logger.LogCritical("FATAL ERROR: RabbitMQ connections could not be created and opened");

                    return false;
                }
            }
        }

        private async Task OnConnectionShutdownAsync(object sender, ShutdownEventArgs args)
        {
            if (_disposed) return;

            _logger.LogWarning("A RabbitMQ connection is on shutdown. Trying to re-connect...");

            await TryConnectAsync();
        }

        private async Task OnCallbackExceptionAsync(object sender, CallbackExceptionEventArgs args)
        {
            if (_disposed) return;

            _logger.LogWarning("A RabbitMQ connection throw exception. Trying to re-connect...");

            await TryConnectAsync();
        }

        private async Task OnConnectionBlocked(object sender, ConnectionBlockedEventArgs args)
        {
            if (_disposed) return;

            _logger.LogWarning("A RabbitMQ connection is shutdown. Trying to re-connect...");

            await TryConnectAsync();
        }
    }
}
