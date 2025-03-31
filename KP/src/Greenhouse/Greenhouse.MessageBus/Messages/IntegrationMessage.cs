namespace Greenhouse.MessageBus.Messages
{
    public class IntegrationMessage
    {
        public Guid Id { get; set; }

        public IntegrationMessage()
        {
            Id = Guid.NewGuid();
        }
    }
}
