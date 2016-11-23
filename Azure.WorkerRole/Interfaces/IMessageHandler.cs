using Microsoft.ServiceBus.Messaging;

namespace Azure.WorkerRole.Interfaces
{
    public interface IMessageHandler
    {
        void Handle(BrokeredMessage brokeredMessage, QueueClient queue);
    }
}