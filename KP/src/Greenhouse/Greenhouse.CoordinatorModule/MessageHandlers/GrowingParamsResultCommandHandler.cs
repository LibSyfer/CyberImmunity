using Greenhouse.CoordinatorModule.Services;
using Greenhouse.MessageBus.Abstractions;
using Greenhouse.MessageBus.Messages.SeedPlantingModule;
using Greenhouse.MessageBus.Messages.TomatoDbConnectionModule;
using Greenhouse.Share;

namespace Greenhouse.CoordinatorModule.MessageHandlers
{
    public class GrowingParamsResultCommandHandler : IIntegrationMessageHandler<GrowingParamsResultCommand>
    {
        private readonly ILogger<GrowingParamsResultCommandHandler> _logger;
        private readonly IMessageBus _messageBus;
        private readonly CoordinatorService _coordinatorService;

        public GrowingParamsResultCommandHandler(ILogger<GrowingParamsResultCommandHandler> logger,
            IMessageBus messageBus,
            CoordinatorService coordinatorService)
        {
            _logger = logger;
            _messageBus = messageBus;
            _coordinatorService = coordinatorService;
        }

        public async Task Handle(GrowingParamsResultCommand message, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Получены параметры выращивания, инициализация высадки семян...");
            _coordinatorService.SetGrowingParams(new Models.GrowingParams
            {
                TomatoId = message.TomatoId,
                LightIntensity = message.LightIntensity,
                LightDuration = message.LightDuration,
                TemperatureDay = message.TemperatureDay,
                TemperatureNight = message.TemperatureNight,
                HumidityLevel = message.HumidityLevel,
                WateringFrequency = message.WateringFrequency,
                FertilizerType = message.FertilizerType
            });

            await _messageBus.SendAsync(GreenhouseServicesNames.SeedPlantingModule, new StartSeedingCommand(), cancellationToken);
        }
    }
}
