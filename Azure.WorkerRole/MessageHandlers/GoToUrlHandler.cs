using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using Azure.WorkerRole.Interfaces;
using CefGlueScreenshot.Offscreen;
using ChromiumPrototype.Messages;
using ChromiumPrototype.Messages.Extensions;
using ChromiumPrototype.Storage;
using Microsoft.ServiceBus.Messaging;

namespace Azure.WorkerRole.MessageHandlers
{
    public class GoToUrlHandler : IMessageHandler
    {
        private readonly ConcurrentDictionary<Guid, OffscreenBrowser> _sessions;
        private readonly BlobStorage _storage;
        ManualResetEvent CompletedEvent = new ManualResetEvent(false);

        public GoToUrlHandler(ConcurrentDictionary<Guid, OffscreenBrowser> sessions, BlobStorage storage)
        {
            _sessions = sessions;
            _storage = storage;
        }

        public void Handle(BrokeredMessage brokeredMessage, QueueClient queue)
        {
            var message = brokeredMessage.GetBody<GoToUrlMessage>();
            var browser = _sessions.GetOrAdd(message.SessionId, new OffscreenBrowser());

            while (browser.GetBrowser() == null)
            {
                Thread.Sleep(50);
            }

            browser.GetBrowser().GetMainFrame().LoadUrl(message.Url);

            var loaded = WaitForBrowserLoadingAsync(browser).Result;

            if(!loaded)
                return;

            var visitor = new SourceVisitor(s =>
            {
                Console.Write(s);

                var fileName = $"{message.SessionId}-{Guid.NewGuid()}";
                _storage.CreateOrUpdate(fileName, s);
                var reply = new ContentReadyMessage()
                {
                    SessionId = message.SessionId,
                    FileName = fileName
                };
                queue.Send(reply.ToBrokeredMessage());
                CompletedEvent.Set();
            });

            browser.GetBrowser().GetMainFrame().GetSource(visitor);
            CompletedEvent.WaitOne();
        }

        private static Task<bool> WaitForBrowserLoadingAsync(OffscreenBrowser browser)
        {
            var tcs = new TaskCompletionSource<bool>();
            LoadingStateChangeDelegate handler = null;
            handler = (cefBrowser, loading, back, forward) => {
                if (loading) return;
                browser.LoadingStateChanged -= handler;
                tcs.TrySetResult(true);
            };
            browser.LoadingStateChanged += handler;
            return tcs.Task;
        }
    }
}