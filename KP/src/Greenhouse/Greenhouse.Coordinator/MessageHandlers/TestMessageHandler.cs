using Greenhouse.Coordinator.Messages;
using Greenhouse.MessageBus.Abstractions;

namespace Greenhouse.Coordinator.MessageHandlers
{
    public class TestMessageHandler : IIntegrationMessageHandler<TestMessage>
    {
        public TestMessageHandler() { }

        public Task Handle(TestMessage message, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }
    }
}
