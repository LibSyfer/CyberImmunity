using Greenhouse.MessageBus.Abstractions;
using Greenhouse.MessageBus.RabbitMQ.Configurations;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;

namespace Greenhouse.MessageBus.RabbitMQ.Extensions
{
    public static class RabbitMqDependencyInjectionExtensions
    {
        public static IMessageBusBuilder AddBaseRabbitMqServices(this IHostApplicationBuilder builder)
        {
            var messageBusSection = builder.Configuration.GetSection(RabbitMQSettings.Section);
            builder.Services.Configure<RabbitMQSettings>(messageBusSection);

            builder.Services.AddSingleton<IRabbitMQPersistentConnection>(sp =>
            {
                var logger = sp.GetRequiredService<ILogger<DefaultRabbitMQPersistentConnection>>();

                var messageBusSettings = sp.GetRequiredService<IOptions<RabbitMQSettings>>().Value ?? throw new ArgumentNullException(nameof(RabbitMQSettings));

                var factory = new ConnectionFactory
                {
                    HostName = messageBusSettings.Host ?? throw new ArgumentNullException(nameof(messageBusSettings.Host)),
                };

                if (!string.IsNullOrEmpty(messageBusSettings.Username))
                {
                    factory.UserName = messageBusSettings.Username;
                }

                if (!string.IsNullOrEmpty(messageBusSettings.Password))
                {
                    factory.Password = messageBusSettings.Password;
                }

                return new DefaultRabbitMQPersistentConnection(factory, logger);
            });

            return new MessageBusBuilder(builder.Services);
        }

        public static IMessageBusBuilder ConfigureClientMessageBus(this IMessageBusBuilder messageBusBuilder)
        {
            return messageBusBuilder;
        }

        public static IMessageBusBuilder ConfigureMonitorMessageBus(this IMessageBusBuilder messageBusBuilder)
        {
            messageBusBuilder.Services.AddSingleton<IMonitorMessageBus, MonitorMessageBusRabbitMQ>(sp =>
            {
                var rabbitMQPersistentConnection = sp.GetRequiredService<IRabbitMQPersistentConnection>();
                var logger = sp.GetRequiredService<ILogger<MonitorMessageBusRabbitMQ>>();

                return new MonitorMessageBusRabbitMQ(rabbitMQPersistentConnection, logger, sp);
            });

            messageBusBuilder.Services.AddSingleton<IHostedService>(sp => (MonitorMessageBusRabbitMQ)sp.GetRequiredService<IMonitorMessageBus>());

            return messageBusBuilder;
        }

        private class MessageBusBuilder(IServiceCollection services) : IMessageBusBuilder
        {
            public IServiceCollection Services => services;
        }
    }
}
