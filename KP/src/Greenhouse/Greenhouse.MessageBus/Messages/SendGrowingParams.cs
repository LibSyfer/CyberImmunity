namespace Greenhouse.MessageBus.Messages
{
    public class SendGrowingParams : IntegrationMessage
    {
        public Guid ParamsId { get; set; }
    }
}


