using MessageRouter.Receivers;
using MessageRouter.Serialization;
using NetMQ;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessageRouter.NetMQ
{
    /// <summary>
    /// NetMQ implementation of <see cref="IAsyncReceiverManager"/>. Manages the state of NetMQ <see cref="NetMQAsyncReceiver"/>s
    /// </summary>
    public class NetMQAsyncReceiverManager : IAsyncReceiverManager
    {
        private readonly NetMQAsyncReceiver receiver;
        private readonly NetMQPoller poller;
        private Task pollerTask;


        /// <summary>
        /// Raised when an incoming message is received
        /// </summary>
        public event RequestTaskDelegate RequestReceived;


        /// <summary>
        /// Initializes a new NetMQAsyncReceiverManager
        /// </summary>
        /// <param name="receiver">NetMQAsyncReceiver that can receive requests from remotes</param>
        public NetMQAsyncReceiverManager(NetMQAsyncReceiver receiver)
        {
            this.receiver = receiver ?? throw new ArgumentNullException(nameof(receiver));
            this.poller = new NetMQPoller { receiver.PollableSocket };

            receiver.RequestReceived += RequestReceived;
        }


        /// <summary>
        /// Asynchronously starts the receiver manager running
        /// </summary>
        public void Start()
        {
            poller.RunAsync();
        }


        /// <summary>
        /// Stops the asynchronous receiver manager
        /// </summary>
        public void Stop()
        {
            poller.StopAsync();
        }
    }
}
