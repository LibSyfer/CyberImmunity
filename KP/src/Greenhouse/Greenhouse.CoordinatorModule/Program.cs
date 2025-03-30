using Greenhouse.CoordinatorModule.MessageHandlers;
using Greenhouse.CoordinatorModule.Services;
using Greenhouse.MessageBus.Extensions;
using Greenhouse.MessageBus.Messages.SeedPlantingSystem;
using Greenhouse.MessageBus.Messages.TomatoDbConnectionModule;
using Greenhouse.MessageBus.RabbitMQ.Extensions;
using Greenhouse.Share;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<CoordinatorService>();

builder.AddBaseRabbitMqServices()
        .ConfigureClientMessageBus(GreenhouseServicesNames.CoordinatorModule)
        .RegisterMessageHandler<GrowingParamsResultCommand, GrowingParamsResultCommandHandler>()
        .RegisterMessageHandler<SeedingFinishCommand, SeedingFinishCommandHandler>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapGet("/status", (CoordinatorService coordinatorService) =>
{
    return Results.Ok(new
    {
        Status = coordinatorService.IsBuzy ? "buzy" : "free"
    });
})
.WithDisplayName("status")
.WithOpenApi();

app.MapPost("/growing", async (CoordinatorService coordinatorService, Guid paramsId, CancellationToken cancellationToken) =>
{
    var isSuccess = coordinatorService.StartGrowing(paramsId);
    if (!isSuccess)
    {
        return Results.BadRequest();
    }
    return Results.Ok();
})
.WithDisplayName("growing")
.WithOpenApi();

app.Run();