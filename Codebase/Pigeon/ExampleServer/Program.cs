using System;
using ExampleContracts;

using Pigeon.Addresses;
using Pigeon.NetMQ;
using Pigeon.Unity;

using Unity;

namespace ExampleServer
{
    class Program
    {
        static void Main(string[] args)
        {
            var container = new UnityContainer();

            container.RegisterSingleton<Server>();

            var router =
                UnityBuilder
                    .FromContainer(container)
                    .WithName("ExampleServer")
                    .WithTransport<NetMQTransport>(t =>
                    {
                        t.WithReceiver(TcpAddress.Localhost(5555));
                        t.WithPublisher(TcpAddress.Localhost(5556));
                    })
                    .WithHandlers(config =>
                        config
                            .WithRequestHandler<Connect, Connected, Server>()
                            .WithRequestHandler<Disconect, Disconnected, Server>()
                            .WithRequestHandler<ExampleContracts.Message, MessagePosted, Server>())
                    .BuildAndStart();

            Console.WriteLine("Press enter to stop server");
            Console.ReadLine();

            router.Stop();
        }
    }
}
