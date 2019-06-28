using Apache.NMS;

using Pigeon.Topics;

namespace Pigeon.ActiveMQ
{
    public class ActiveMQQueueSubscriber : ActiveMQSubscriber
    {
        public ActiveMQQueueSubscriber(SubjectMapper subjectMapper, ActiveMessageFactory messageFactory, ITopicDispatcher topicDispatcher) 
            : base(subjectMapper, messageFactory, topicDispatcher)
        { }


        protected override IDestination MakeDestination(string topic) =>
            session.GetQueue(topic);
    }
}
