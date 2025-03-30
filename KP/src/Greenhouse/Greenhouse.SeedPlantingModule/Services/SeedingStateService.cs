using Greenhouse.SeedPlantingModule.Models;

namespace Greenhouse.SeedPlantingModule.Services
{
    public class SeedingStateService
    {
        private readonly ILogger<SeedingStateService> _logger;
        private SeedingState _seedingState;
        private readonly object _sync = new();

        public SeedingStateService(ILogger<SeedingStateService> logger)
        {
            _logger = logger;
            _seedingState = SeedingState.Inactive;
        }

        public SeedingState State => _seedingState;

        public bool PrepareForSeeding()
        {
            if (_seedingState == SeedingState.Inactive)
            {
                _logger.LogInformation("Приготовление в высадке");
                ChangeState(SeedingState.ReadyToStart);
                return true;
            }

            _logger.LogWarning("Попытка приготовиться к высаживанию в состояниии: {SeedingState}", _seedingState.ToString());

            return false;
        }

        public bool StartSeeding()
        {
            if (_seedingState == SeedingState.ReadyToStart)
            {
                _logger.LogInformation("Начало высадки");
                ChangeState(SeedingState.Active);
                return true;
            }

            _logger.LogWarning("Попытка начать высаживание в состояниии: {SeedingState}", _seedingState.ToString());

            return false;
        }

        public bool FinishSeeding()
        {
            if (_seedingState == SeedingState.Active)
            {
                _logger.LogInformation("Завершение высадки");
                ChangeState(SeedingState.Inactive);
                return true;
            }

            _logger.LogWarning("Попытка закончить высаживание в состояниии: {SeedingState}", _seedingState.ToString());

            return false;
        }

        private void ChangeState(SeedingState newState)
        {
            lock(_sync)
            {
                _seedingState = newState;
            }
        }
    }
}
