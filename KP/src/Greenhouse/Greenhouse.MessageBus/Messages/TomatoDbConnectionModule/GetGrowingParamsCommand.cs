namespace Greenhouse.MessageBus.Messages.TomatoDbConnectionModule
{
    public class GetGrowingParamsCommand : IntegrationMessage
    {
        public Guid ParamsId { get; set; }
    }
}
