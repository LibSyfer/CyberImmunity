using Greenhouse.MessageBus.Extensions;
using Greenhouse.MessageBus.RabbitMQ.Extensions;
using Greenhouse.SecurityMonitor.Handlers;

var builder = Host.CreateApplicationBuilder(args);

builder.AddBaseRabbitMqServices()
        .ConfigureMonitorMessageBus()
        .RegisterMonitorHandler<MonitorPolicyHandler>();

// add rabbitmq connection settings
//var messageBusSection = builder.Configuration.GetSection(RabbitMQSettings.Section);
//builder.Services.Configure<RabbitMQSettings>(messageBusSection);

//builder.Services.AddSingleton<IRabbitMQPersistentConnection>(sp =>
//{
//    var logger = sp.GetRequiredService<ILogger<DefaultRabbitMQPersistentConnection>>();

//    var messageBusSettings = sp.GetRequiredService<IOptions<RabbitMQSettings>>().Value ?? throw new ArgumentNullException(nameof(RabbitMQSettings));

//    var factory = new ConnectionFactory
//    {
//        HostName = messageBusSettings.Host ?? throw new ArgumentNullException(nameof(messageBusSettings.Host)),
//    };

//    if (!string.IsNullOrEmpty(messageBusSettings.Username))
//    {
//        factory.UserName = messageBusSettings.Username;
//    }

//    if (!string.IsNullOrEmpty(messageBusSettings.Password))
//    {
//        factory.Password = messageBusSettings.Password;
//    }

//    return new DefaultRabbitMQPersistentConnection(factory, logger);
//});

// add monitor 
//builder.Services.AddSingleton<IMonitorMessageBus, MonitorMessageBusRabbitMQ>(sp =>
//{
//    var rabbitMQPersistentConnection = sp.GetRequiredService<IRabbitMQPersistentConnection>();
//    var logger = sp.GetRequiredService<ILogger<MonitorMessageBusRabbitMQ>>();

//    return new MonitorMessageBusRabbitMQ(rabbitMQPersistentConnection, logger, sp);
//});

// add message bus in hosted services for start consuming
//builder.Services.AddSingleton<IHostedService>(sp => (MonitorMessageBusRabbitMQ)sp.GetRequiredService<IMonitorMessageBus>());

// register consumer
//builder.Services.AddTransient<IMonitorMessageHandler, MonitorPolicyHandler>();

var host = builder.Build();

host.Run();
