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
    /// Basic client to make synchronous calls to a remote
    /// </summary>
    public class MessageClient : IMessageClient
    {
        private readonly ISenderManager senderManager;
        private readonly IMessageFactory messageFactory;


        /// <summary>
        /// Initializes an instance of a MessageClient
        /// </summary>
        /// <param name="senderManager">Manages sender connections to remotes</param>
        /// <param name="messageFactory">Wraps requests in the message protocol</param>
        public MessageClient(ISenderManager senderManager, IMessageFactory messageFactory)
        {
            this.senderManager = senderManager ?? throw new ArgumentNullException(nameof(senderManager));
            this.messageFactory = messageFactory ?? throw new ArgumentNullException(nameof(messageFactory));
        }


        /// <summary>
        /// Dispatches a request to a remote routed by the <see cref="ISenderManager"/>
        /// </summary>
        /// <typeparam name="TResponse">Expected reponse type</typeparam>
        /// <typeparam name="TRequest">Request type</typeparam>
        /// <param name="request">Request object</param>
        /// <returns>Response object</returns>
        public TResponse Send<TRequest, TResponse>(TRequest request) 
            where TRequest : class 
            where TResponse : class
        {
            if (null == request)
                throw new ArgumentNullException(nameof(request));

            var requestMessage = messageFactory.CreateRequest(request);
            var sender = senderManager.SenderFor<TRequest>();
            var responseMessage = sender.SendAndReceive(requestMessage);
            var response = messageFactory.ExtractResponse<TResponse>(responseMessage);
            return response;
        }


        public async Task<TResponse> SendAsync<TRequest, TResponse>(TRequest request, double timeout = 3600.0)
            where TRequest : class
            where TResponse : class
        {
            if (null == request)
                throw new ArgumentNullException(nameof(request));

            var requestMessage = messageFactory.CreateRequest(request);
            var sender = senderManager.AsyncSenderFor<TRequest>();
            var responseMessage = await sender.SendAndReceiveAsync(requestMessage, timeout);
            var response = messageFactory.ExtractResponse<TResponse>(responseMessage);
            return response;
        }


        public void Start() => senderManager.Start();


        public void Stop() => senderManager.Stop();
    }
}
