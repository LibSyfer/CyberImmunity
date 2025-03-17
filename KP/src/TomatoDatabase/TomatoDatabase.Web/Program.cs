using TomatoDatabase.Web.Models;

var builder = WebApplication.CreateBuilder(args);

var app = builder.Build();

var tomatos = new[]
{
    new Tomato { Id = Guid.Parse("715CD946-D92F-4B80-B1FF-1ED7F42387DA"), Name = "Мягкий" },
    new Tomato { Id = Guid.Parse("BEBD76C1-5915-4B7B-844F-1FBCC53EF4F0"), Name = "Сочный" },
    new Tomato { Id = Guid.Parse("5515DBBA-BD77-4B30-975D-C3D49D931D82"), Name = "Кислый" },
};

var tomatoGrowingParams = new[]
{
    new GrowingParams
    {
        Id = Guid.Parse("DF30DBEC-E088-4A62-A788-A023ED744029"),
        TomatoId = tomatos[0].Id,
        LightIntensity = 30000,             // 30 000 люксов
        LightDuration = 14,                 // 14 часов светового дня
        TemperatureDay = 25,                // 25 °C днем
        TemperatureNight = 18,              // 18 °C ночью
        HumidityLevel = 70,                 // 70% влажности
        WateringFrequency = 2,              // Полив каждые 2 дня
        FertilizerType = "NPK 20-20-20",    // Сбалансированное удобрение
    },
    new GrowingParams
    {
        Id = Guid.Parse("629A58D6-1C63-4B19-B15D-4E131CB48F4B"),
        TomatoId = tomatos[0].Id,
        LightIntensity = 50000,             // 50 000 люксов (естественный солнечный свет)
        LightDuration = 12,                 // 12 часов светового дня
        TemperatureDay = 28,                // 28 °C днем
        TemperatureNight = 20,              // 20 °C ночью
        HumidityLevel = 60,                 // 60% влажности
        WateringFrequency = 3,              // Полив каждые 3 дня
        FertilizerType = "Компост",         // Органическое удобрение
    },
    new GrowingParams
    {
        Id = Guid.Parse("0E8D8080-FA13-4448-B134-472C6101387C"),
        TomatoId = tomatos[1].Id,
        LightIntensity = 15000,             // 15 000 люксов
        LightDuration = 12,                 // 12 часов светового дня
        TemperatureDay = 20,                // 20 °C днем
        TemperatureNight = 16,              // 16 °C ночью
        HumidityLevel = 75,                 // 75% влажности
        WateringFrequency = 1,              // Полив каждый день
        FertilizerType = "Азотное",         // Удобрение с высоким содержанием азота
    },
    new GrowingParams
    {
        Id = Guid.Parse("A2440942-AA5F-4323-B97B-7677687BA720"),
        TomatoId = tomatos[1].Id,
        LightIntensity = 40000,             // 40 000 люксов
        LightDuration = 14,                 // 14 часов светового дня
        TemperatureDay = 26,                // 26 °C днем
        TemperatureNight = 19,              // 19 °C ночью
        HumidityLevel = 65,                 // 65% влажности
        WateringFrequency = 2,              // Полив каждые 2 дня
        FertilizerType = "Калийное",        // Удобрение с высоким содержанием калия
    },
    new GrowingParams
    {
        Id = Guid.Parse("02791717-4E57-4C88-98F5-177B5B978FAF"),
        TomatoId = tomatos[2].Id,
        LightIntensity = 35000,             // 35 000 люксов
        LightDuration = 12,                 // 12 часов светового дня
        TemperatureDay = 32,                // 32 °C днем
        TemperatureNight = 25,              // 25 °C ночью
        HumidityLevel = 50,                 // 50% влажности
        WateringFrequency = 1,              // Полив каждый день
        FertilizerType = "Фосфорное",       // Удобрение с высоким содержанием фосфора
    }
};

app.MapGet("/tomatos", () =>
{
    return tomatos;
});

app.MapGet("/tomatos/{tomatoId}/growing-params", (Guid tomatoId) =>
{
    var growingParamsList = tomatoGrowingParams.Where(e => e.TomatoId == tomatoId).ToList();
    return Results.Ok(growingParamsList);
});

app.MapGet("/tomatos/{tomatoId}/growing-params/{paramsId}", (Guid tomatoId, Guid paramsId) =>
{
    var growingParams = tomatoGrowingParams.Where(e => e.TomatoId == tomatoId && e.Id == paramsId).FirstOrDefault();
    if (growingParams == null)
    {
        return Results.NotFound();
    }

    return Results.Ok(growingParams);
});

app.Run();
