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

        var actionNameObj = headers[nameof(MonitorHeaders.ActionName)];
        if (actionNameObj is not null) {
            monitorHeaders.ActionName = (string)actionNameObj;
        }

        var sourceObj = headers[nameof(MonitorHeaders.Source)];
        if (sourceObj is not null) {
            monitorHeaders.Source = (string)sourceObj;
        }

        var destinationObj = headers[nameof(MonitorHeaders.Destination)];
        if (destinationObj is not null) {
            monitorHeaders.Destination = (string)destinationObj;
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
