using Greenhouse.MessageBus.Abstractions;
using Greenhouse.MessageBus.Messages.SeedPlantingSystem;

namespace Greenhouse.CoordinatorModule.MessageHandlers
{
    public class SeedingFinishCommandHandler : IIntegrationMessageHandler<SeedingFinishCommand>
    {
        private readonly ILogger<SeedingFinishCommandHandler> _logger;

        public SeedingFinishCommandHandler(ILogger<SeedingFinishCommandHandler> logger)
        {
            _logger = logger;
        }

        public async Task Handle(SeedingFinishCommand message, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Высадка семян завершена, инициализация настройки параметров выращивания...");

            await Task.Delay(5000, cancellationToken);

            _logger.LogInformation("Выращивание прервано. Конец реализации");
        }
    }
}
