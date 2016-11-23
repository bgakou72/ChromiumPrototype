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
        static QueueClient CompletedClient;
        static BlobStorage Storage;
        const string connectionString = "Endpoint=sb://chromiumprototype.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=8AKkOajy1GB0JIc43XMgCmDyhm9JEIqv/lEE3LHH26k=";

        static void Main(string[] args)
        {

            Storage = new BlobStorage("htmls");
            
            CompletedClient = QueueClient.CreateFromConnectionString(connectionString, "CompletedJobs");
            CompletedClient.OnMessage(Callback);
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("Waiting...");

            while (true)
            {
                
                Thread.Sleep(100);
            }
        }

        private static void Callback(BrokeredMessage brokeredMessage)
        {
            var message = brokeredMessage.GetBody<ContentReadyMessage>();

            brokeredMessage.Complete();
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("{0} Received completed job {1}", DateTime.Now, message.SessionId);
            Console.WriteLine("{0} Reading file {1}", DateTime.Now, message.FileName);
            var conteudo = Storage.Read(message.FileName);
            Console.WriteLine("{0} File read! Length: {1} ", DateTime.Now, conteudo.Length);
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("{0} Content: ", DateTime.Now);
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.WriteLine();
            Console.Write(conteudo.Length > 5000 ? conteudo.Substring(0,5000) : conteudo);
            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("Waiting...");

        }
    }
}
