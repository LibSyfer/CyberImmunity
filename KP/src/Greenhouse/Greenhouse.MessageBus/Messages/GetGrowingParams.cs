namespace Greenhouse.MessageBus.Messages
{
    public class GetGrowingParams : IntegrationMessage
    {
        public Guid ParamsId { get; set; }
    }
}

