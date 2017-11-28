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
    /// Basic client implementation
    /// </summary>
    public class MessageClient : IMessageClient
    {
        private readonly ISenderMonitor senderMonitor;
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
        /// <param name="senderMonitor">Manages sender connections to remotes</param>
        /// <param name="messageFactory">Wraps requests in the message protocol</param>
        public MessageClient(ISenderMonitor senderMonitor, IMessageFactory messageFactory)
        {
            this.senderMonitor = senderMonitor ?? throw new ArgumentNullException(nameof(senderMonitor));
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

            var requestMessage = messageFactory.CreateRequest(request);
            var sender = senderMonitor.SenderFor<TRequest>();
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
                    throw new InvalidOperationException($"{GetType().Name} is already running");

                senderMonitor.Start();
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
                    throw new InvalidOperationException($"{GetType().Name} is not running");

                senderMonitor.Stop();
                running = false;
            }
        }
    }
}
