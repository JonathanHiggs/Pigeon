using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MessageRouter.Messages;
using MessageRouter.Serialization;
using NetMQ.Sockets;
using NetMQ;

namespace MessageRouter.NetMQ
{
    public class NetMQSender : ISender
    {
        private readonly ICollection<IAddress> addresses = new List<IAddress>();
        private readonly ISerializer<byte[]> binarySerializer;
        private readonly DealerSocket dealerSocket;


        public IEnumerable<IAddress> Addresses => addresses;

        public Type SerializerType => binarySerializer.GetType();


        public NetMQSender(DealerSocket dealerSocket, ISerializer<byte[]> binarySerializer)
        {
            this.dealerSocket = dealerSocket ?? throw new ArgumentNullException(nameof(dealerSocket));
            this.binarySerializer = binarySerializer ?? throw new ArgumentNullException(nameof(binarySerializer));
        }


        public void Connect(IAddress address)
        {
            if (!addresses.Contains(address))
            {
                dealerSocket.Connect(address.ToString());
                addresses.Add(address);
            }
        }


        public void Disconnect(IAddress address)
        {
            if (addresses.Contains(address))
            {
                dealerSocket.Unbind(address.ToString());
                addresses.Remove(address);
            }
        }


        public Message Send(Message message)
        {
            var requestMessage = new NetMQMessage();
            requestMessage.AppendEmptyFrame();
            requestMessage.Append(binarySerializer.Serialize<Message>(message));

            dealerSocket.SendMultipartMessage(requestMessage);

            var responseMessage = dealerSocket.ReceiveMultipartMessage();

            if (null == responseMessage || responseMessage.FrameCount == 0)
                throw new InvalidOperationException("Response message is in an incorrect format");

            var returnData = responseMessage[1].ToByteArray();
            return binarySerializer.Deserialize<Message>(returnData);
        }
    }
}
