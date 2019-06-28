using Apache.NMS;

namespace Pigeon.ActiveMQ
{
    public class ActiveMQTopicPublisher : ActiveMQPublisher
    {
        public ActiveMQTopicPublisher(SubjectMapper subjectMapper, ActiveMessageFactory messageFactory) 
            : base(subjectMapper, messageFactory)
        { }


        protected override IDestination MakeDestination(string topic) =>
            session.GetTopic(topic);
    }
}
