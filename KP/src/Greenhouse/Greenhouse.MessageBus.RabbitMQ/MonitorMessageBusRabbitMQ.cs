using Greenhouse.MessageBus.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Greenhouse.MessageBus.RabbitMQ
{
    public class MonitorMessageBusRabbitMQ : IMonitorMessageBus, IHostedService, IDisposable
    {
        public const string MONITOR_QUEUE = "MONITOR_QUEUE";
        public const string MONITOR_EXCHANGE = "MONITOR_EXCHANGE";

        private readonly IRabbitMQPersistentConnection _persistentConnection;
        private readonly ILogger<MonitorMessageBusRabbitMQ> _logger;
        private readonly IServiceProvider _serviceProvider;
        private IChannel? _consumerChannel;

        public MonitorMessageBusRabbitMQ(IRabbitMQPersistentConnection persistentConnection, ILogger<MonitorMessageBusRabbitMQ> logger,
            IServiceProvider serviceProvider)
        {
            _persistentConnection = persistentConnection;
            _logger = logger;
            _serviceProvider = serviceProvider;
        }

        public async Task ResendAsync(string destination, string actionName, ReadOnlyMemory<byte> payload, CancellationToken cancellationToken = default)
        {
            if (!_persistentConnection.IsConnected)
            {
                await _persistentConnection.TryConnectAsync(cancellationToken: cancellationToken);
            }

            _logger.LogTrace("Creating RabbitMQ channel to send verified message: {ActionName}", actionName);

            using (var channel = await _persistentConnection.CreateChannelAsync(cancellationToken))
            {
                _logger.LogTrace("Declaring RabbitMQ destination service exchange [{ExchangeName}] to send verified message: {ActionName}", destination, actionName);

                await channel.ExchangeDeclareAsync(
                    exchange: destination,
                    type: "direct",
                    cancellationToken: cancellationToken
                    );

                _logger.LogTrace("Send message to RabbitMQ: {ActionName}", actionName);

                await channel.BasicPublishAsync(
                        exchange: destination,
                        routingKey: actionName,
                        mandatory: true,
                        basicProperties: new BasicProperties(),
                        body: payload,
                        cancellationToken: cancellationToken
                        );
            }
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Starting RabbitMQ connection");
            _consumerChannel = await CreateConsumerChannelAsync(cancellationToken);
            await StartBasicConsumeAsync(cancellationToken);
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        public void Dispose()
        {
            if (_consumerChannel != null)
            {
                _consumerChannel.Dispose();
            }
        }

        private async Task<IChannel> CreateConsumerChannelAsync(CancellationToken cancellationToken = default)
        {
            if (!_persistentConnection.IsConnected)
            {
                await _persistentConnection.TryConnectAsync(cancellationToken);
            }

            _logger.LogTrace("Creating RabbitMQ consumer channel");

            var channel = await _persistentConnection.CreateChannelAsync(cancellationToken);
            await channel.ExchangeDeclareAsync(
                exchange: MONITOR_EXCHANGE,
                type: "fanout",
                cancellationToken: cancellationToken
                );

            await channel.QueueDeclareAsync(
                queue: MONITOR_QUEUE,
                durable: true,
                exclusive: false,
                autoDelete: false,
                arguments: null,
                cancellationToken: cancellationToken
                );

            await channel.QueueBindAsync(
                    queue: MONITOR_QUEUE,
                    exchange: MONITOR_EXCHANGE,
                    routingKey: "",
                    cancellationToken: cancellationToken
                    );

            channel.CallbackExceptionAsync += async (sender, args) =>
            {
                _logger.LogWarning(args.Exception, "Recreating RabbitMQ consumer channel");

                _consumerChannel?.Dispose();
                _consumerChannel = await CreateConsumerChannelAsync(args.CancellationToken);
                await StartBasicConsumeAsync(args.CancellationToken);
            };

            return channel;
        }

        private async Task StartBasicConsumeAsync(CancellationToken cancellationToken = default)
        {
            _logger.LogTrace("Starting RabbitMQ basic consume");
            
            if (_consumerChannel != null)
            {
                var consumer = new AsyncEventingBasicConsumer(_consumerChannel);
                consumer.ReceivedAsync += OnMessageReceivedAsync;

                await _consumerChannel.BasicConsumeAsync(
                    queue: MONITOR_QUEUE,
                    autoAck: false,
                    consumer: consumer,
                    cancellationToken: cancellationToken
                    );
            }
            else
            {
                _logger.LogError("StartBasicConsume can't call on _consumerChannel == null");
            }
        }

        private async Task OnMessageReceivedAsync(object sender, BasicDeliverEventArgs args)
        {
            await using var scope = _serviceProvider.CreateAsyncScope();

            var handlers = scope.ServiceProvider.GetServices<IMonitorMessageHandler>();
            if (handlers is not null)
            {
                foreach (var handler in handlers) {
                    await Task.Yield();
                    _logger.LogTrace("Processing RabbitMQ message with handler: {MessageHandler}", nameof(handler));
                    await handler.Handle(args.BasicProperties.Headers ?? new Dictionary<string, object?>(), args.Body, args.CancellationToken);
                }
            }
            else
            {
                _logger.LogWarning("No monitor consume handlers registered");
            }

            await _consumerChannel!.BasicAckAsync(args.DeliveryTag, multiple: false, cancellationToken: args.CancellationToken);
        }
    }
}
