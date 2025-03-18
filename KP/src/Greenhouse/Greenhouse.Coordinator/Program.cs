using Greenhouse.Coordinator;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<CoordinatorService>();

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

app.MapPost("/growing", (Guid paramsId) =>
{
    return Results.Ok(new
    {
        GrowindId = Guid.NewGuid()
    });
})
.WithDisplayName("growing")
.WithOpenApi();

app.Run();