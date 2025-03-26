using Microsoft.Extensions.DependencyInjection;

namespace Greenhouse.MessageBus.Abstractions
{
    public interface IMessageBusBuilder
    {
        public IServiceCollection Services { get; }
    }
}
