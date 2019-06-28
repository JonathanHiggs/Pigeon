using System.Collections.Generic;

using Pigeon.Publishers;
using Pigeon.Subscribers;

namespace Pigeon.ActiveMQ
{
    public class ActiveMQMonitor : IPublisherMonitor<ActiveMQPublisher>, ISubscriberMonitor<ActiveMQSubscriber>
    {
        private readonly HashSet<ActiveMQPublisher> publishers = new HashSet<ActiveMQPublisher>();
        private readonly HashSet<ActiveMQSubscriber> subscribers = new HashSet<ActiveMQSubscriber>();
        private readonly object lockObj = new object();

        private bool running = false;


        public void AddPublisher(ActiveMQPublisher publisher) =>
            Add(publisher, publishers);


        public void AddSubscriber(ActiveMQSubscriber subscriber) =>
            Add(subscriber, subscribers);


        public void StartMonitoring()
        {
            lock (lockObj)
            {
                if (running)
                    return;

                foreach (var publisher in publishers)
                    publisher.InitializeConnection();

                foreach (var subscriber in subscribers)
                    subscriber.InitializeConnection();

                running = true;
            }
        }


        public void StopMonitoring()
        {
            lock (lockObj)
            {
                if (!running)
                    return;

                foreach (var publisher in publishers)
                    publisher.TerminateConnection();

                foreach (var subscriber in subscribers)
                    subscriber.TerminateConnection();

                running = false;
            }
        }


        private void Add<TConnection>(TConnection connection, HashSet<TConnection> connectionSet)
            where TConnection : ActiveMQConnection
        {
            if (connection is null)
                return;

            connectionSet.Add(connection);

            lock (lockObj)
            {
                if (running)
                    connection.InitializeConnection();
            }
        }
    }
}
