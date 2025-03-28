using Greenhouse.MessageBus.Messages;

namespace Greenhouse.MessageBus.Abstractions
{
    public interface IIntegrationMessageHandler<in TIntegrationMessage> : IIntegrationMessageHandler
        where TIntegrationMessage : IntegrationMessage
    {
        Task IIntegrationMessageHandler.Handle(IntegrationMessage message, CancellationToken cancellationToken) => Handle((TIntegrationMessage)message, cancellationToken);

        Task Handle(TIntegrationMessage message, CancellationToken cancellationToken = default);
    }

    public interface IIntegrationMessageHandler
    {
        Task Handle(IntegrationMessage message, CancellationToken cancellationToken = default);
    }
}
