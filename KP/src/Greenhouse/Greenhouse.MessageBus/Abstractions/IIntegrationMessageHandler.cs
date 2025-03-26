using Greenhouse.MessageBus.Events;

namespace Greenhouse.MessageBus.Abstractions
{
    public interface IIntegrationMessageHandler<in TIntegrationMessage>
        where TIntegrationMessage : IntegrationMessage
    {
        Task Handle(TIntegrationMessage message, CancellationToken cancellationToken = default);
    }
}
