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

namespace MessageRouter.NetMQ.Receivers
{
    /// <summary>
    /// Implementation of <see cref="IReceiver"/> that wraps a NetMQ <see cref="RouterSocket"/>. Encapsulates a connection
    /// that is able to bind to an <see cref="IAddress"/> to receive and respond to incoming <see cref="Message"/>es from
    /// connected remote <see cref="INetMQReceiver"/>s
    /// </summary>
    public class NetMQReceiver : INetMQReceiver, IDisposable
    {
        private readonly Dictionary<IAddress, bool> boundStatusByAddress = new Dictionary<IAddress, bool>();
        private readonly ISerializer<byte[]> serializer;
        private RouterSocket socket;
        private bool isBound = false;


        /// <summary>
        /// Gets an enumerable of <see cref="IAddress"/> that the receiver is listening to
        /// </summary>
        public IEnumerable<IAddress> Addresses => boundStatusByAddress.Keys;


        /// <summary>
        /// Gets a bool status flag indicating whether the receiver is bound to its <see cref="IAddress"/> and is listening to incoming messages
        /// </summary>
        public bool IsBound => isBound;


        /// <summary>
        /// Gets the inner pollable socket
        /// </summary>
        public ISocketPollable PollableSocket => socket;


        /// <summary>
        /// Raised when an incoming message is received
        /// </summary>
        public event RequestTaskHandler RequestReceived;


        /// <summary>
        /// Initializes a new instance of <see cref="NetMQReceiver"/>
        /// </summary>
        /// <param name="socket">Inner NetMQ <see cref="RouterSocket"/></param>
        /// <param name="serializer">A serializer that will convert request and response messages to a binary format for transport along the wire</param>
        public NetMQReceiver(RouterSocket socket, ISerializer<byte[]> serializer)
        {
            this.socket = socket ?? throw new ArgumentNullException(nameof(socket));
            this.serializer = serializer ?? throw new ArgumentNullException(nameof(socket));

            socket.ReceiveReady += OnRequestReceived;
        }


        /// <summary>
        /// Adds an <see cref="IAddress"/> the receiver will listening to incoming <see cref="Message"/>s on
        /// </summary>
        /// <param name="address"></param>
        public void AddAddress(IAddress address)
        {
            if (null == address)
                throw new ArgumentNullException(nameof(address));

            if (boundStatusByAddress.ContainsKey(address))
                return;

            boundStatusByAddress.Add(address, false);

            if (isBound)
            {
                socket.Bind(address.ToString());
                boundStatusByAddress[address] = true;
            }
        }


        /// <summary>
        /// Removes all <see cref="IAddress"/>es the receiver will listen for incoming <see cref="Message"/>s on
        /// </summary>
        public void RemoveAllAddresses()
        {
            if (isBound)
                UnbindAll();

            boundStatusByAddress.Clear();
        }


        /// <summary>
        /// Removes an <see cref="IAddress"/> the receiver will listen for incoming <see cref="Message"/>s on
        /// </summary>
        /// <param name="address"></param>
        public void RemoveAddress(IAddress address)
        {
            if (null == address || !boundStatusByAddress.ContainsKey(address))
                return;

            if (isBound && !boundStatusByAddress[address])
                Unbind(address);

            boundStatusByAddress.Remove(address);
        }


        /// <summary>
        /// Starts the receiver listening for incoming <see cref="Message"/>s  on all added <see cref="IAddress"/>es
        /// </summary>
        public void BindAll()
        {
            if (isBound)
                return;

            foreach (var address in (from address in Addresses
                                    where !IsAddedAndBound(address)
                                    select address).ToList())
            {
                socket.Bind(address.ToString());
                boundStatusByAddress[address] = true;
            }

            isBound = true;
        }


        /// <summary>
        /// Stops the receiver listening for incoming <see cref="Message"/>s on all added <see cref="IAddress"/>es
        /// </summary>
        public void UnbindAll()
        {
            if (!isBound)
                return;

            foreach (var address in (from address in Addresses
                                    where IsAddedAndBound(address)
                                    select address).ToList())
            {
                socket.Unbind(address.ToString());
                boundStatusByAddress[address] = false;
            }

            isBound = false;
        }


        /// <summary>
        /// Synchronously trys receiving a <see cref="RequestTask"/> from a connected <see cref="ISender"/>
        /// </summary>
        /// <param name="requestTask">Combination of the request <see cref="Message"/> and a response Action</param>
        /// <returns>Boolean flag indicating whether a request task was retrieved</returns>
        public bool TryReceive(out RequestTask requestTask)
        {
            NetMQMessage message = default(NetMQMessage);
            if (!socket.TryReceiveMultipartMessage(ref message))
            {
                requestTask = default(RequestTask);
                return false;
            }

            requestTask = Handle(message);
            return true;
        }


        /// <summary>
        /// Synchronously retrieves a <see cref="RequestTask"/> from a connected <see cref="ISender"/>
        /// </summary>
        /// <returns>Combination of the request <see cref="Message"/> and a response Action</returns>
        public RequestTask Receive()
        {
            var requestMessage = socket.ReceiveMultipartMessage();
            return Handle(requestMessage);
        }


        private void OnRequestReceived(object sender, NetMQSocketEventArgs e)
        {
            RequestReceived?.Invoke(this, Receive());
        }


        #region Message Builders
        private RequestTask Handle(NetMQMessage requestMessage)
        {
            var request = ExtractRequest(requestMessage);

            var requestTask = new RequestTask(request, (response) =>
            {
                var message = new NetMQMessage();
                AddAddress(message, requestMessage);
                AddRequestId(message, requestMessage);
                AddResponse(message, response);
                socket.SendMultipartMessage(message);
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
                return serializer.Deserialize<Message>(message[2].ToByteArray());
            else if (message.FrameCount == 5 || message.FrameCount == 6)
                // Asynchronous messages have request in slot 4
                return serializer.Deserialize<Message>(message[4].ToByteArray());
            else
                throw new InvalidOperationException("Request message has unexpected format");
        }


        private NetMQMessage AddResponse(NetMQMessage responseMessage, Message response)
        {
            var data = serializer.Serialize(response);
            responseMessage.Append(data);
            return responseMessage;
        }
        #endregion


        #region Binding Helpers
        private bool IsAddedAndBound(IAddress address)
        {
            return boundStatusByAddress.TryGetValue(address, out var bound) && bound;
        }
        

        private void Unbind(IAddress address)
        {
            if (!isBound)
                return;

            if (!IsAddedAndBound(address))
                return;

            socket.Unbind(address.ToString());
            boundStatusByAddress[address] = false;
        }

        #endregion

        
        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls


        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    UnbindAll();
                    socket.ReceiveReady -= OnRequestReceived;
                    socket.Dispose();
                    socket = null;
                }
                
                disposedValue = true;
            }
        }

        
        public void Dispose()
        {
            Dispose(true);
        }
        #endregion
    }
}
