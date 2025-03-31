using Greenhouse.ClimateControlModule.Services;
using Greenhouse.MessageBus.Abstractions;
using Greenhouse.MessageBus.Messages.ClimateControlModule;

namespace Greenhouse.ClimateControlModule.MessageHandlers
{
    public class SetClimateControlParamsCommandHandler : IIntegrationMessageHandler<SetClimateControlParamsCommand>
    {
        private readonly ClimateControlService _climateControlService;

        public SetClimateControlParamsCommandHandler(ClimateControlService climateControlService)
        {
            _climateControlService = climateControlService;
        }

        public Task Handle(SetClimateControlParamsCommand message, CancellationToken cancellationToken = default)
        {
            _climateControlService.UpdateNecessaryParams(new Models.ClimateControlParams
            {
                TemperatureDay = message.TemperatureDay,
                TemperatureNight = message.TemperatureNight,
                HumidityLevel = message.HumidityLevel
            });

            return Task.CompletedTask;
        }
    }
}
