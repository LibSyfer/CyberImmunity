using Greenhouse.EventBus.Events;

namespace Greenhouse.EventBus.Abstractions;

public interface IMessageBus
{
    Task PublishAsync(string destinationServiceName, IntegrationEvent @event, CancellationToken cancellationToken = default);

    void Subscribe<T, TH>()
            where T : IntegrationEvent
            where TH : IIntegrationEventHandler<T>;
}
