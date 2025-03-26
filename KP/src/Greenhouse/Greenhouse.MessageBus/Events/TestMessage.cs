namespace Greenhouse.MessageBus.Events
{
    public class TestMessage : IntegrationMessage
    {
        public TestMessage() { }

        public string TestFeild { get; set; }
    }
}
