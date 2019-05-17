using System;
using System.Linq;
using System.Threading.Tasks;

using Pigeon.Addresses;
using Pigeon.Contrib.Meta.Describe.v1_0;
using Pigeon.NetMQ;
using Pigeon.Sandbox.Contracts;
using Pigeon.Unity;

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


        public async Task Start()
        {
            Console.WriteLine("Press return to send message");
            Console.WriteLine("Enter 'stop' to end");
            
            do
            {
                var line = Console.ReadLine();

                if (line == "meta")
                    DisplayMeta(await router.Send<DescribeRouter, RouterDescription>(new DescribeRouter()));
                else if (line == "stop")
                    break;
                else
                    await SendRequest();
            }
            while (true);

            router.Stop();
        }


        public static void Run()
        {
            Console.Write("Enter server name: ");
            var serverName = Console.ReadLine();

            var router = UnityBuilder.Named("TestClient")
                                     .WithTransport<NetMQTransport>(t => 
                                     {
                                         t.WithSender(TcpAddress.FromNameAndPort(serverName, 5555))
                                            .For<TestMessage>()
                                            .For<DescribeRouter>();
                                     })
                                     .BuildAndStart();

            var client = new Client(router);
            client.Start().GetAwaiter().GetResult();
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


        public void DisplayMeta(RouterDescription description)
        {
            Console.WriteLine($"Id:        {description.Identity.Id}");
            Console.WriteLine($"Name:      {description.Identity.Name}");
            Console.WriteLine($"Host:      {description.Identity.Host}");
            Console.WriteLine($"Started:   {description.RuntimeInfo.StartedTimestamp.Value}");

            if (description.Receivers.Any())
            {
                Console.WriteLine("Receivers:");
                foreach (var receiver in description.Receivers)
                    Console.WriteLine($"\t{receiver.Type.Name}\t : {String.Join(", ", receiver.Addresses.Select(a => a.ToString()))}");
            }

            if (description.Publishers.Any())
            {
                Console.WriteLine("Publishers:");
                foreach (var publisher in description.Publishers)
                    Console.WriteLine($"\t{publisher.Type.Name}");
            }
        }
    }
}
