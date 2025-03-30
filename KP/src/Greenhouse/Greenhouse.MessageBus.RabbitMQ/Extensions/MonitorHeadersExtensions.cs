namespace Greenhouse.MessageBus.RabbitMQ.Extensions;

public static class MonitorHeadersExtensions
{

    public static void WriteMonitorHeaders(this IDictionary<string, object?> headers, MonitorHeaders monitorHeaders) {
        headers[nameof(MonitorHeaders.ActionName)] = monitorHeaders.ActionName;
        headers[nameof(MonitorHeaders.Source)] = monitorHeaders.Source;
        headers[nameof(MonitorHeaders.Destination)] = monitorHeaders.Destination;
    }

    public static MonitorHeaders ReadMonitorHeaders(this IDictionary<string, object?> headers)
    {
        var monitorHeaders = new MonitorHeaders();

        if (headers.TryGetValue(nameof(MonitorHeaders.ActionName), out var actionNameObj))
        {
            if (actionNameObj is not null)
            {
                monitorHeaders.ActionName = actionNameObj.ToString() ?? "";
            }
        }

        if (headers.TryGetValue(nameof(MonitorHeaders.Source), out var sourceObj))
        {
            if (sourceObj is not null)
            {
                monitorHeaders.Source = sourceObj.ToString() ?? "";
            }
        }

        if (headers.TryGetValue(nameof(MonitorHeaders.Destination), out var destinationObj))
        {
            if (destinationObj is not null)
            {
                monitorHeaders.Destination = destinationObj.ToString() ?? "";
            }
        }

        return monitorHeaders;
    }

    public class MonitorHeaders
    {
        public string ActionName { get; set; } = string.Empty;
    
        public string Source { get; set; } = string.Empty;
        
        public string Destination { get; set; } = string.Empty;
    }
}
