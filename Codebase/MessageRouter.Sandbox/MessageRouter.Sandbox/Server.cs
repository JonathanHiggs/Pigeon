using System;
using System.Threading;

using MessageRouter.Addresses;
using MessageRouter.NetMQ;
using MessageRouter.NetMQ.Receivers;
using MessageRouter.NetMQ.Senders;

namespace MessageRouter.Sandbox
{
    public class Server
    {
        public static void Run()
        {
            var router =
                Router.Builder("TestServer")
                      .WithTransport<NetMQConfig>()
                      .WithReceiver<INetMQReceiver>(TcpAddress.Wildcard(5555))
                      .WithRequestHandler<TestMessage, TestMessage>(Handler)
                      .BuildAndStart();
            
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
