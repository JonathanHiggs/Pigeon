using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MessageRouter.Packages;
using MessageRouter.Serialization;
using NetMQ;

namespace MessageRouter.NetMQ
{
    public class MessageFactory : IMessageFactory
    {
        private readonly ISerializer serializer;


        /// <summary>
        /// Initializes a new instance of <see cref="MessageFactory"/>
        /// </summary>
        /// <param name="serializer">A serializer that will convert data into a binary format for transmission</param>
        public MessageFactory(ISerializer serializer)
        {
            this.serializer = serializer ?? throw new ArgumentNullException(nameof(serializer));
        }


        public NetMQMessage CreateTopicMessage(Package package)
        {
            var message = new NetMQMessage(2);
            message.Append(package.Body.GetType().FullName);
            message.Append(serializer.Serialize(package));
            return message;
        }


        public Package ExtractTopicPackage(NetMQMessage message)
        {
            return serializer.Deserialize<Package>(message[1].ToByteArray());
        }


        public NetMQMessage CreateRequestMessage(Package package)
        {
            var message = new NetMQMessage();
            message.AppendEmptyFrame();
            message.Append(serializer.Serialize(package));
            return message;
        }


        public Package ExtractResponsePackage(NetMQMessage message)
        {
            return serializer.Deserialize<Package>(message[1].ToByteArray());
        }


        public Package ExtractRequestPackage(NetMQMessage message)
        {
            return serializer.Deserialize<Package>(message[4].ToByteArray());
        }


        public NetMQMessage CreateResponseMessage(Package package, NetMQMessage requestMessage)
        {
            var message = new NetMQMessage(5);
            message.Append(requestMessage[0]);
            message.AppendEmptyFrame();
            message.Append(requestMessage[2]);
            message.AppendEmptyFrame();
            message.Append(serializer.Serialize(package));
            return message;
        }


        public bool ValidRequestMessage(NetMQMessage requestMessage)
        {
            return null != requestMessage && requestMessage.FrameCount == 5;
        }
    }
}
