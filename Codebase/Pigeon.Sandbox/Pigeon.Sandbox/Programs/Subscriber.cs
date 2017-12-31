using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pigeon.Addresses;
using Pigeon.NetMQ;
using Pigeon.NetMQ.Subscribers;
using Pigeon.Sandbox.Contracts;
using Pigeon.Unity;
using Unity;

namespace Pigeon.Sandbox.Programs
{
    public class Subscriber
    {
        private readonly Router router;
        private IDisposable subscription;
        private int received = 0;

        public Subscriber()
        {
            router = UnityBuilder.WithName("Subscriber")
                                 .WithTransport<NetMQConfig>()
                                 .WithSubscriber<INetMQSubscriber, Observation>(TcpAddress.Localhost(5556))
                                 .WithAsyncTopicHandler<Observation>(AsyncHandler)
                                 .Build();
        }


        public void RunSubscriber()
        {
            Console.WriteLine("Press enter to stop the subscriber");
            router.Start();
            subscription = router.Subscribe<Observation>();

            Console.ReadLine();
            subscription.Dispose();
            router.Stop();

            Console.WriteLine($"{received} observations received");
        }


        public static void Run()
        {
            var subscriber = new Subscriber();
            subscriber.RunSubscriber();
        }


        private void Handler(Observation observation)
        {
            received += 1;

            if (received % 1000 == 0)
                Console.WriteLine($"Received: {observation}");
        }


        private async Task AsyncHandler(Observation observation)
        {
            received += 1;

            await Task.Delay(TimeSpan.FromMilliseconds(3));
            
            if (received % 1000 == 0)
                Console.WriteLine($"Received: {observation}");
        }
    }
}
