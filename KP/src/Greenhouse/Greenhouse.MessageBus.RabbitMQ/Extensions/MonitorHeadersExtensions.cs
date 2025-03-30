using System.Text;

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

        if (headers.TryGetValue(nameof(MonitorHeaders.ActionName), out var actionNameObj) && actionNameObj is byte[] actionNameByteArray)
        {
            monitorHeaders.ActionName = Encoding.UTF8.GetString(actionNameByteArray);
        }

        if (headers.TryGetValue(nameof(MonitorHeaders.Source), out var sourceObj) && sourceObj is byte[] sourceByteArray)
        {
            monitorHeaders.Source = Encoding.UTF8.GetString(sourceByteArray);
        }

        if (headers.TryGetValue(nameof(MonitorHeaders.Destination), out var destinationObj) && destinationObj is byte[] destinationByteArray)
        {
            monitorHeaders.Destination = Encoding.UTF8.GetString(destinationByteArray);
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
