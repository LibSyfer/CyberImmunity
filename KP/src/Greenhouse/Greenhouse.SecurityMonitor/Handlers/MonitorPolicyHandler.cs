using Greenhouse.MessageBus.Abstractions;
using Greenhouse.MessageBus.Messages.ClimateControlModule;
using Greenhouse.MessageBus.Messages.SeedPlantingModule;
using Greenhouse.MessageBus.Messages.TomatoDbConnectionModule;
using Greenhouse.MessageBus.RabbitMQ.Extensions;
using Greenhouse.Share;

namespace Greenhouse.SecurityMonitor.Handlers
{
    public class MonitorPolicyHandler : IMonitorMessageHandler
    {
        private readonly ILogger<MonitorPolicyHandler> _logger;
        private readonly IMonitorMessageBus _monitorMessageBus;

        public MonitorPolicyHandler(ILogger<MonitorPolicyHandler> logger, IMonitorMessageBus monitorMessageBus)
        {
            _logger = logger;
            _monitorMessageBus = monitorMessageBus;
        }

        public async Task Handle(IDictionary<string, object?> metadata, ReadOnlyMemory<byte> payload, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation($"Получено сообщение, проверка политик...");

            var monitorHeaders = metadata.ReadMonitorHeaders();
            var authorizeAction = false;

            if (monitorHeaders.ActionName.Equals(nameof(GetGrowingParamsCommand), StringComparison.OrdinalIgnoreCase)
                && monitorHeaders.Source.Equals(GreenhouseServicesNames.CoordinatorModule, StringComparison.OrdinalIgnoreCase)
                && monitorHeaders.Destination.Equals(GreenhouseServicesNames.TomatoDbConnectionModule, StringComparison.OrdinalIgnoreCase))
                authorizeAction = true;

            if (monitorHeaders.ActionName.Equals(nameof(GrowingParamsResultCommand), StringComparison.OrdinalIgnoreCase)
                && monitorHeaders.Source.Equals(GreenhouseServicesNames.TomatoDbConnectionModule, StringComparison.OrdinalIgnoreCase)
                && monitorHeaders.Destination.Equals(GreenhouseServicesNames.CoordinatorModule, StringComparison.OrdinalIgnoreCase))
                authorizeAction = true;

            if (monitorHeaders.ActionName.Equals(nameof(StartSeedingCommand), StringComparison.OrdinalIgnoreCase)
                && monitorHeaders.Source.Equals(GreenhouseServicesNames.CoordinatorModule, StringComparison.OrdinalIgnoreCase)
                && monitorHeaders.Destination.Equals(GreenhouseServicesNames.SeedPlantingModule, StringComparison.OrdinalIgnoreCase))
                authorizeAction = true;

            if (monitorHeaders.ActionName.Equals(nameof(SeedingFinishCommand), StringComparison.OrdinalIgnoreCase)
                && monitorHeaders.Source.Equals(GreenhouseServicesNames.SeedPlantingModule, StringComparison.OrdinalIgnoreCase)
                && monitorHeaders.Destination.Equals(GreenhouseServicesNames.CoordinatorModule, StringComparison.OrdinalIgnoreCase))
                authorizeAction = true;

            if (monitorHeaders.ActionName.Equals(nameof(SetClimateControlParamsCommand), StringComparison.OrdinalIgnoreCase)
                && monitorHeaders.Source.Equals(GreenhouseServicesNames.CoordinatorModule, StringComparison.OrdinalIgnoreCase)
                && monitorHeaders.Destination.Equals(GreenhouseServicesNames.ClimateControlModule, StringComparison.OrdinalIgnoreCase))
                authorizeAction = true;

            if (authorizeAction)
            {
                _logger.LogInformation($"Действие [Action: {monitorHeaders.ActionName}] [Source: {monitorHeaders.Source}] [Destination: {monitorHeaders.Destination}] разрешено политиками безопасности");

                await _monitorMessageBus.ResendAsync(monitorHeaders.Destination, monitorHeaders.ActionName, payload, cancellationToken);
            }
            else
            {
                _logger.LogWarning($"Действие [Action: {monitorHeaders.ActionName}] [Source: {monitorHeaders.Source}] [Destination: {monitorHeaders.Destination}] запрещено политиками безопасности");
            }
        }
    }
}
