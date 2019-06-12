using System.Windows;

using ExampleContracts;

using Pigeon.Addresses;
using Pigeon.NetMQ;
using Pigeon.Unity;

using Unity;

namespace ExampleClient
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private readonly UnityContainer container = new UnityContainer();


        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
        }

        private void Application_Startup(object sender, StartupEventArgs e)
        {
            var router =
                UnityBuilder
                    .FromContainer(container)
                    .WithName("ExampleClient")
                    .WithTransport<NetMQTransport>(t =>
                    {
                        t.WithSender(TcpAddress.Localhost(5555))
                            .For<Connect>()
                            .For<Disconect>()
                            .For<Message>();

                        t.WithSubscriber(TcpAddress.Localhost(5556))
                            .Handles<UserConnected>()
                            .Handles<UserDisconnected>()
                            .Handles<Message>();
                    })
                    .BuildAndStart();

            MainWindow = container.Resolve<MainWindow>();
            MainWindow.Show();
        }
    }
}
