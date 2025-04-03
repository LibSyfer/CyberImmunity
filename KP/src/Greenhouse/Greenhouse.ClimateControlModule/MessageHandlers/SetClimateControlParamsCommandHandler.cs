using Greenhouse.ClimateControlModule.Services;
using Greenhouse.MessageBus.Abstractions;
using Greenhouse.MessageBus.Messages.ClimateControlModule;

namespace Greenhouse.ClimateControlModule.MessageHandlers
{
    public class SetClimateControlParamsCommandHandler : IIntegrationMessageHandler<SetClimateControlParamsCommand>
    {
        private readonly ILogger<SetClimateControlParamsCommandHandler> _logger;
        private readonly ClimateControlService _climateControlService;

        public SetClimateControlParamsCommandHandler(ILogger<SetClimateControlParamsCommandHandler> logger, ClimateControlService climateControlService)
        {
            _logger = logger;
            _climateControlService = climateControlService;
        }

        public async Task Handle(SetClimateControlParamsCommand message, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Изменение параметров среды");

            await Task.Delay(5000, cancellationToken);

            _climateControlService.UpdateNecessaryParams(new Models.ClimateControlParams
            {
                TemperatureDay = message.TemperatureDay,
                TemperatureNight = message.TemperatureNight,
                HumidityLevel = message.HumidityLevel
            });
        }
    }
}
