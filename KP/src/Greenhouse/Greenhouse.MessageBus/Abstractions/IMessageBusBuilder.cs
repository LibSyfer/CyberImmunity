using Microsoft.Extensions.DependencyInjection;

namespace Greenhouse.MessageBus.Abstractions
{
    public interface IMessageBusBuilder
    {
        public IServiceCollection Services { get; }
    }

    public interface IClientMessageBusBuilder
    {
        public IServiceCollection Services { get; }
    }
    public interface IMonitorMessageBusBuilder
    {
        public IServiceCollection Services { get; }
    }
}
