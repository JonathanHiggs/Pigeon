using System;

using Apache.NMS;

using Pigeon.Serialization;

namespace Pigeon.ActiveMQ
{
    /// <summary>
    /// Creates and extracts <see cref="IMessage"/>s
    /// </summary>
    public class ActiveMessageFactory
    {
        private ISerializerCache serializerCache;


        /// <summary>
        /// Initializes a new instance of <see cref="ActiveMessageFactory"/>
        /// </summary>
        /// <param name="serializerCache"><see cref="ISerializerCache"/> for accessing <see cref="ISerializer"/>s to encode and decode binary messages</param>
        public ActiveMessageFactory(ISerializerCache serializerCache)
        {
            this.serializerCache = serializerCache ?? throw new ArgumentNullException(nameof(serializerCache));
        }


        public IMessage MessageFromTopic(ISession session, object topicEvent)
        {
            return session.CreateObjectMessage(topicEvent);
        }


        public object TopicFromMessage(IMessage message)
        {
            message.
        }
    }
}
