using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Threading;
using Azure.WorkerRole.MessageHandlers;
using CefGlueScreenshot.Offscreen;
using Microsoft.Azure;
using Microsoft.ServiceBus;
using Microsoft.ServiceBus.Messaging;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.ServiceRuntime;
using Xilium.CefGlue;

namespace Azure.WorkerRole
{
    public class WorkerRole : RoleEntryPoint
    {
        // The name of your queue
        const string QueueName = "BrowserJobs";
        const string CompletedQueueName = "CompletedJobs";

        // QueueClient is thread-safe. Recommended that you cache 
        // rather than recreating it on every request
        QueueClient Client;
        QueueClient CompletedClient;
        ManualResetEvent CompletedEvent = new ManualResetEvent(false);
        

        public override void Run()
        {
            var mainArgs = new CefMainArgs(new string[] { });
            var app = new OffscreenApp();
            CefRuntime.ExecuteProcess(mainArgs, app, IntPtr.Zero);

            // init CEF
            var settings = new CefSettings
            {
                SingleProcess = false,
                MultiThreadedMessageLoop = true,
                LogSeverity = CefLogSeverity.Error,
                LogFile = "CefGlue.log"
            };
            CefRuntime.Initialize(mainArgs, settings, app, IntPtr.Zero);

            Trace.WriteLine("Starting processing of messages");

            // Initiates the message pump and callback is invoked for each message that is received, calling close on the client will stop the pump.
            Client.OnMessage((receivedMessage) =>
                {
                    try
                    {
                        // Process the message
                        Trace.WriteLine("Processing Service Bus message: " + receivedMessage.SequenceNumber.ToString());
                        var handler = MessageHandlerFactory.GetMessageHandler(receivedMessage.ContentType);
                        handler.Handle(receivedMessage, CompletedClient);
                        receivedMessage.Complete();
                    }
                    catch(Exception ex)
                    {
                        // Handle any message processing specific exceptions here
                    }
                });

            CompletedEvent.WaitOne();
        }

        public override bool OnStart()
        {
            // Set the maximum number of concurrent connections 
            ServicePointManager.DefaultConnectionLimit = 12;

            // Create the queue if it does not exist already
            string connectionString = CloudConfigurationManager.GetSetting("Microsoft.ServiceBus.ConnectionString");
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
            Client = QueueClient.CreateFromConnectionString(connectionString, QueueName);
            CompletedClient = QueueClient.CreateFromConnectionString(connectionString,CompletedQueueName);
            return base.OnStart();
        }

        public override void OnStop()
        {
            // Close the connection to Service Bus Queue
            Client.Close();
            CompletedClient.Close();
            CompletedEvent.Set();
            base.OnStop();
        }
    }
}
