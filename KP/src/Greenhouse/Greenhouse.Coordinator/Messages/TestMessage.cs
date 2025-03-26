using Greenhouse.MessageBus.Events;

namespace Greenhouse.Coordinator.Messages
{
    public class TestMessage : IntegrationMessage
    {
        public string TestFeild { get; set; }
    }
}
