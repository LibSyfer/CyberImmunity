using Greenhouse.MessageBus.Extensions;
using Greenhouse.MessageBus.Messages.TomatoDbConnectionModule;
using Greenhouse.MessageBus.RabbitMQ.Extensions;
using Greenhouse.Share;
using Greenhouse.TomatoDbConnectionModule.MessageHandlers;
using Greenhouse.TomatoDbConnectionModule.Models;

var builder = Host.CreateApplicationBuilder(args);

var dbConnection = builder.Configuration.GetConnectionString("TomatoDb") ?? throw new ArgumentNullException("Db connection string");
builder.Services.AddHttpClient();
builder.Services.Configure<TomatoDbSettings>(opt =>
{
    opt.Host = dbConnection;
});

builder.AddBaseRabbitMqServices()
        .ConfigureClientMessageBus(GreenhouseServicesNames.TomatoDbConnectionModule)
        .RegisterMessageHandler<GetGrowingParamsCommand, GetGrowingParamsCommandHandler>();

var host = builder.Build();
host.Run();
