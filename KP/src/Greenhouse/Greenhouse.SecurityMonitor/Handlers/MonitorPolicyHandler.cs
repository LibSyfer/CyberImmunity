using Greenhouse.MessageBus.Abstractions;

namespace Greenhouse.SecurityMonitor.Handlers
{
    public class MonitorPolicyHandler : IMonitorMessageHandler
    {
        private readonly ILogger<MonitorPolicyHandler> _logger;

        public MonitorPolicyHandler(ILogger<MonitorPolicyHandler> logger)
        {
            _logger = logger;
        }

        public Task Handle(IDictionary<string, object?> metadata, object payload, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation($"Handling message metadata: {metadata}");

            return Task.CompletedTask;
        }
    }
}
