using Greenhouse.Coordinator.Models;
using Greenhouse.MessageBus.Abstractions;
using Greenhouse.MessageBus.Messages;

namespace Greenhouse.CoordinatorModule.Services
{
    public class CoordinatorService
    {
        private readonly ILogger<CoordinatorService> _logger;
        private bool _isBuzy;
        private Task? _currentGrowingTask;
        private CancellationTokenSource? _cancellationTokenSource;
        private readonly object _lock = new();

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
                    _logger.LogInformation("Start growing");

                    _logger.LogInformation("Getting growing params.");
                    await _messageBus.SendAsync("DatabaseNetModule", new GetGrowingParams
                    {
                        ParamsId = paramsId
                    });
                });
            }

            return true;
        }
    }
}
