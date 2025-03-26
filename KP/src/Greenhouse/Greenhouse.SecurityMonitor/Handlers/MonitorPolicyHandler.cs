using Greenhouse.MessageBus.Abstractions;
using Greenhouse.MessageBus.RabbitMQ.Extensions;

namespace Greenhouse.SecurityMonitor.Handlers
{
    public class MonitorPolicyHandler : IMonitorMessageHandler
    {
        private readonly ILogger<MonitorPolicyHandler> _logger;

        public MonitorPolicyHandler(ILogger<MonitorPolicyHandler> logger)
        {
            _logger = logger;
        }

        public Task Handle(IDictionary<string, object?> metadata, ReadOnlyMemory<byte> payload, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation($"Handling message metadata: {metadata}");

            var monitorHeaders = metadata.ReadMonitorHeaders();

            var authorizeAction = false;

            if (monitorHeaders.ActionName.Equals("Test", StringComparison.OrdinalIgnoreCase) 
                && monitorHeaders.Source.Equals("Coordinator", StringComparison.OrdinalIgnoreCase)
                && monitorHeaders.Destination.Equals("Coordinator", StringComparison.OrdinalIgnoreCase))
                authorizeAction = true;

            if (authorizeAction)
            {
                _logger.LogInformation($"[Action: {monitorHeaders.ActionName}] [Source: {monitorHeaders.Source}] [Destination: {monitorHeaders.Destination}] allowed");
            }
            else
            {
                _logger.LogWarning($"[Action: {monitorHeaders.ActionName}] [Source: {monitorHeaders.Source}] [Destination: {monitorHeaders.Destination}] rejected");
            }

            return Task.CompletedTask;
        }
    }
}
