using MessageRouter.Senders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MessageRouter.Addresses;
using MessageRouter.Messages;
using MessageRouter.Serialization;
using NetMQ;

namespace MessageRouter.NetMQ
{
    public class NetMQAsyncSender : IAsyncSender
    {
        private readonly List<IAddress> addresses = new List<IAddress>();
        private readonly ISerializer<byte[]> binarySerializer;
        private readonly AsyncSocket socket;


        public IEnumerable<IAddress> Addresses => addresses;


        public Type SerializerType => typeof(ISerializer<byte[]>);



        public NetMQAsyncSender(AsyncSocket socket, ISerializer<byte[]> binarySerializer)
        {
            this.socket = socket ?? throw new ArgumentNullException(nameof(socket));
            this.binarySerializer = binarySerializer ?? throw new ArgumentNullException(nameof(binarySerializer));
        }


        public void Connect(IAddress address)
        {
            if (!addresses.Contains(address))
            {
                socket.Connect(address);
                addresses.Add(address);
            }
        }


        public void Disconnect(IAddress address)
        {
            if (addresses.Contains(address))
            {
                socket.Disconnect(address);
                addresses.Remove(address);
            }
        }


        public Message SendAndReceive(Message message)
        {
            var requestTask = SendAndReceiveAsync(message);
            requestTask.Wait();
            return requestTask.Result;
        }


        public async Task<Message> SendAndReceiveAsync(Message request)
        {
            var message = new NetMQMessage();
            message.AppendEmptyFrame();
            message.Append(binarySerializer.Serialize<Message>(request));

            var responseMessage = await socket.SendAndReceive(message);

            return binarySerializer.Deserialize<Message>(responseMessage[1].ToByteArray());
        }
    }
}
