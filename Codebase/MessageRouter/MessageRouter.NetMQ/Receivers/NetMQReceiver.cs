using MessageRouter.Addresses;
using MessageRouter.NetMQ.Common;
using MessageRouter.Packages;
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
    /// that is able to bind to an <see cref="IAddress"/> to receive and respond to incoming <see cref="Package"/>es from
    /// connected remote <see cref="INetMQReceiver"/>s
    /// </summary>
    public class NetMQReceiver : NetMQConnection, INetMQReceiver, IDisposable
    {
        private RouterSocket socket;
        private readonly RequestTaskHandler handler;


        /// <summary>
        /// Gets the <see cref="RequestTaskHandler"/> delegate the <see cref="IReceiver"/> calls when
        /// an incoming message is received
        /// </summary>
        public RequestTaskHandler Handler => handler;


        /// <summary>
        /// Initializes a new instance of <see cref="NetMQReceiver"/>
        /// </summary>
        /// <param name="socket">Inner NetMQ <see cref="RouterSocket"/></param>
        /// <param name="serializer">A serializer that will convert request and response messages to a binary format for transport along the wire</param>
        /// <param name="handler"><see cref="RequestTaskHandler"/> is called when an incoming message is received</param>
        public NetMQReceiver(RouterSocket socket, ISerializer serializer, RequestTaskHandler handler)
            : base(socket, serializer)
        {
            this.socket = socket ?? throw new ArgumentNullException(nameof(socket));
            this.handler = handler ?? throw new ArgumentNullException(nameof(handler));

            socket.ReceiveReady += OnRequestReceived;
        }
        

        /// <summary>
        /// Synchronously trys receiving a <see cref="RequestTask"/> from a connected <see cref="ISender"/>
        /// </summary>
        /// <param name="requestTask">Combination of the request <see cref="Package"/> and a response Action</param>
        /// <returns>Boolean flag indicating whether a request task was retrieved</returns>
        public bool TryReceive(out RequestTask requestTask)
        {
            NetMQMessage message = default(NetMQMessage);
            if (!socket.TryReceiveMultipartMessage(ref message))
            {
                requestTask = default(RequestTask);
                return false;
            }

            requestTask = ExtractTaskFromMessage(message);
            return true;
        }


        /// <summary>
        /// Add <see cref="IAddress"/> to the socket
        /// </summary>
        /// <param name="address"><see cref="IAddress"/> to be added</param>
        public override void SocketAdd(IAddress address)
        {
            socket.Bind(address.ToString());
        }


        /// <summary>
        /// Remote <see cref="IAddress"/> from the socket
        /// </summary>
        /// <param name="address"><see cref="IAddress"/> to be removed</param>
        public override void SocketRemove(IAddress address)
        {
            socket.Unbind(address.ToString());
        }


        private void OnRequestReceived(object sender, NetMQSocketEventArgs e)
        {
            // Move handling request off NetMQPoller thread and onto TaskPool as soon as possible
            Task.Run(() =>
            {
                if (!TryReceive(out var requestTask))
                    return;

                handler(this, requestTask);
            });
        }


        #region Message Builders
        private RequestTask ExtractTaskFromMessage(NetMQMessage requestMessage)
        {
            var request = ExtractPackage(requestMessage);

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


        private Package ExtractPackage(NetMQMessage message)
        {
            if (message.FrameCount == 3 || message.FrameCount == 4)
                // Synchronous messages have request in slot 2
                return serializer.Deserialize<Package>(message[2].ToByteArray());
            else if (message.FrameCount == 5 || message.FrameCount == 6)
                // Asynchronous messages have request in slot 4
                return serializer.Deserialize<Package>(message[4].ToByteArray());
            else
                throw new InvalidOperationException("Request message has unexpected format");
        }


        private NetMQMessage AddResponse(NetMQMessage responseMessage, Package response)
        {
            var data = serializer.Serialize(response);
            responseMessage.Append(data);
            return responseMessage;
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
                    TerminateConnection();
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
