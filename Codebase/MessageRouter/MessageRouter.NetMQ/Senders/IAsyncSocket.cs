using System.Threading.Tasks;
using MessageRouter.Addresses;
using MessageRouter.Receivers;
using NetMQ;
using System;

namespace MessageRouter.NetMQ.Senders
{
    /// <summary>
    /// Socket interface for connecting and asynchronously sending and receiving <see cref="NetMQMessage"/>s
    /// </summary>
    public interface IAsyncSocket : ISocketPollable
    {
        /// <summary>
        /// Connects the socket to the <see cref="IAddress"/>
        /// </summary>
        /// <param name="address">Address of the remote to connect to</param>
        void Connect(string address);


        /// <summary>
        /// Disconects the socket from the <see cref="IAddress"/>
        /// </summary>
        /// <param name="address">Address of the connected remote</param>
        void Disconnect(string address);


        /// <summary>
        /// Dispatches a <see cref="NetMQMessage"/> over the transport to a remote <see cref="IReceiver"/> and
        /// returns a task that will complete when a response is returned from the remote or when the
        /// timeout elapses
        /// </summary>
        /// <param name="message"><see cref="NetMQMessage"/> to send to the remote</param>
        /// <param name="timeout"><see cref="TimeSpan"/> after which the returned <see cref="Task{NetMQMessage}"/> will throw an error if no response has been received</param>
        /// <returns>A task that will complete successfully when a responce is received or that will fail once the timeout elapses</returns>
        Task<NetMQMessage> SendAndReceive(NetMQMessage message, TimeSpan timeout);
    }
}