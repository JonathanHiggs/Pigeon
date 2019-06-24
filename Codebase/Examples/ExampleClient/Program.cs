using System;

using ExampleContracts.Models;
using ExampleContracts.Requests;
using ExampleContracts.Topics;

using Pigeon.Addresses;
using Pigeon.Json;
using Pigeon.NetMQ;
using Pigeon.Serialization;
using Pigeon.Unity;

using Unity;

namespace ExampleClient
{
    public static class Program
    {
        [STAThread]
        public static void Main(string[] args)
        {
            var container = new UnityContainer();

            container
                .RegisterSingleton<UnityContainer>()
                .RegisterSingleton<App>()
                .RegisterSingleton<MessagingService>();
            
            var router =
                UnityBuilder
                    .FromContainer(container)
                    .WithName("ExampleClient")
                    .WithSerializer<DotNetSerializer>()
                    .WithSerializer<JsonSerializer>(true)
                    .WithTransport<NetMQTransport>(t => {
                        t.WithSender(TcpAddress.Localhost(5555))
                            .For<UserConnecting>()
                            .For<UserDisconecting>()
                            .For<ConnectedUsers>()
                            .For<PostMessage>();

                        t.WithSubscriber(TcpAddress.Localhost(5556))
                            .Handles<UserConnected>()
                            .Handles<UserDisconnected>()
                            .Handles<PostedMessage>();
                    })
                    .WithHandlers(c => {
                        c.WithTopicHandler<UserConnected, MessagingService>()
                            .WithTopicHandler<UserDisconnected, MessagingService>()
                            .WithTopicHandler<PostedMessage, MessagingService>();
                    })
                    .BuildAndStart();
            
            using (container.Resolve<MessagingService>())
            {
                var app = container.Resolve<App>();
                app.Container = container;
                app.InitializeComponent();
                app.Run();
            }

            router.Stop();
        }
    }
}
