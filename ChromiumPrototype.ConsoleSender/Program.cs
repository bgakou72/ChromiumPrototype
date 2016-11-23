using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ChromiumPrototype.Messages;
using ChromiumPrototype.Messages.Extensions;
using Microsoft.ServiceBus.Messaging;

namespace ChromiumPrototype.ConsoleSender
{
    class Program
    {
        const string connectionString = "Endpoint=sb://chromiumprototype.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=8AKkOajy1GB0JIc43XMgCmDyhm9JEIqv/lEE3LHH26k=";
        static QueueClient JobsClient;


        static void Main(string[] args)
        {
            JobsClient = QueueClient.CreateFromConnectionString(connectionString, "BrowserJobs");
            var id = Guid.NewGuid();

            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine();
            Console.WriteLine("Enter url to visit:");
            var url = Console.ReadLine(); ;

            while (url != "exit")
            {
                var message = new GoToUrlMessage()
                {
                    SessionId = id,
                    Url = url
                };

                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("{0} Sending request to visit: {0}", DateTime.Now, url);
                JobsClient.Send(message.ToBrokeredMessage());
                Console.WriteLine("{0} Sent!", DateTime.Now);

                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine();
                Console.WriteLine("Enter url to visit:");
                url = Console.ReadLine();
            }

            
        }
    }
}
