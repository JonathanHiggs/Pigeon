using System;

using Apache.NMS;

namespace Pigeon.ActiveMQ
{
    internal struct ActiveSubscription : IDisposable
    {
        public ActiveSubscription(string topic, IDestination destination, IMessageConsumer messageConsumer)
        {
            Topic = topic;
            Destination = destination;
            Consumer = messageConsumer;
        }


        public string Topic { get; }


        public IDestination Destination { get; private set; }


        public IMessageConsumer Consumer { get; private set; }


        public override bool Equals(object obj) => !(obj is null) && obj is ActiveSubscription other && Topic == other.Topic;


        public override int GetHashCode() => Topic.GetHashCode();


        public void Dispose()
        {
            Consumer.Dispose();
            Destination.Dispose();

            Consumer = null;
            Destination = null;
        }
    }
}
