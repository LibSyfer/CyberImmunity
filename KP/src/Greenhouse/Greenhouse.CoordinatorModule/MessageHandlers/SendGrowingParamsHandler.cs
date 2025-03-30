using Greenhouse.MessageBus.Abstractions;
using Greenhouse.MessageBus.Messages;

namespace Greenhouse.CoordinatorModule.MessageHandlers;

public class SendGrowingParamsHandler : IIntegrationMessageHandler<SendGrowingParams>
{
    public Task Handle(SendGrowingParams message, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}
