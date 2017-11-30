using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MessageRouter.Addresses;

namespace MessageRouter.Senders
{
    public class SenderRouter : ISenderRouter
    {
        private readonly Dictionary<Type, ISenderFactory> senderFactoryFromSenderType = new Dictionary<Type, ISenderFactory>();
        private readonly Dictionary<Type, ISender> sendersFromRequestType = new Dictionary<Type, ISender>();
        private readonly Dictionary<Type, IAddress> addressFromRequestType = new Dictionary<Type, IAddress>();
        private readonly Dictionary<IAddress, Type> senderFromAddressMap = new Dictionary<IAddress, Type>();
        private readonly Dictionary<Type, ISenderMonitor> senderMonitorFromSenderType = new Dictionary<Type, ISenderMonitor>();

        private readonly Dictionary<Type, ISender> senderCache = new Dictionary<Type, ISender>();
        private readonly Dictionary<Type, ISenderMonitor> monitorCache = new Dictionary<Type, ISenderMonitor>();
        private bool running = false;
        private object lockObj = new object();


        public ISenderRouter AddFactory<TSender>(ISenderFactory factory) where TSender : ISender
        {
            if (senderFactoryFromSenderType.ContainsKey(typeof(TSender)))
                throw new InvalidOperationException($"Factory already registered for {typeof(TSender).Name}");

            senderFactoryFromSenderType.Add(typeof(TSender), factory);

            return this;
        }


        public ISenderRouter AddRequestMapping<TRequest>(IAddress address)
        {
            if (addressFromRequestType.ContainsKey(typeof(TRequest)))
                throw new InvalidOperationException($"Request {typeof(TRequest).Name} is already mapped");

            addressFromRequestType.Add(typeof(TRequest), address);

            return this;
        }


        public ISenderRouter AddSender<TSender>(IAddress address) where TSender : ISender
        {
            if (senderFromAddressMap.ContainsKey(address))
                throw new InvalidOperationException($"Address {address.ToString()} already mapped");

            senderFromAddressMap.Add(address, typeof(TSender));

            return this;
        }


        public ISender SenderFor<TRequest>()
        {
            var requestType = typeof(TRequest);

            if (!senderCache.TryGetValue(requestType, out var sender))
                sender = ResolveSender(requestType);

            return sender;
        }


        public void Start()
        {
            lock(lockObj)
            {
                if (running)
                    return;

                foreach (var monitor in monitorCache.Values)
                    monitor.Start();

                running = true;
            }
        }


        public void Stop()
        {
            lock(lockObj)
            {
                if (!running)
                    return;

                foreach (var monitor in monitorCache.Values)
                    monitor.Start();

                running = false;
            }
        }


        private ISender ResolveSender(Type requestType)
        {
            lock (lockObj)
            {
                var address = addressFromRequestType[requestType];
                var senderType = senderFromAddressMap[address];
                var senderFactory = senderFactoryFromSenderType[senderType];
                var sender = senderFactory.GetSender(address);
                senderCache[requestType] = sender;

                if (!monitorCache.TryGetValue(senderType, out var senderMonitor))
                {
                    senderMonitor = senderFactory.GetMonitor();

                    if (running)
                        senderMonitor.Start();
                }

                senderMonitor.Add(sender);

                return sender;
            }
        }

    }
}
