using MessageRouter.Addresses;
using MessageRouter.Messages;
using MessageRouter.NetMQ;
using MessageRouter.NetMQ.Receivers;
using MessageRouter.Receivers;
using MessageRouter.Serialization;
using MessageRouter.Server;
using NetMQ;
using NetMQ.Sockets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MessageRouter.Sandbox
{
    public class Server
    {
        public static void Run()
        {
            var router = Router.Builder()
                .WithTransport(new NetMQTransport())
                .WithReceiver<INetMQReceiver>(TcpAddress.Wildcard(5555))
                .WithHandler<TestMessage, TestMessage>(Handler)
                .WithName("TestServer")
                .Build();

            router.Start();
            
            Console.WriteLine("Press enter to stop server");
            Console.ReadLine();

            router.Stop();
        }


        private static TestMessage Handler(TestMessage request)
        {
            Console.WriteLine($"Received {request.Num}");
            Thread.Sleep(3000);
            Console.WriteLine($"Returning {request.Num + 1}");
            return new TestMessage { Num = request.Num + 1 };
        }
    }
}
