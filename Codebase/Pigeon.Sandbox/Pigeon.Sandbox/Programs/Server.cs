using System;
using System.Threading.Tasks;

using Pigeon.Addresses;
using Pigeon.Contrib;
using Pigeon.NetMQ;
using Pigeon.Sandbox.Contracts;
using Pigeon.Unity;

namespace Pigeon.Sandbox.Programs
{
    public class Server
    {
        public static void Run()
        {
            var router = 
                UnityBuilder
                    .Named("TestServer")
                    .WithTransport<NetMQTransport>(t => t.WithReceiver(TcpAddress.Wildcard(5555)))
                    .WithHandlers(h => h.WithAsyncRequestHandler<TestMessage, TestMessage>(Handler))
                    .WithContribHandlers()
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
