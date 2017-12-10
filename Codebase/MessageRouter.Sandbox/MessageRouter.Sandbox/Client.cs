using System;
using System.Threading.Tasks;

using MessageRouter.Addresses;
using MessageRouter.NetMQ;
using MessageRouter.NetMQ.Senders;

namespace MessageRouter.Sandbox
{
    public class Client
    {
        public static void Run()
        {
            Console.Write("Enter server name: ");
            var serverName = Console.ReadLine();

            var router = Router.Builder()
                .WithTransport(new NetMQTransport())
                .WithSenderRouting<INetMQSender, TestMessage>(TcpAddress.FromNameAndPort(serverName, 5555))
                .WithName("TestClient")
                .Build();
            
            router.Start();
            
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

                        var response = await router.Send<TestMessage, TestMessage>(request);
                        Console.WriteLine($"Received: {response.Num}");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex);
                    }
                });
            }
            while (true);

            router.Stop();
        }
    }
}
