using Greenhouse.MessageBus.Abstractions;
using Greenhouse.MessageBus.Events;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;

namespace Greenhouse.MessageBus.RabbitMQ
{
    public class MessageBusRabbitMQ : IMessageBus, IHostedService, IDisposable
    {
        public const string MESSAGE_QUEUE = "MESSAGE_QUEUE";
        public const string MESSAGE_EXCHANGE = "MESSAGE_EXCHANGE";

        private IConnectionFactory _connectionFactory;
        private readonly ILogger<DefaultRabbitMQPersistentConnection> _logger;
        private IConnection? _connection;
        private bool _disposed;

        public Task SendAsync(string destination, IntegrationMessage message, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task Subscribe<T, TH>()
            where T : IntegrationMessage
            where TH : IIntegrationMessageHandler<T>
        {
            throw new NotImplementedException();
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}
