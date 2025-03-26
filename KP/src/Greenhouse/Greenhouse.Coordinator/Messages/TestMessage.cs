using Greenhouse.MessageBus.Events;

namespace Greenhouse.Coordinator.Messages
{
    public class TestMessage : IntegrationMessage
    {
        public TestMessage() { }

        public string TestFeild { get; set; }
    }
}
