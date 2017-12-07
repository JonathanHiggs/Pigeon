using MessageRouter.Receivers;
using MessageRouter.Serialization;
using NetMQ;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessageRouter.NetMQ.Receivers
{
    /// <summary>
    /// NetMQ implementation of <see cref="IReceiverMonitor"/>. Manages the state of <see cref="INetMQReceiver"/>s
    /// and attaches them to a <see cref="NetMQPoller"/>
    /// </summary>
    public class NetMQReceiverMonitor : IReceiverMonitor
    {
        private readonly INetMQReceiver receiver;
        private readonly INetMQPoller poller;


        /// <summary>
        /// Raised when an incoming message is received
        /// </summary>
        public event RequestTaskHandler RequestReceived;


        /// <summary>
        /// Initializes a new NetMQReceiverMonitor
        /// </summary>
        /// <param name="receiver">NetMQReceiver that can receive requests from remotes</param>
        /// <param name="poller">NetMQPoller monitors for incoming messages</param>
        public NetMQReceiverMonitor(INetMQReceiver receiver, INetMQPoller poller)
        {
            this.receiver = receiver ?? throw new ArgumentNullException(nameof(receiver));
            this.poller = poller ?? throw new ArgumentNullException(nameof(poller));

            receiver.RequestReceived += OnRequestReceived;
            poller.Add(receiver.PollableSocket);
        }


        /// <summary>
        /// Asynchronously starts the receiver manager running
        /// </summary>
        public void StartReceivers()
        {
            poller.RunAsync();
            receiver.Bind();
        }


        /// <summary>
        /// Stops the asynchronous receiver manager
        /// </summary>
        public void StopReceivers()
        {
            poller.StopAsync();
            receiver.UnbindAll();
        }


        private void OnRequestReceived(IReceiver receiver, RequestTask requestTask)
        {
            RequestReceived?.Invoke(receiver, requestTask);
        }
    }
}
