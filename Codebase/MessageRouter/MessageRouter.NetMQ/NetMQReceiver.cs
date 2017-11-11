using MessageRouter.Addresses;
using MessageRouter.Messages;
using MessageRouter.Receivers;
using MessageRouter.Serialization;
using NetMQ;
using NetMQ.Sockets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessageRouter.NetMQ
{
    public class NetMQReceiver : IReceiver
    {
        private readonly ICollection<IAddress> addresses = new List<IAddress>();
        protected readonly RouterSocket routerSocket;
        private readonly ISerializer<byte[]> binarySerializer;


        public IEnumerable<IAddress> Addresses => addresses;


        public NetMQReceiver(RouterSocket routerSocket, ISerializer<byte[]> binarySerializer)
        {
            this.routerSocket = routerSocket ?? throw new ArgumentNullException(nameof(routerSocket));
            this.binarySerializer = binarySerializer ?? throw new ArgumentNullException(nameof(routerSocket));
        }


        public void Bind(IAddress address)
        {
            if (!addresses.Contains(address))
            {
                routerSocket.Bind(address.ToString());
                addresses.Add(address);
            }
        }


        public void Unbind(IAddress address)
        {
            if (addresses.Contains(address))
            {
                routerSocket.Unbind(address.ToString());
                addresses.Remove(address);
            }
        }


        public RequestTask Receive()
        {
            var requestMessage = routerSocket.ReceiveMultipartMessage();
            
            var request = ExtractRequest(requestMessage);

            var requestTask = new RequestTask(request, (response) =>
            {
                var message = new NetMQMessage();
                AddAddress(message, requestMessage);
                AddRequestId(message, requestMessage);
                AddResponse(message, response);
                routerSocket.SendMultipartMessage(message);
            });

            return requestTask;
        }


        private NetMQMessage AddAddress(NetMQMessage responseMessage, NetMQMessage requestMessage)
        {
            responseMessage.Append(requestMessage[0]);
            responseMessage.AppendEmptyFrame();
            return responseMessage;
        }


        private NetMQMessage AddRequestId(NetMQMessage responseMessage, NetMQMessage requestMessage)
        {
            // Check for non-asynchronous messages
            if (requestMessage.FrameCount != 5)
                return responseMessage;

            // Asynchronous messages have the request id in slot 2
            responseMessage.Append(requestMessage[2]);
            responseMessage.AppendEmptyFrame();
            return responseMessage;
        }


        private Message ExtractRequest(NetMQMessage message)
        {
            if (message.FrameCount == 3 || message.FrameCount == 4)
                // Synchronous messages have request in slot 2
                return binarySerializer.Deserialize<Message>(message[2].ToByteArray());
            else if (message.FrameCount == 5 || message.FrameCount == 6)
                // Asynchronous messages have request in slot 4
                return binarySerializer.Deserialize<Message>(message[4].ToByteArray());
            else
                throw new InvalidOperationException("Request message has unexpected format");
        }


        private NetMQMessage AddResponse(NetMQMessage responseMessage, Message response)
        {
            var data = binarySerializer.Serialize<Message>(response);
            responseMessage.Append(data);
            return responseMessage;
        }
    }
}
