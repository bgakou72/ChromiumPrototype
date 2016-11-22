using System;
using System.Collections.Concurrent;
using System.Threading;
using CefSharp;
using CefSharp.OffScreen;
using ChromiumPrototype.MessageHandlers.Interfaces;
using ChromiumPrototype.Messages;
using ChromiumPrototype.Messages.Extensions;
using ChromiumPrototype.Storage;
using Microsoft.ServiceBus.Messaging;

namespace ChromiumPrototype.MessageHandlers
{
    public class GoToUrlHandler : IMessageHandler
    {
        private readonly ConcurrentDictionary<Guid, ChromiumWebBrowser> _sessions;
        private readonly BlobStorage _storage;

        public GoToUrlHandler(ConcurrentDictionary<Guid, ChromiumWebBrowser> sessions, BlobStorage storage)
        {
            _sessions = sessions;
            _storage = storage;
        }

        public void Handle(BrokeredMessage brokeredMessage, QueueClient queue)
        {
            var message = brokeredMessage.GetBody<GoToUrlMessage>();
            var browser = _sessions.GetOrAdd(message.SessionId, new ChromiumWebBrowser());

            browser.Load(message.Url);

            while (browser.IsLoading)
            {
                Thread.Sleep(100);
            }

            browser.GetSourceAsync()
                .ContinueWith(task =>
                    {
                        var fileName = $"{message.SessionId}-{Guid.NewGuid()}";
                        _storage.CreateOrUpdate(fileName, task.Result);
                        var reply = new ContentReadyMessage()
                        {
                            SessionId = message.SessionId,
                            FileName = fileName
                        };
                        queue.Send(reply.ToBrokeredMessage());
                        brokeredMessage.Complete();
                    });
            
        }
    }
}