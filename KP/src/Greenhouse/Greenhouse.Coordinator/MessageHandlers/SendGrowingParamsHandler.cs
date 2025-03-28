using Greenhouse.MessageBus.Abstractions;
using Greenhouse.MessageBus.Messages;

namespace Greenhouse.Coordinator.MessageHandlers;

public class SendGrowingParamsHandler : IIntegrationMessageHandler<SendGrowingParams>
{
    public Task Handle(SendGrowingParams message, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}
