using Greenhouse.MessageBus.Extensions;
using Greenhouse.MessageBus.Messages.TomatoDbConnectionModule;
using Greenhouse.MessageBus.RabbitMQ.Extensions;
using Greenhouse.Share;
using Greenhouse.TomatoDbConnectionModule.MessageHandlers;

var builder = Host.CreateApplicationBuilder(args);

var dbConnection = builder.Configuration.GetConnectionString("tomatoDb") ?? throw new ArgumentNullException("Db connection string");
builder.Services.AddHttpClient<GetGrowingParamsCommandHandler>((sp, client) =>
{
    client.BaseAddress = new Uri(dbConnection);
});

builder.AddBaseRabbitMqServices()
        .ConfigureClientMessageBus(Services.TomatoDbConnectionModule)
        .RegisterMessageHandler<GetGrowingParamsCommand, GetGrowingParamsCommandHandler>();

var host = builder.Build();
host.Run();
