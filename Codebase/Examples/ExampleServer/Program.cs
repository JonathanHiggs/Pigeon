using System;

using ExampleContracts.Models;
using ExampleContracts.Requests;
using ExampleContracts.Responses;

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

            container.RegisterSingleton<UserMessageService>();

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
                            .WithRequestHandler<UserConnecting, Response<User>, UserMessageService>()
                            .WithRequestHandler<UserDisconecting, Response<User>, UserMessageService>()
                            .WithRequestHandler<ConnectedUsers, ConnectedUsersList, UserMessageService>()
                            .WithRequestHandler<PostMessage, Response<Message>, UserMessageService>())
                    .BuildAndStart();

            Console.Title = "Chat Server";
            Console.WriteLine("Press enter to stop server");

            var server = container.Resolve<UserMessageService>();
            var running = true;

            while (running)
            {
                var input = Console.ReadLine();

                switch(input)
                {
                    case "reset":
                        server.Reset();
                        break;

                    case "exit":
                        running = false;
                        break;
                }
            }

            router.Stop();
        }
    }
}
