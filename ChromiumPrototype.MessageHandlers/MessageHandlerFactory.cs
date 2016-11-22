using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CefSharp.OffScreen;
using ChromiumPrototype.MessageHandlers.Interfaces;
using ChromiumPrototype.Messages;
using ChromiumPrototype.Messages.Extensions;
using ChromiumPrototype.Storage;

namespace ChromiumPrototype.MessageHandlers
{
    public static class MessageHandlerFactory
    {
        private static Dictionary<string, IMessageHandler> _Handlers;
        private static ConcurrentDictionary<Guid, ChromiumWebBrowser> _Sessions;
        private static BlobStorage _Storage;

        static MessageHandlerFactory()
        {
            _Storage = new BlobStorage("htmls");
            _Sessions = new ConcurrentDictionary<Guid, ChromiumWebBrowser>();
            _Handlers = new Dictionary<string, IMessageHandler>();
            _Handlers.Add(typeof(GoToUrlMessage).GetMessageType(), new GoToUrlHandler(_Sessions, _Storage));
            
        }

        public static IMessageHandler GetMessageHandler(string messageType)
        {
            return _Handlers[messageType];
        }
    }
}
