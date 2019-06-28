using Apache.NMS;

namespace Pigeon.ActiveMQ
{
    internal struct ActiveProducer
    {
        public ActiveProducer(string topic, IDestination destination, IMessageProducer producer)
        {
            Topic = topic;
            Destination = destination;
            Producer = producer;
        }

        public IDestination Destination { get; }

        public IMessageProducer Producer { get; }

        public string Topic { get; }
    }
}
