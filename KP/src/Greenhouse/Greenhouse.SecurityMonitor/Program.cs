using Greenhouse.MessageBus.Extensions;
using Greenhouse.MessageBus.RabbitMQ.Extensions;
using Greenhouse.SecurityMonitor.Handlers;

var builder = Host.CreateApplicationBuilder(args);

builder.AddBaseRabbitMqServices()
        .ConfigureMonitorMessageBus()
        .RegisterMonitorHandler<MonitorPolicyHandler>();

var host = builder.Build();

host.Run();
