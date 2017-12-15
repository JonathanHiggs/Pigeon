using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MessageRouter.Addresses;
using MessageRouter.Messages;
using MessageRouter.Serialization;
using NetMQ;
using NetMQ.Sockets;

namespace MessageRouter.NetMQ.Publishers
{
    public class NetMQPublisher : INetMQPublisher
    {
        private readonly Dictionary<IAddress, bool> boundStatusByAddress = new Dictionary<IAddress, bool>();
        private readonly PublisherSocket socket;
        private readonly ISerializer<byte[]> serializer;
        private bool isBound = false;


        public ISocketPollable PollableSocket => socket;


        public IEnumerable<IAddress> Addresses => boundStatusByAddress.Keys;


        public NetMQPublisher(PublisherSocket socket, ISerializer<byte[]> serializer)
        {
            this.socket = socket ?? throw new ArgumentNullException(nameof(socket));
            this.serializer = serializer ?? throw new ArgumentNullException(nameof(serializer));
        }


        public void AddAddress(IAddress address)
        {
            if (boundStatusByAddress.ContainsKey(address))
                return;

            boundStatusByAddress.Add(address, false);

            if (isBound)
            {
                socket.Bind(address.ToString());
                boundStatusByAddress[address] = false;
            }
        }


        public void BindAll()
        {
            if (isBound)
                return;

            var addresses = from address in Addresses
                          where !IsAddedAndBound(address)
                          select address;

            foreach (var address in addresses.ToList())
            {
                socket.Bind(address.ToString());
                boundStatusByAddress[address] = true;
            }
        }


        public void Publish(Message message)
        {
            var topicName = message.Body.GetType().FullName;
            var data = serializer.Serialize(message);

            socket.SendMoreFrame(topicName).SendFrame(data);
        }


        public void RemoveAddress(IAddress address)
        {
            if (null == address || !boundStatusByAddress.ContainsKey(address))
                return;

            if (isBound && boundStatusByAddress[address])
            {
                socket.Unbind(address.ToString());
                boundStatusByAddress[address] = false;
            }

            boundStatusByAddress.Remove(address);
        }


        public void UnbindAll()
        {
            if (!isBound)
                return;

            var addresses = from address in Addresses
                            where IsAddedAndBound(address)
                            select address;

            foreach (var address in addresses.ToList())
            {
                socket.Unbind(address.ToString());
                boundStatusByAddress[address] = false;
            }
        }


        private bool IsAddedAndBound(IAddress address)
        {
            return boundStatusByAddress.TryGetValue(address, out var bound) && bound;
        }
    }
}
