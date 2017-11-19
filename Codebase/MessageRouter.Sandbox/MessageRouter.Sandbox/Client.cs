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
    public class Client
    {
        public static void Run()
        {
            var random = new Random();
            var poller = new NetMQPoller();
            var senderFactory = new NetMQSenderFactory(poller);
            var senderManager = new SenderManager(senderFactory);
            var messageFactory = new MessageFactory();
            var client = new MessageClient(senderManager, messageFactory);

            Console.WriteLine("Enter server name: ");
            var serverName = Console.ReadLine();

            senderManager.AddRequestMapping<TestMessage>(TcpAddress.Client.Named(serverName, 5555));

            while (true)
            {
                var request = new TestMessage { Num = random.Next(100) };
                Console.WriteLine($"Sending: {request.Num}");
                var response = client.Send<TestMessage, TestMessage>(request);

                Console.WriteLine($"Received: {response.Num}");
                Console.ReadLine();
            }
        }
    }
}
