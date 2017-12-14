using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using MessageRouter.Addresses;
using MessageRouter.Messages;
using MessageRouter.Monitors;
using MessageRouter.Requests;
using MessageRouter.Senders;

namespace MessageRouter.Receivers
{
    /// <summary>
    /// Manages the lifecycle of <see cref="IReceiver"/>s
    /// </summary>
    public class ReceiverCache : IReceiverCache
    {
        private readonly IMonitorCache monitorCache;
        private readonly IMessageFactory messageFactory;
        private readonly IRequestDispatcher requestDispatcher;
        private readonly Dictionary<IAddress, IReceiver> receivers = new Dictionary<IAddress, IReceiver>();
        private readonly Dictionary<Type, IReceiverFactory> factories = new Dictionary<Type, IReceiverFactory>();


        /// <summary>
        /// Gets a readonly collection of <see cref="IReceiverFactory"/>s for creating <see cref="IReceiver"/>s at runtime
        /// </summary>
        public IReadOnlyCollection<IReceiverFactory> ReceiverFactories => factories.Values;


        /// <summary>
        /// Initializes a new instance of <see cref="ReceiverCache"/>
        /// </summary>
        /// <param name="monitorCache">Stores <see cref="IMonitor"/>s that actively manage <see cref="IReceiver"/></param>
        /// <param name="messageFactory">Creates and extracts <see cref="Message"/>s that are received from remote <see cref="ISender"/>s</param>
        /// <param name="requestDispatcher">Maps request messages to registered handlers for processing</param>
        public ReceiverCache(IMonitorCache monitorCache, IMessageFactory messageFactory, IRequestDispatcher requestDispatcher)
        {
            this.monitorCache = monitorCache ?? throw new ArgumentNullException(nameof(monitorCache));
            this.messageFactory = messageFactory ?? throw new ArgumentNullException(nameof(messageFactory));
            this.requestDispatcher = requestDispatcher ?? throw new ArgumentNullException(nameof(requestDispatcher));
        }


        /// <summary>
        /// Processes an incoming <see cref="RequestTask"/> extracting the wrapped request from the <see cref="Message"/> and forwarding 
        /// to the <see cref="IRequestDispatcher"/> to calculate a response that is sent back over the wire
        /// </summary>
        /// <param name="requestTask">Combined incoming message with method of returning a response</param>
        public void HandleRequest(RequestTask requestTask)
        {
            var requestObject = messageFactory.ExtractMessage(requestTask.Request);
            var responseObject = requestDispatcher.Handle(requestObject);
            var responseMessage = messageFactory.CreateMessage(responseObject);
            requestTask.ResponseHandler(responseMessage);
        }


        /// <summary>
        /// Adds a <see cref="IReceiverFactory{TReceiver}"/> to the cache for config-time creation of <see cref="IReceiver"/>s
        /// </summary>
        /// <typeparam name="TReceiver">Transport specific implementation of <see cref="IReceiver"/></typeparam>
        /// <param name="factory">Factory used to create <see cref="IReceiver"/>s at config-time</param>
        public void AddFactory<TReceiver>(IReceiverFactory<TReceiver> factory) where TReceiver : IReceiver
        {
            if (null == factory)
                throw new ArgumentNullException(nameof(factory));

            if (factories.ContainsKey(factory.ReceiverType))
                return;

            factories.Add(factory.ReceiverType, factory);
            monitorCache.AddMonitor(factory.ReceiverMonitor);
        }


        /// <summary>
        /// Creates and adds a <see cref="IReceiver"/> to the cache that binds and accepts requests sent to the <see cref="IAddress"/> created from the matching <see cref="IReceiverFactory{TReceiver}"/>
        /// </summary>
        /// <typeparam name="TReceiver">Transport specific implementation of <see cref="IReceiver"/> to create</typeparam>
        /// <param name="address">The <see cref="IAddress"/> the <see cref="IReceiver"/> binds to</param>
        public void AddReceiver<TReceiver>(IAddress address) where TReceiver : IReceiver
        {
            if (null == address)
                throw new ArgumentNullException(nameof(address));

            if (receivers.ContainsKey(address))
                throw new InvalidOperationException(nameof(address));

            var factory = factories[typeof(TReceiver)];
            var receiver = factory.CreateReceiver(address);
            receiver.RequestReceived += (s, e) => Task.Run(() => HandleRequest(e));
            receivers.Add(address, receiver);
        }
    }
}
