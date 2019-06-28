using System;

using Apache.NMS;

namespace Pigeon.ActiveMQ
{
    public class ActiveMessageFactory
    {
        public IMessage MessageFromTopic(object topicEvent)
        {
            throw new NotImplementedException();
        }


        public object TopicFromMessage(IMessage message)
        {
            throw new NotImplementedException();
        }
    }
}
