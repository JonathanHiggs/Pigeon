using Apache.NMS;

using Pigeon.Topics;

namespace Pigeon.ActiveMQ
{
    public class ActiveMQTopicSubscriber : ActiveMQSubscriber
    {
        public ActiveMQTopicSubscriber(SubjectMapper subjectMapper, ActiveMessageFactory messageFactory, ITopicDispatcher topicDispatcher) 
            : base(subjectMapper, messageFactory, topicDispatcher)
        { }


        protected override IDestination MakeDestination(string topic) =>
            session.GetTopic(topic);
    }
}
