using MessageRouter.Addresses;
using MessageRouter.Client;
using MessageRouter.Messages;
using MessageRouter.NetMQ;
using MessageRouter.Senders;
using NetMQ;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessageRouter.Sandbox
{
    public class AsyncClient
    {
        public static void Run()
        {
            var random = new Random();
            var poller = new NetMQPoller();
            var senderFactory = new NetMQSenderFactory();
            var senderManager = new NetMQSenderManager(senderFactory, poller);
            var messageFactory = new MessageFactory();
            var client = new MessageClient(senderManager, messageFactory);

            Console.WriteLine("Enter server name: ");
            var serverName = Console.ReadLine();

            senderManager.AddAsync<TestMessage>(TcpAddress.Client.Named(serverName, 5555));

            poller.RunAsync();

            Console.WriteLine("Press return to send message");
            Console.WriteLine("Enter 'stop' to end");

            do
            {
                var line = Console.ReadLine();
                if (line == "stop")
                    break;
                
                Task.Factory.StartNew(async () =>
                {
                    var request = new TestMessage { Num = random.Next(100) };
                    Console.WriteLine($"Sending: {request.Num}");

                    var response = await client.SendAsync<TestMessage, TestMessage>(request, 99999);
                    Console.WriteLine($"Received: {response.Num}");
                });
            }
            while (true);

            poller.StopAsync();
        }
    }
}
