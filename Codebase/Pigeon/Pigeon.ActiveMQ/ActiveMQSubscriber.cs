using System;
using System.Collections.Generic;
using System.Linq;

using Apache.NMS;

using Pigeon.Subscribers;
using Pigeon.Topics;

namespace Pigeon.ActiveMQ
{
    public abstract class ActiveMQSubscriber : ActiveMQConnection, ISubscriber, IDisposable
    {
        private SubjectMapper subjectMapper;
        private ActiveMessageFactory messageFactory;
        private readonly ITopicDispatcher topicDispatcher;

        private readonly Dictionary<string, ActiveSubscription> subscriptions = new Dictionary<string, ActiveSubscription>();


        public ActiveMQSubscriber(SubjectMapper subjectMapper, ActiveMessageFactory messageFactory, ITopicDispatcher topicDispatcher)
        {
            this.subjectMapper = subjectMapper ?? throw new ArgumentNullException(nameof(subjectMapper));
            this.messageFactory = messageFactory ?? throw new ArgumentNullException(nameof(messageFactory));
            this.topicDispatcher = topicDispatcher ?? throw new ArgumentNullException(nameof(topicDispatcher));
        }


        public void Subscribe<TTopic>() =>
            Subscribe<TTopic>(subjectMapper.GetTopicName<TTopic>());


        public void Subscribe<TTopic>(string subject)
        {
            if (subscriptions.Keys.Contains(subject))
                return;

            var destination = MakeDestination(subject);
            var consumer = session.CreateConsumer(destination);
            var subscription = new ActiveSubscription(subject, destination, consumer);

            subscriptions.Add(subject, subscription);
            consumer.Listener += OnMessageReceived;
        }


        protected abstract IDestination MakeDestination(string topic);


        public void Unsubscribe<TTopic>()
        {
            var topic = subjectMapper.GetTopicName<TTopic>();

            if (!subscriptions.TryGetValue(topic, out var subscription))
                return;

            subscriptions.Remove(topic);
            subscription.Consumer.Listener -= OnMessageReceived;
            subscription.Dispose();
        }


        private void OnMessageReceived(IMessage message)
        {
            var topic = messageFactory.TopicFromMessage(message);
            topicDispatcher.Handle(this, topic);
        }


        #region IDisposable Support

        private bool disposedValue = false; // To detect redundant calls


        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (!disposedValue)
            {
                if (disposing)
                {
                    foreach (var subscription in subscriptions.Values)
                    {
                        subscription.Consumer.Close();
                        subscription.Consumer.Dispose();
                        subscription.Destination.Dispose();
                    }

                    subscriptions.Clear();
                }

                disposedValue = true;
            }
        }

        #endregion
    }
}
