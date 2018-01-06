using System;
using System.Threading;
using System.Threading.Tasks;
using Pigeon.Addresses;
using Pigeon.NetMQ;
using Pigeon.NetMQ.Receivers;
using Pigeon.Sandbox.Contracts;
using Pigeon.Unity;
using Unity;

namespace Pigeon.Sandbox.Programs
{
    public class Server
    {
        public static void Run()
        {
            var router = UnityBuilder.Named("TestServer")
                                     .WithTransport<NetMQConfig>()
                                     .WithReceiver<INetMQReceiver>(TcpAddress.Wildcard(5555))
                                     .WithAsyncRequestHandler<TestMessage, TestMessage>(Handler)
                                     .BuildAndStart();
            
            Console.WriteLine("Press enter to stop server");
            Console.ReadLine();

            router.Stop();
        }


        private static async Task<TestMessage> Handler(TestMessage request)
        {
            Console.WriteLine($"Received {request.Num}");
            await Task.Delay(TimeSpan.FromSeconds(1)); // Simulate work
            Console.WriteLine($"Returning {request.Num + 1}");
            return new TestMessage { Num = request.Num + 1 };
        }
    }
}
