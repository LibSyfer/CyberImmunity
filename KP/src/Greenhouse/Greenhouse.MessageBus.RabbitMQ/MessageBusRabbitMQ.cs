using System.Text;
using System.Text.Json;
using Greenhouse.MessageBus.Abstractions;
using Greenhouse.MessageBus.Messages;
using Greenhouse.MessageBus.RabbitMQ.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Greenhouse.MessageBus.RabbitMQ
{
    public class MessageBusRabbitMQ : IMessageBus, IHostedService, IDisposable
    {
        private readonly IRabbitMQPersistentConnection _persistentConnection;
        private readonly ILogger<MessageBusRabbitMQ> _logger;
        private readonly IServiceProvider _serviceProvider;
        private readonly MessageBusRegister _messageBusRegister;
        private readonly string _exchangeName;
        private IChannel? _consumerChannel;

        public MessageBusRabbitMQ(IRabbitMQPersistentConnection persistentConnection,
            ILogger<MessageBusRabbitMQ> logger,
            IServiceProvider serviceProvider,
            IOptions<MessageBusRegister> mbrOptions,
            string exchangeName)
        {
            _persistentConnection = persistentConnection;
            _logger = logger;
            _serviceProvider = serviceProvider;
            _messageBusRegister = mbrOptions.Value;
            _exchangeName = exchangeName;
        }

        public async Task SendAsync(string destination, IntegrationMessage message, CancellationToken cancellationToken = default)
        {
            if (!_persistentConnection.IsConnected)
            {
                await _persistentConnection.TryConnectAsync(cancellationToken: cancellationToken);
            }

            using (var channel = await _persistentConnection.CreateChannelAsync(cancellationToken: cancellationToken))
            {
                var routingKey = message.GetType().Name;
                var monitorHeaders = new MonitorHeadersExtensions.MonitorHeaders() {
                    ActionName = routingKey,
                    Source = _exchangeName,
                    Destination = destination
                };

                var basicProperties = new BasicProperties();
                basicProperties.Headers = new Dictionary<string, object?>();
                basicProperties.Headers.WriteMonitorHeaders(monitorHeaders);

                await channel.ExchangeDeclareAsync(
                    exchange: MonitorMessageBusRabbitMQ.MONITOR_EXCHANGE,
                    type: "fanout",
                    cancellationToken: cancellationToken
                    );

                var stringMessage = JsonSerializer.Serialize(message);
                var body = Encoding.UTF8.GetBytes(stringMessage);

                await channel.BasicPublishAsync(
                    exchange: MonitorMessageBusRabbitMQ.MONITOR_EXCHANGE,
                    routingKey: "",
                    mandatory: true,
                    basicProperties: basicProperties,
                    body: body,
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
            if (_consumerChannel is not null) {
                _consumerChannel.Dispose();
            }
        }

        private async Task<IChannel> CreateConsumerChannelAsync(CancellationToken cancellationToken)
        {
            if (!_persistentConnection.IsConnected)
            {
                await _persistentConnection.TryConnectAsync(cancellationToken: cancellationToken);
            }

            _logger.LogTrace("Creating RabbitMQ consumer channel");

            var channel = await _persistentConnection.CreateChannelAsync(cancellationToken: cancellationToken);
            await channel.ExchangeDeclareAsync(
                exchange: _exchangeName,
                type: "direct",
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
                foreach (var (messageName, _) in _messageBusRegister.MessageTypes)
                {
                    var consumer = new AsyncEventingBasicConsumer(_consumerChannel);
                    consumer.ReceivedAsync += OnMessageReceivedAsync;

                    var queueName = $"{_exchangeName}-{messageName}";

                    await _consumerChannel.QueueDeclareAsync(
                        queue: queueName,
                        durable: true,
                        exclusive: false,
                        autoDelete: false,
                        arguments: null,
                        cancellationToken: cancellationToken
                        );

                    await _consumerChannel.QueueBindAsync(
                        queue: queueName,
                        exchange: _exchangeName,
                        routingKey: messageName,
                        cancellationToken: cancellationToken
                        );

                    await _consumerChannel.BasicConsumeAsync(
                        queue: queueName,
                        autoAck: false,
                        consumer: consumer,
                        cancellationToken: cancellationToken
                        );
                }
            }
            else
            {
                _logger.LogError("StartBasicConsume can't call on _consumerChannel == null");
            }
        }

        private async Task OnMessageReceivedAsync(object sender, BasicDeliverEventArgs args)
        {
            var messageName = args.RoutingKey;
            var stringMessage = Encoding.UTF8.GetString(args.Body.ToArray());
            
            await ProcessMessage(messageName, stringMessage, args.CancellationToken);

            await _consumerChannel!.BasicAckAsync(args.DeliveryTag, multiple: false, cancellationToken: args.CancellationToken);
        }

        private async Task ProcessMessage(string messageName, string stringMessgae, CancellationToken cancellationToken)
        {
            await using var scope = _serviceProvider.CreateAsyncScope();

            if (!_messageBusRegister.MessageTypes.TryGetValue(messageName, out var messageType))
            {
                _logger.LogWarning("Unable to resolve event type for event name {MessageName}", messageName);
                return;
            }

            var message = JsonSerializer.Deserialize(stringMessgae, messageType) as IntegrationMessage;
            if (message is null)
            {
                _logger.LogWarning("Deserialization error of consuming message in RabbitMQ event: {MessageName}", messageName);
                return;
            }

            var handlers = scope.ServiceProvider.GetKeyedServices<IIntegrationMessageHandler>(messageType);
            if (handlers is not null)
            {
                foreach (var handler in handlers)
                {
                    await Task.Yield();
                    _logger.LogTrace("Processing RabbitMQ message: {ActionName} with handler: {MessageHandler}", messageName, nameof(handler));
                    await handler.Handle(message, cancellationToken);
                }
            }
            else
            {
                _logger.LogWarning("No consume handlers registered");
            }
        }
    }
}
