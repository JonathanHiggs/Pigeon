﻿using System;
using System.Threading.Tasks;
using System.Windows;
using ExampleContracts.Models;
using ExampleContracts.Requests;
using ExampleContracts.Topics;

using Pigeon.Addresses;
using Pigeon.NetMQ;
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
                .RegisterSingleton<MainWindow>()
                .RegisterSingleton<ViewModel>();

            var router =
                UnityBuilder
                    .FromContainer(container)
                    .WithName("ExampleClient")
                    .WithTransport<NetMQTransport>(t =>
                    {
                        t.WithSender(TcpAddress.Localhost(5555))
                            .For<UserConnecting>()
                            .For<UserDisconecting>()
                            .For<ConnectedUsers>()
                            .For<Message>();

                        t.WithSubscriber(TcpAddress.Localhost(5556))
                            .Handles<UserConnected>()
                            .Handles<UserDisconnected>()
                            .Handles<Message>();
                    })
                    .BuildAndStart();

            var app = container.Resolve<App>();
            app.Container = container;
            app.InitializeComponent();
            app.Run();

            router.Stop();
        }
    }
}