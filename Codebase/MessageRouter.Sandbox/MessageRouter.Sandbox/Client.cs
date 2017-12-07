using MessageRouter.Addresses;
using MessageRouter.Client;
using MessageRouter.Messages;
using MessageRouter.NetMQ;
using MessageRouter.NetMQ.Senders;
using MessageRouter.Routing;
using MessageRouter.Senders;
using MessageRouter.Serialization;
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
            Console.Write("Enter server name: ");
            var serverName = Console.ReadLine();
            var address = TcpAddress.FromNameAndPort(serverName, 5555);

            var monitorCache = new MonitorCache();

            var router = new Router();
            var senderCache = new SenderCache(router, monitorCache);


            var serializer = new BinarySerializer();
            var poller = new NetMQPoller();
            var senderMonitor = new NetMQSenderMonitor(poller);
            var senderFactory = new NetMQSenderFactory(senderMonitor, serializer);
            var messageFactory = new MessageFactory();

            var client = new MessageClient(senderCache, monitorCache, messageFactory);


            router.AddSenderRouting<TestMessage, INetMQSender>(address);
            senderCache.AddFactory(senderFactory);
                        
            client.Start();
            
            Console.WriteLine("Press return to send message");
            Console.WriteLine("Enter 'stop' to end");

            var random = new Random();

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

                        var response = await client.Send<TestMessage, TestMessage>(request);
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
