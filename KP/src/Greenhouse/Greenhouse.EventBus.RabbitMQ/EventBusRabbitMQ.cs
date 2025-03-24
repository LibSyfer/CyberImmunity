using Greenhouse.EventBus.Abstractions;
using Greenhouse.EventBus.Events;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using System.Text;
using System.Text.Json;

namespace Greenhouse.EventBus.RabbitMQ;

public class EventBusRabbitMQ : IMessageBus, IHostedService, IDisposable
{
    const string BROKER_NAME = "GREENHOUSE";
    const string MONITOR_NAME = "MONITOR";
    const string AUTOFAC_SCOPE_NAME = "GREENHOUSE";
    
    private readonly IRabbitMQPersistentConnection _persistentConnection;
    private readonly ILogger<EventBusRabbitMQ> _logger;

    private string _serviceName;

    public EventBusRabbitMQ(IRabbitMQPersistentConnection persistentConnection,
        ILogger<EventBusRabbitMQ> logger,
        string serviceName)
    {
       _persistentConnection = persistentConnection;
       _logger = logger;
       _serviceName = serviceName;
    }

    public async Task PublishAsync(string destinationServiceName, IntegrationEvent @event, CancellationToken cancellationToken = default)
    {
        if (!_persistentConnection.IsConnected)
        {
            await _persistentConnection.TryConnectAsync(cancellationToken);
        }

        var eventName = @event.GetType().Name;

        _logger.LogTrace("Creating RabbitMQ channel to publish event: {EventId} ({EventName})", @event.Id, eventName);

        using (var channel = await _persistentConnection.CreateCannelAsync(cancellationToken))
        {
            _logger.LogTrace("Declaring RabbitMQ exchange to publish event: {EventId}", @event.Id);

            await channel.ExchangeDeclareAsync(
                exchange: BROKER_NAME,
                type: "direct",
                durable: false,
                autoDelete: true);

            var message = JsonSerializer.Serialize(@event);
            var body = Encoding.UTF8.GetBytes(message);

            var properties = new BasicProperties();
            properties.Headers = new Dictionary<string, object?>
            {
                { "source", _serviceName },
                { "destination", destinationServiceName }
            };

            await channel.BasicPublishAsync(
                exchange: BROKER_NAME,
                routingKey: MONITOR_NAME,
                mandatory: true,
                basicProperties: properties,
                body: body,
                cancellationToken: cancellationToken
            );
        }
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public void Subscribe<T, TH>()
        where T : IntegrationEvent
        where TH : IIntegrationEventHandler<T>
    {
        throw new NotImplementedException();
    }

    public void Unsubscribe<T, TH>()
        where T : IntegrationEvent
        where TH : IIntegrationEventHandler<T>
    {
        throw new NotImplementedException();
    }

    public void Dispose()
    {
        throw new NotImplementedException();
    }
}
