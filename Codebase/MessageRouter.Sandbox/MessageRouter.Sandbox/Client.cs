using MessageRouter.Addresses;
using MessageRouter.Client;
using MessageRouter.Messages;
using MessageRouter.NetMQ;
using MessageRouter.NetMQ.Senders;
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
            var senderMonitor = new NetMQSenderMonitor(poller);
            var senderFactory = new NetMQSenderFactory(senderMonitor);
            var senderRouter = new SenderRouter();
            var messageFactory = new MessageFactory();
            var client = new MessageClient(senderRouter, messageFactory);

            Console.WriteLine("Enter server name: ");
            var serverName = Console.ReadLine();
            var address = TcpAddress.FromNameAndPort(serverName, 5555);

            senderRouter
                .AddFactory<NetMQSender>(senderFactory)
                .AddSender<NetMQSender>(address)
                .AddRequestMapping<TestMessage>(address);
            
            client.Start();
            
            Console.WriteLine("Press return to send message");
            Console.WriteLine("Enter 'stop' to end");

            do
            {
                var line = Console.ReadLine();
                if (line == "stop")
                    break;
                
                Task.Factory.StartNew(async () =>
                {
                    try
                    {
                        var request = new TestMessage { Num = random.Next(100) };
                        Console.WriteLine($"Sending: {request.Num}");

                        var response = await client.SendAsync<TestMessage, TestMessage>(request);
                        Console.WriteLine($"Received: {response.Num}");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex);
                    }
                });
            }
            while (true);

            client.Stop();
        }
    }
}
