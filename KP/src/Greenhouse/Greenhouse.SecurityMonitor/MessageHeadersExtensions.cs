namespace Greenhouse.SecurityMonitor
{
    public static class MessageHeadersExtensions
    {
        public static (string operation, string source, string destination) GetMonitoringHeaders(this IDictionary<string, object?> headers)
        { 

            return ("", "", "");
        }
    }
}
