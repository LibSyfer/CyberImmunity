using Greenhouse.Coordinator.Models;

namespace Greenhouse.Coordinator.Service
{
    public class CoordinatorService
    {
        private readonly ILogger<CoordinatorService> _logger;
        private bool _isBuzy;
        private Task? _currentGrowingTask;
        private CancellationTokenSource? _cancellationTokenSource;
        private readonly object _lock = new();

        private readonly DatabaseNetModule _databaseNetModule;

        public CoordinatorService(ILogger<CoordinatorService> logger, DatabaseNetModule databaseNetModule)
        {
            _logger = logger;
            _isBuzy = false;
            _databaseNetModule = databaseNetModule;
        }

        public bool IsBuzy => _isBuzy;

        public bool StartGrowing(Guid paramsId)
        {
            lock(_lock)
            {
                if (_currentGrowingTask != null && !_currentGrowingTask.IsCanceled)
                {
                    return false;
                }

                _cancellationTokenSource = new CancellationTokenSource();

                _currentGrowingTask = Task.Run(async () => await DoGrowingFlow(paramsId, _cancellationTokenSource.Token), _cancellationTokenSource.Token);
            }

            return true;
        }

        private async Task DoGrowingFlow(Guid paramsId, CancellationToken cancellationToken)
        {
            var growingParams = await _databaseNetModule.GetGrowingParamsAsync(paramsId, cancellationToken);

            // last step
        }
    }
}
