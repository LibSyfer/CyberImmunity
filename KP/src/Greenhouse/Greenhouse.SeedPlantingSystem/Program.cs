using Greenhouse.MessageBus.Abstractions;
using Greenhouse.MessageBus.Extensions;
using Greenhouse.MessageBus.Messages.SeedPlantingSystem;
using Greenhouse.MessageBus.RabbitMQ.Extensions;
using Greenhouse.SeedPlantingSystem.MessageHandlers;
using Greenhouse.SeedPlantingSystem.Models;
using Greenhouse.SeedPlantingSystem.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<SeedingStateService>();

builder.AddBaseRabbitMqServices()
        .ConfigureClientMessageBus("SeedPlantingSystem")
        .RegisterMessageHandler<StartSeedingCommand, StartSeedingCommandHandler>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.MapGet("/status", (SeedingStateService seedingStateService) =>
{
    var seedingState = seedingStateService.State;
    switch (seedingState)
    {
        case SeedingState.ReadyToStart:
            return Results.Ok(new
            {
                Code = SeedingState.ReadyToStart,
                Message = "Все готово для начала высадки"
            });

        case SeedingState.Active:
            return Results.Ok(new
            {
                Code = SeedingState.Active,
                Message = "Высадка семян"
            });

        default:
            return Results.Ok(new
            {
                Code = SeedingState.Inactive,
                Message = "Высадка не производится"
            });
    }
});

app.MapPost("/start-seeding", (SeedingStateService seedingStateService) =>
{
    var seedingState = seedingStateService.State;
    if (seedingState == SeedingState.ReadyToStart)
    {
        seedingStateService.StartSeeding();

        return Results.Ok("Началась высадка семян");
    }

    return Results.BadRequest($"Нельзя начать высадку из состояния: {seedingState.ToString()}");
});

app.MapPost("/finish-seeding", async (SeedingStateService seedingStateService, IMessageBus messageBus, CancellationToken cancellationToken) =>
{
    var seedingState = seedingStateService.State;
    if (seedingState == SeedingState.Active)
    {
        seedingStateService.FinishSeeding();

        await messageBus.SendAsync("CoordinatorModule", new SeedingFinishCommand(), cancellationToken);

        return Results.Ok("Закончилась высадка семян");
    }

    return Results.BadRequest($"Нельзя закончить высадку в состоянии: {seedingState.ToString()}");
});

app.Run();
