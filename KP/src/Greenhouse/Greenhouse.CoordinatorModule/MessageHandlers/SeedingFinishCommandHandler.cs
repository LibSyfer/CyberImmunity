using Greenhouse.CoordinatorModule.Services;
using Greenhouse.MessageBus.Abstractions;
using Greenhouse.MessageBus.Messages.ClimateControlModule;
using Greenhouse.MessageBus.Messages.SeedPlantingModule;
using Greenhouse.Share;

namespace Greenhouse.CoordinatorModule.MessageHandlers
{
    public class SeedingFinishCommandHandler : IIntegrationMessageHandler<SeedingFinishCommand>
    {
        private readonly ILogger<SeedingFinishCommandHandler> _logger;
        private readonly IMessageBus _messageBus;
        private readonly CoordinatorService _coordinatorService;

        public SeedingFinishCommandHandler(ILogger<SeedingFinishCommandHandler> logger,
            IMessageBus messageBus,
            CoordinatorService coordinatorService)
        {
            _logger = logger;
            _messageBus = messageBus;
            _coordinatorService = coordinatorService;
        }

        public async Task Handle(SeedingFinishCommand message, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Высадка семян завершена, инициализация настройки параметров выращивания...");

            await Task.Delay(5000, cancellationToken);

            if (_coordinatorService.GrowingParams is null)
            {
                _logger.LogError("Выращивание прервано. Параметры выращивания не получены, нельзя установить параметры среды");
                return;
            }

            _logger.LogInformation("Отправка параметров климата в модуль климат контроля...");
            var growingParams = _coordinatorService.GrowingParams;
            await _messageBus.SendAsync(GreenhouseServicesNames.ClimateControlModule, new SetClimateControlParamsCommand
            {
                TemperatureDay = growingParams.TemperatureDay,
                TemperatureNight = growingParams.TemperatureNight,
                HumidityLevel = growingParams.HumidityLevel
            });

            _logger.LogInformation("Выращивание прервано. Конец реализации");
        }
    }
}
