using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using MessageRouter.Addresses;
using MessageRouter.Messages;
using MessageRouter.Monitors;
using MessageRouter.Requests;

namespace MessageRouter.Receivers
{
    public class ReceiverCache : IReceiverCache
    {
        private readonly IMonitorCache monitorCache;
        private readonly IMessageFactory messageFactory;
        private readonly IRequestDispatcher requestDispatcher;

        private readonly Dictionary<IAddress, IReceiver> receivers = new Dictionary<IAddress, IReceiver>();
        private readonly Dictionary<Type, IReceiverFactory> receiverFactories = new Dictionary<Type, IReceiverFactory>();


        public IReadOnlyCollection<IReceiverFactory> ReceiverFactories => throw new NotImplementedException();


        public ReceiverCache(IMonitorCache monitorCache, IMessageFactory messageFactory, IRequestDispatcher requestDispatcher)
        {
            this.monitorCache = monitorCache ?? throw new ArgumentNullException(nameof(monitorCache));
            this.messageFactory = messageFactory ?? throw new ArgumentNullException(nameof(messageFactory));
            this.requestDispatcher = requestDispatcher ?? throw new ArgumentNullException(nameof(requestDispatcher));
        }


        public void HandleRequest(RequestTask requestTask)
        {
            var requestObject = messageFactory.ExtractRequest(requestTask.Request);
            var responseObject = requestDispatcher.Handle(requestObject);
            var responseMessage = messageFactory.CreateResponse(responseObject);
            requestTask.ResponseHandler(responseMessage);
        }


        public void AddFactory<TReceiver>(IReceiverFactory<TReceiver> factory) where TReceiver : IReceiver
        {
            receiverFactories.Add(factory.ReceiverType, factory);
        }


        public void AddReceiver<TReceiver>(IAddress address) where TReceiver : IReceiver
        {
            var factory = receiverFactories[typeof(TReceiver)];
            var receiver = factory.CreateReceiver(address);
            receiver.RequestReceived += (s, e) => Task.Run(() => HandleRequest(e));
            receivers.Add(address, receiver);
            monitorCache.AddMonitor(factory.ReceiverMonitor);
        }
    }
}
