using Greenhouse.Coordinator.Service;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<CoordinatorService>();
builder.Services.AddSingleton<DatabaseNetModule>();

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

app.MapPost("/growing", (CoordinatorService coordinatorService, Guid paramsId) =>
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