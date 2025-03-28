namespace Greenhouse.MessageBus.Messages
{
    public class IntegrationMessage
    {
        public string OperationName { get; set; }

        public string Source { get; set; }

        public string Destination { get; set; }
    }
}
