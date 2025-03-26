using Greenhouse.MessageBus.Events;

namespace Greenhouse.MessageBus.Abstractions
{
    public interface IMessageBus
    {
        Task SendAsync(string destination, IntegrationMessage message, CancellationToken cancellationToken = default);
    }
}
