using System;
using System.Reflection;
using System.Threading.Tasks;

using Pigeon.Annotations;
using Pigeon.Diagnostics;
using Pigeon.Fluent;
using Pigeon.Fluent.Simple;
using Pigeon.Monitors;
using Pigeon.Publishers;
using Pigeon.Receivers;
using Pigeon.Senders;
using Pigeon.Subscribers;

namespace Pigeon
{
    /// <summary>
    /// <see cref="Router"/> represents a transport abstraction; all messages are routed, sent and received 
    /// through the Router
    /// route
    /// </summary>
    public class Router : IRouter<IRouterInfo>
    {
        private readonly ISenderCache senderCache;
        private readonly IMonitorCache monitorCache;
        private readonly IReceiverCache receiverCache;
        private readonly IPublisherCache publisherCache;
        private readonly ISubscriberCache subscriberCache;

        private readonly RouterInfo routerInfo;
        private bool running = false;
        private object lockObj = new object();


        /// <summary>
        /// Gets a <see cref="IRouterInfo"/> to access state information of the <see cref="Router"/>
        /// </summary>
        public IRouterInfo Info => routerInfo;

        
        /// <summary>
        /// Initializes a new instance of <see cref="IPublisherCache"/>
        /// </summary>
        /// <param name="name">A name that can be used to identify the router as a node on a distributed network</param>
        /// <param name="senderCache"></param>
        /// <param name="monitorCache"></param>
        /// <param name="receiverCache"></param>
        /// <param name="publisherCache"></param>
        /// <param name="subscriberCache"></param>
        public Router(
            string name,
            ISenderCache senderCache,
            IMonitorCache monitorCache,
            IReceiverCache receiverCache,
            IPublisherCache publisherCache,
            ISubscriberCache subscriberCache)
        {
            if (String.IsNullOrWhiteSpace(name))
                throw new ArgumentNullException(nameof(name));

            this.senderCache = senderCache ?? throw new ArgumentNullException(nameof(senderCache));
            this.monitorCache = monitorCache ?? throw new ArgumentNullException(nameof(monitorCache));
            this.receiverCache = receiverCache ?? throw new ArgumentNullException(nameof(receiverCache));
            this.publisherCache = publisherCache ?? throw new ArgumentNullException(nameof(publisherCache));
            this.subscriberCache = subscriberCache ?? throw new ArgumentNullException(nameof(subscriberCache));

            routerInfo = new RouterInfo
            {
                Name = name,
                Running = false,
                StartedTimestamp = null,
                StoppedTimestamp = null
            };
        }


        /// <summary>
        /// Distributes a message to any and all connected <see cref="ISubscriber"/>s
        /// </summary>
        /// <typeparam name="TTopic">The topic type of the message to publish</typeparam>
        /// <param name="message">The topic message to distribute</param>
        public void Publish<TTopic>(TTopic message) where TTopic : class
        {
#if DEBUG
            // Try to catch missing attributes in debug only for performance reasons
            if (typeof(TTopic).GetCustomAttribute<TopicAttribute>() is null)
                throw new MissingAttributeException(typeof(TTopic), typeof(TopicAttribute));
#endif

            publisherCache.Publish(message);
        }


        /// <summary>
        /// Dispatches a request asynchronously through an internally resolved <see cref="ISender"/> to a remote
        /// <see cref="IReceiver"/> with a default timeout of one hour
        /// </summary>
        /// <typeparam name="TRequest">Request type</typeparam>
        /// <typeparam name="TResponse">Expected response type</typeparam>
        /// <param name="request">Request object</param>
        /// <returns>Response object</returns>
        public async Task<TResponse> Send<TRequest, TResponse>(TRequest request)
            where TRequest : class
            where TResponse : class
        {
#if DEBUG
            // Try to catch missing attributes in debug only for performance reasons
            var requestAttribute = typeof(TRequest).GetCustomAttribute<RequestAttribute>();

            if (requestAttribute is null)
                throw new MissingAttributeException(typeof(TRequest), typeof(TResponse));

            if (requestAttribute.ResponseType != typeof(TResponse))
                throw new MismatchingResponseTypeException(typeof(TRequest), typeof(TResponse));
#endif

            return await senderCache.Send<TRequest, TResponse>(request);
        }


        /// <summary>
        /// Dispatches a request asynchronously through an internally resolved <see cref="ISender"/> to a remote
        /// <see cref="IReceiver"/>
        /// </summary>
        /// <typeparam name="TRequest">Request type</typeparam>
        /// <typeparam name="TResponse">Expected response type</typeparam>
        /// <param name="request">Request object</param>
        /// <param name="timeout">Time to wait for a response before throwing an exception</param>
        /// <returns>Response object</returns>
        public async Task<TResponse> Send<TRequest, TResponse>(TRequest request, TimeSpan timeout)
            where TRequest : class
            where TResponse : class
        {
#if DEBUG
            // Try to catch missing attributes in debug only for performance reasons
            var requestAttribute = typeof(TRequest).GetCustomAttribute<RequestAttribute>();

            if (requestAttribute is null)
                throw new MissingAttributeException(typeof(TRequest), typeof(TResponse));

            if (requestAttribute.ResponseType != typeof(TResponse))
                throw new MismatchingResponseTypeException(typeof(TRequest), typeof(TResponse));
#endif

            return await senderCache.Send<TRequest, TResponse>(request, timeout);
        }


        public IDisposable Subscribe<TTopic>()
        {
#if DEBUG
            // Try to catch missing attributes in debug only for performance reasons
            if (typeof(TTopic).GetCustomAttribute<TopicAttribute>() is null)
                throw new MissingAttributeException(typeof(TTopic), typeof(TopicAttribute));
#endif

            return subscriberCache.Subscribe<TTopic>();
        }

        
        public void Unsubscribe<TTopic>()
        {
            subscriberCache.Unsubscribe<TTopic>();
        }


        /// <summary>
        /// Starts all internal active transports running
        /// </summary>
        public void Start()
        {
            lock(lockObj)
            {
                if (running)
                    return;

                monitorCache.StartAllMonitors();

                routerInfo.Running = true;
                routerInfo.StartedTimestamp = DateTime.Now;
                routerInfo.StoppedTimestamp = null;

                running = true;
            }
        }


        /// <summary>
        /// Stops all internal active transports running
        /// </summary>
        public void Stop()
        {
            lock(lockObj)
            {
                if (!running)
                    return;

                monitorCache.StopAllMonitors();

                routerInfo.Running = false;
                routerInfo.StoppedTimestamp = DateTime.Now;

                running = false;
            }
        }


        public static Builder Builder(string name)
        {
            return new Builder(name);
        }


        public static ContainerBuilder Builder(string name, IContainer container)
        {
            return new ContainerBuilder(name, container);
        }
    }
}
