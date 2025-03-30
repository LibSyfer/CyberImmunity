using Greenhouse.MessageBus.Abstractions;
using Greenhouse.MessageBus.Messages.SeedPlantingSystem;
using Greenhouse.MessageBus.Messages.TomatoDbConnectionModule;
using Greenhouse.Share;

namespace Greenhouse.CoordinatorModule.MessageHandlers
{
    public class GrowingParamsResultCommandHandler : IIntegrationMessageHandler<GrowingParamsResultCommand>
    {
        private readonly ILogger<GrowingParamsResultCommandHandler> _logger;
        private readonly IMessageBus _messageBus;

        public GrowingParamsResultCommandHandler(ILogger<GrowingParamsResultCommandHandler> logger,
            IMessageBus messageBus)
        {
            _logger = logger;
            _messageBus = messageBus;
        }

        public async Task Handle(GrowingParamsResultCommand message, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Получены параметры выращивания, инициализация высадки семян...");
            await _messageBus.SendAsync(GreenhouseServicesNames.SeedPlantingModule, new StartSeedingCommand(), cancellationToken);
        }
    }
}
