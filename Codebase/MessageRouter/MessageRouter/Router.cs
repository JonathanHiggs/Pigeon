using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using MessageRouter.Fluent;
using MessageRouter.Messages;
using MessageRouter.Monitors;
using MessageRouter.Receivers;
using MessageRouter.Senders;
using MessageRouter.Server;

namespace MessageRouter
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

        private readonly RouterInfo routerInfo;
        private bool running = false;
        private object lockObj = new object();


        /// <summary>
        /// Gets a <see cref="IRouterInfo"/> to access state information of the <see cref="Router"/>
        /// </summary>
        public IRouterInfo Info => routerInfo;


        /// <summary>
        /// Initializes a new instance of <see cref="Router"/>
        /// </summary>
        /// <param name="name"></param>
        /// <param name="senderCache">Manages sender connections to remotes</param>
        /// <param name="monitorCache">Manages the state of <see cref="ISender"/>s</param>
        /// <param name="messageFactory">Wraps requests in the message protocol</param>
        /// <param name="receiverMonitor">Manages the state of <see cref="IReceiver"/>s</param>
        /// <param name="requestDispatcher">Resolves a registered handler for incoming requests from <see cref="IReceiver"/>s</param>
        public Router(
            string name,
            ISenderCache senderCache,
            IMonitorCache monitorCache,
            IReceiverCache receiverCache)
        {
            if (String.IsNullOrWhiteSpace(name))
                throw new ArgumentNullException(nameof(name));

            this.senderCache = senderCache ?? throw new ArgumentNullException(nameof(senderCache));
            this.monitorCache = monitorCache ?? throw new ArgumentNullException(nameof(monitorCache));
            this.receiverCache = receiverCache ?? throw new ArgumentNullException(nameof(receiverCache));

            routerInfo = new RouterInfo
            {
                Name = name,
                Running = false,
                StartedTimestamp = null,
                StoppedTimestamp = null
            };
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
            return await senderCache.Send<TRequest, TResponse>(request, timeout);
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


        public static RouterBuilder Builder()
        {
            return new RouterBuilder();
        }
    }
}
