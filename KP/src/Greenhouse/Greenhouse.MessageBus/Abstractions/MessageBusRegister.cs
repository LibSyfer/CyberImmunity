namespace Greenhouse.MessageBus.Abstractions;

public class MessageBusRegister
{
    public Dictionary<string, Type> MessageTypes { get; } = [];
}
