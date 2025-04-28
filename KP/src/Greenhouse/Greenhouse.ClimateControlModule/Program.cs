using Greenhouse.ClimateControlModule.MessageHandlers;
using Greenhouse.ClimateControlModule.Services;
using Greenhouse.MessageBus.Extensions;
using Greenhouse.MessageBus.Messages.ClimateControlModule;
using Greenhouse.MessageBus.RabbitMQ.Extensions;
using Greenhouse.Share;

var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddSingleton<ClimateControlService>();
builder.Services.AddHostedService(sp => sp.GetRequiredService<ClimateControlService>());

builder.AddBaseRabbitMqServices()
        .ConfigureClientMessageBus(GreenhouseServicesNames.ClimateControlModule)
        .RegisterMessageHandler<SetClimateControlParamsCommand, SetClimateControlParamsCommandHandler>();

var host = builder.Build();
host.Run();
