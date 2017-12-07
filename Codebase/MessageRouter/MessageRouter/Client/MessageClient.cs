using MessageRouter.Messages;
using MessageRouter.Senders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessageRouter.Client
{
    /// <summary>
    /// Sends request messages to remotes
    /// </summary>
    public class MessageClient : IMessageClient
    {
        private readonly ISenderCache senderCache;
        private readonly IMonitorCache monitorCache;
        private readonly IMessageFactory messageFactory;
        private bool running = false;
        private object lockObj = new object();


        /// <summary>
        /// Gets the status of the client
        /// </summary>
        public bool IsRunning => running;


        /// <summary>
        /// Initializes an instance of a MessageClient
        /// </summary>
        /// <param name="senderCache">Manages sender connections to remotes</param>
        /// <param name="messageFactory">Wraps requests in the message protocol</param>
        public MessageClient(ISenderCache senderCache, IMonitorCache monitorCache, IMessageFactory messageFactory)
        {
            this.senderCache = senderCache ?? throw new ArgumentNullException(nameof(senderCache));
            this.monitorCache = monitorCache ?? throw new ArgumentNullException(nameof(monitorCache));
            this.messageFactory = messageFactory ?? throw new ArgumentNullException(nameof(messageFactory));
        }


        /// <summary>
        /// Dispatches a request asynchronously to a remote routed by the <see cref="ISenderMonitor"/>
        /// Default timeout of one hour
        /// </summary>
        /// <typeparam name="TRequest">Request type</typeparam>
        /// <typeparam name="TResponse">Expected response type</typeparam>
        /// <param name="request">Request object</param>
        /// <returns>Response object</returns>
        public async Task<TResponse> Send<TRequest, TResponse>(TRequest request)
            where TRequest : class
            where TResponse : class
        {
            return await Send<TRequest, TResponse>(request, TimeSpan.FromHours(1));
        }


        /// <summary>
        /// Dispatches a request asynchronously to a remote routed by the <see cref="ISenderMonitor"/>
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
            if (null == request)
                throw new ArgumentNullException(nameof(request));

            var sender = senderCache.SenderFor<TRequest>();
            var requestMessage = messageFactory.CreateRequest(request);
            var responseMessage = await sender.SendAndReceive(requestMessage, timeout);
            var response = messageFactory.ExtractResponse<TResponse>(responseMessage);
            return response;
        }


        /// <summary>
        /// Starts the <see cref="Client"/> running
        /// </summary>
        public void Start()
        {
            lock (lockObj)
            {
                if (running)
                    return;

                monitorCache.StartAllMonitors();
                running = true;
            }
        }


        /// <summary>
        /// Stops the <see cref="Client"/> running and disconnects <see cref="ISender"/>
        /// </summary>
        public void Stop()
        {
            lock (lockObj)
            {
                if (!running)
                    return;

                monitorCache.StopAllMonitors();
                running = false;
            }
        }
    }
}
