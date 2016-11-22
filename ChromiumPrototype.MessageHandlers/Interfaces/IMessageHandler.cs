using System;
using System.Collections.Concurrent;
using CefSharp.OffScreen;
using Microsoft.ServiceBus.Messaging;

namespace ChromiumPrototype.MessageHandlers.Interfaces
{
    public interface IMessageHandler
    {
        void Handle(BrokeredMessage brokeredMessage, QueueClient queue);
    }
}