namespace Greenhouse.MessageBus.RabbitMQ.Configurations
{
    public class RabbitMQSettings
    {
        public const string Section = "MessageBusConnection:RabbitMQ";

        public string Host { get; set; }

        public string Username { get; set; }

        public string Password { get; set; }
    }
}
