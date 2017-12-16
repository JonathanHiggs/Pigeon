using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using MessageRouter.Addresses;
using MessageRouter.NetMQ;
using MessageRouter.NetMQ.Publishers;
using MessageRouter.Sandbox.Contracts;

namespace MessageRouter.Sandbox.Programs
{
    public class Publisher
    {
        private readonly Router router;
        private readonly Timer timer;

        private Random random = new Random();
        private double price = 100.0;
        private double drift = 0.00001;
        private double vol = 0.05;
        private int sent = 0;


        public Publisher()
        {
            router = Router.Builder("Publisher")
                           .WithTransport<NetMQConfig>()
                           .WithPublisher<INetMQPublisher>(TcpAddress.Wildcard(5556))
                           .Build();

            timer = new Timer
            {
                AutoReset = true,
                Interval = 1.0,
            };

            timer.Elapsed += Publish;
        }

        private void Publish(object sender, ElapsedEventArgs e)
        {
            price = price + drift + (0.5 - random.NextDouble()) * vol;
            var observation = new Observation("AAPL", price);
            router.Publish(observation);
            sent += 1;

            if (sent % 1000 == 0)
                Console.WriteLine($"Publishing: {observation}");
        }

        public void Start()
        {
            router.Start();
            timer.Enabled = true;

            Console.WriteLine("Press enter to stop the publisher");
            Console.ReadLine();

            timer.Enabled = false;
            router.Stop();

            Console.WriteLine($"{sent} Sent");
        }


        public static void Run()
        {
            var publisher = new Publisher();
            publisher.Start();
        }
    }
}
