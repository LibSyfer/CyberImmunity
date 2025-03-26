using Greenhouse.MessageBus.Abstractions;
using Greenhouse.MessageBus.Events;
using Microsoft.Extensions.DependencyInjection;

namespace Greenhouse.MessageBus.Extensions
{
    public static class MessageBusBuilderExtensions
    {
        public static IMessageBusBuilder RegisterMonitorHandler<T>(this IMessageBusBuilder messageBusBuilder)
            where T : class, IMonitorMessageHandler
        {
            messageBusBuilder.Services.AddTransient<IMonitorMessageHandler, T>();

            return messageBusBuilder;
        }

        public static IMessageBusBuilder RegisterMessageHandler<T, TH>(this IMessageBusBuilder messageBusBuilder)
            where T: IntegrationMessage
            where TH : class, IIntegrationMessageHandler<T>
        {
            messageBusBuilder.Services.AddKeyedTransient<IIntegrationMessageHandler<T>, TH>(typeof(T));

            return messageBusBuilder;
        }
    }
}
