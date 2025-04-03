using Greenhouse.CoordinatorModule.Models;
using Greenhouse.MessageBus.Abstractions;
using Greenhouse.MessageBus.Messages.TomatoDbConnectionModule;
using Greenhouse.Share;

namespace Greenhouse.CoordinatorModule.Services
{
    public class CoordinatorService
    {
        private readonly ILogger<CoordinatorService> _logger;
        private bool _isBuzy;
        private Task? _currentGrowingTask;
        private CancellationTokenSource? _cancellationTokenSource;
        private readonly object _lock = new();

        private GrowingParams? _growingParams;

        private readonly IMessageBus _messageBus;

        public CoordinatorService(ILogger<CoordinatorService> logger, IMessageBus messageBus)
        {
            _logger = logger;
            _isBuzy = false;
            _messageBus = messageBus;
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

                _currentGrowingTask = Task.Run(async () =>
                {
                    _logger.LogInformation("Начало процесса выращивания");

                    await Task.Delay(5000, _cancellationTokenSource.Token);
                    _logger.LogInformation("Получение параметров выращивания");
                    await _messageBus.SendAsync(GreenhouseServicesNames.TomatoDbConnectionModule, new GetGrowingParamsCommand
                    {
                        ParamsId = paramsId
                    }, _cancellationTokenSource.Token);
                }, _cancellationTokenSource.Token);
            }

            return true;
        }

        public void SetGrowingParams(GrowingParams growingParams)
        {
            _growingParams = growingParams;
        }

        public GrowingParams? GrowingParams => _growingParams;
    }
}
