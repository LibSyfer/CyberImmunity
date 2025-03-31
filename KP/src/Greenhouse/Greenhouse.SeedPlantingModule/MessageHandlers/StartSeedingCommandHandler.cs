using Greenhouse.MessageBus.Abstractions;
using Greenhouse.MessageBus.Messages.SeedPlantingModule;
using Greenhouse.SeedPlantingModule.Services;

namespace Greenhouse.SeedPlantingModule.MessageHandlers
{
    public class StartSeedingCommandHandler : IIntegrationMessageHandler<StartSeedingCommand>
    {
        private readonly ILogger<StartSeedingCommandHandler> _logger;
        private readonly SeedingStateService _seedingStateService;

        public StartSeedingCommandHandler(ILogger<StartSeedingCommandHandler> logger, SeedingStateService seedingStateService)
        {
            _logger = logger;
            _seedingStateService = seedingStateService;
        }

        public Task Handle(StartSeedingCommand message, CancellationToken cancellationToken = default)
        {
            _logger.LogTrace("Получено сообщение: {MessageName}", nameof(message));

            _seedingStateService.PrepareForSeeding();

            return Task.CompletedTask;
        }
    }
}
