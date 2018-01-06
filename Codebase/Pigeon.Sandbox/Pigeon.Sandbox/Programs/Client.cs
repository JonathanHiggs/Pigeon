﻿using System;
using System.Threading.Tasks;

using Pigeon.Addresses;
using Pigeon.NetMQ;
using Pigeon.NetMQ.Senders;
using Pigeon.Sandbox.Contracts;
using Pigeon.Unity;
using Microsoft.Practices.Unity;
using Unity;

namespace Pigeon.Sandbox.Programs
{
    public class Client
    {
        private Random random = new Random();
        private Router router;


        public Client(Router router)
        {
            this.router = router ?? throw new ArgumentNullException(nameof(router));
        }


        public void Start()
        {
            Console.WriteLine("Press return to send message");
            Console.WriteLine("Enter 'stop' to end");
            
            do
            {
                var line = Console.ReadLine();
                if (line == "stop")
                    break;

                Task.Run(async () => await SendRequest());
            }
            while (true);

            router.Stop();
        }


        public static void Run()
        {
            Console.Write("Enter server name: ");
            var serverName = Console.ReadLine();

            var router = UnityBuilder.Named("TestClient")
                                     .WithTransport<NetMQConfig>()
                                     .WithSenderRouting<INetMQSender, TestMessage>(TcpAddress.FromNameAndPort(serverName, 5555))
                                     .BuildAndStart();

            var client = new Client(router);
            client.Start();
        }


        public async Task SendRequest()
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
        }
    }
}
