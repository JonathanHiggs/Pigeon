using System;

using Pigeon.Topics;

namespace Pigeon.ActiveMQ
{
    public class ActiveMQFactory
    {
        private readonly SubjectMapper subjectMapper;
        private readonly ITopicDispatcher topicDispatcher;
        private readonly ActiveMessageFactory messageFactory;


        public ActiveMQFactory(ITopicDispatcher topicDispatcher, SubjectMapper subjectMapper, ActiveMessageFactory messageFactory)
        {
            this.subjectMapper = subjectMapper ?? throw new ArgumentNullException(nameof(subjectMapper));
            this.messageFactory = messageFactory ?? throw new ArgumentNullException(nameof(messageFactory));
            this.topicDispatcher = topicDispatcher ?? throw new ArgumentNullException(nameof(topicDispatcher));
        }


        public ActiveMQQueuePublisher CreateQueuePublisher(ActiveMQAddress address)
        {
            var publisher = new ActiveMQQueuePublisher(subjectMapper, messageFactory);
            publisher.AddAddress(address);
            return publisher;
        }


        public ActiveMQTopicPublisher CreateTopicPublisher(ActiveMQAddress address)
        {
            var publisher = new ActiveMQTopicPublisher(subjectMapper, messageFactory);
            publisher.AddAddress(address);
            return publisher;
        }


        public ActiveMQQueueSubscriber CreateQueueSubscriber(ActiveMQAddress address)
        {
            var subscriber = new ActiveMQQueueSubscriber(subjectMapper, messageFactory, topicDispatcher);
            subscriber.AddAddress(address);
            return subscriber;
        }


        public ActiveMQTopicSubscriber CreateTopicSubscriber(ActiveMQAddress address)
        {
            var subscriber = new ActiveMQTopicSubscriber(subjectMapper, messageFactory, topicDispatcher);
            subscriber.AddAddress(address);
            return subscriber;
        }
    }
}
