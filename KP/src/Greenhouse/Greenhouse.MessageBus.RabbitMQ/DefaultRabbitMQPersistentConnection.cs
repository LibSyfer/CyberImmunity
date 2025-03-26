using Microsoft.Extensions.Logging;
using Polly;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMQ.Client.Exceptions;
using System.Net.Sockets;

namespace Greenhouse.MessageBus.RabbitMQ
{
    public class DefaultRabbitMQPersistentConnection : IRabbitMQPersistentConnection
    {
        private IConnectionFactory _connectionFactory;
        private readonly ILogger<DefaultRabbitMQPersistentConnection> _logger;
        private IConnection? _connection;
        private readonly int _retryCount;
        private bool _disposed;

        private readonly object _sync = new();

        public DefaultRabbitMQPersistentConnection(
            IConnectionFactory connectionFactory,
            ILogger<DefaultRabbitMQPersistentConnection> logger,
            int retryCount = 5)
        {
            _connectionFactory = connectionFactory;
            _logger = logger;
            _retryCount = retryCount;
        }

        public bool IsConnected => _connection != null && _connection.IsOpen && !_disposed;

        public async Task<IChannel> CreateChannelAsync(CancellationToken cancellationToken = default)
        {
            if (!IsConnected)
            {
                throw new InvalidOperationException("No RabbitMQ connections are available to perform this action");
            }

            return await _connection!.CreateChannelAsync(cancellationToken: cancellationToken);
        }

        public async Task<bool> TryConnectAsync(CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("RabbitMQ Client is trying to connect");

            var policy = Policy.Handle<SocketException>()
                    .Or<BrokerUnreachableException>()
                    .WaitAndRetryAsync(_retryCount, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)), (ex, time) =>
                    {
                        _logger.LogWarning("RabbitMQ Client could not connect after {TimeOut}s ({ExceptionMessage})", $"{time.TotalSeconds:n1}", ex.Message);
                    }
                );

            IConnection? newConnection = null;
            await policy.ExecuteAsync(async () =>
            {
                newConnection = await _connectionFactory.CreateConnectionAsync(cancellationToken: cancellationToken);
            });

            if (newConnection is null)
            {
                _logger.LogCritical("FATAL ERROR: RabbitMQ connections could not be created and opened");
                return false;
            }

            lock(_sync)
            {
                _connection = newConnection;

                if (IsConnected)
                {
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

        public void Dispose()
        {
            if (_disposed) return;

            _disposed = true;
            _connection?.Dispose();
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
