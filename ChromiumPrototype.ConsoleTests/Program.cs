using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ChromiumPrototype.Messages;
using ChromiumPrototype.Messages.Extensions;
using ChromiumPrototype.Storage;
using Microsoft.ServiceBus;
using Microsoft.ServiceBus.Messaging;

namespace ChromiumPrototype.ConsoleTests
{
    class Program
    {
        static QueueClient JobsClient;
        static QueueClient CompletedClient;
        static BlobStorage Storage;
        private static ManualResetEvent _completedEvent;
        const string QueueName = "BrowserJobs";
        const string CompletedQueueName = "CompletedJobs";


        static void Main(string[] args)
        {
            _completedEvent = new ManualResetEvent(false);

            Storage = new BlobStorage("htmls");

            string connectionString = "Endpoint=sb://chromiumprototype.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=8AKkOajy1GB0JIc43XMgCmDyhm9JEIqv/lEE3LHH26k=";
            var namespaceManager = NamespaceManager.CreateFromConnectionString(connectionString);
            if (!namespaceManager.QueueExists(QueueName))
            {
                namespaceManager.CreateQueue(QueueName);
            }

            if (!namespaceManager.QueueExists(CompletedQueueName))
            {
                namespaceManager.CreateQueue(CompletedQueueName);
            }

            // Initialize the connection to Service Bus Queue
            JobsClient = QueueClient.CreateFromConnectionString(connectionString, QueueName);
            CompletedClient = QueueClient.CreateFromConnectionString(connectionString, CompletedQueueName);

            CompletedClient.OnMessage(Callback);

            var message = new GoToUrlMessage()
            {
                SessionId = Guid.NewGuid(),
                Url = "http://google.com"
            };

            JobsClient.Send(message.ToBrokeredMessage());
            
            _completedEvent.WaitOne();
        }

        private static void Callback(BrokeredMessage brokeredMessage)
        {
            var message = brokeredMessage.GetBody<ContentReadyMessage>();
            Console.WriteLine("Received completed job {0}", message.SessionId);
            Console.WriteLine("Reading file {0}", message.FileName);
            var conteudo = Storage.Read(message.FileName);
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.WriteLine();
            Console.Write(conteudo);
            Console.WriteLine();
            Console.ReadKey();
            _completedEvent.Set();
        }
    }
}
