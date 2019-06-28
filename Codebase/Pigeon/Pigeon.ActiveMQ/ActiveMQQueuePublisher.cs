using Apache.NMS;

namespace Pigeon.ActiveMQ
{
    public class ActiveMQQueuePublisher : ActiveMQPublisher
    {
        public ActiveMQQueuePublisher(SubjectMapper subjectMapper, ActiveMessageFactory messageFactory) 
            : base(subjectMapper, messageFactory)
        { }


        protected override IDestination MakeDestination(string topic) =>
            session.GetQueue(topic);
    }
}
