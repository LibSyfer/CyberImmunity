using Greenhouse.MessageBus.Messages;

namespace Greenhouse.MessageBus.Abstractions
{
    public interface IMessageBus
    {
        Task SendAsync(string destination, IntegrationMessage message, CancellationToken cancellationToken = default);
    }
}
