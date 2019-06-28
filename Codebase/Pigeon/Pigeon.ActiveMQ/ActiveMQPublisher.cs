using System;
using System.Collections.Generic;

using Apache.NMS;

using Pigeon.Publishers;

namespace Pigeon.ActiveMQ
{
    public abstract class ActiveMQPublisher : ActiveMQConnection, IPublisher, IDisposable
    {
        private SubjectMapper subjectMapper;
        private ActiveMessageFactory messageFactory;

        private readonly Dictionary<string, ActiveProducer> producers = new Dictionary<string, ActiveProducer>();


        protected ActiveMQPublisher(SubjectMapper subjectMapper, ActiveMessageFactory messageFactory)
        {
            this.subjectMapper = subjectMapper ?? throw new ArgumentNullException(nameof(subjectMapper));
            this.messageFactory = messageFactory ?? throw new ArgumentNullException(nameof(messageFactory));
        }


        public void Publish(object topicEvent)
        {
            var topic = subjectMapper.GetTopicName(topicEvent);

            if (!producers.TryGetValue(topic, out var producer))
            {
                var destination = MakeDestination(topic);
                var messageProducer = session.CreateProducer(destination);
                producer = new ActiveProducer(topic, destination, messageProducer);
                producers.Add(topic, producer);
            }

            producer.Producer.DeliveryMode = MsgDeliveryMode.NonPersistent;
            producer.Producer.Send(messageFactory.MessageFromTopic(topicEvent));
        }


        protected abstract IDestination MakeDestination(string topic);


        #region IDisposable Support

        private bool disposedValue = false; // To detect redundant calls


        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (!disposedValue)
            {
                if (disposing)
                {
                    foreach (var producer in producers.Values)
                    {
                        producer.Producer.Close();
                        producer.Producer.Dispose();
                        producer.Destination.Dispose();
                    }

                    producers.Clear();
                }

                disposedValue = true;
            }
        }

        #endregion
    }
}
