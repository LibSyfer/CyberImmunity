using Greenhouse.MessageBus.Abstractions;
using Greenhouse.MessageBus.Events;
using Microsoft.Extensions.DependencyInjection;

namespace Greenhouse.MessageBus.Extensions
{
    public static class MessageBusBuilderExtensions
    {
        public static IMonitorMessageBusBuilder RegisterMonitorHandler<T>(this IMonitorMessageBusBuilder messageBusBuilder)
            where T : class, IMonitorMessageHandler
        {
            messageBusBuilder.Services.AddTransient<IMonitorMessageHandler, T>();

            return messageBusBuilder;
        }

        public static IClientMessageBusBuilder RegisterMessageHandler<T, TH>(this IClientMessageBusBuilder messageBusBuilder)
            where T: IntegrationMessage
            where TH: class, IIntegrationMessageHandler<T>
        {
            messageBusBuilder.Services.AddKeyedTransient<IIntegrationMessageHandler, TH>(typeof(T));

            messageBusBuilder.Services.Configure<MessageBusRegister>(r =>
            {
                r.MessageTypes[typeof(T).Name] = typeof(T);
            });

            return messageBusBuilder;
        }
    }
}
