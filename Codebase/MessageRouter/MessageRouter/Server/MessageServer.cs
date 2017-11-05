using MessageRouter.Messages;
using MessageRouter.Receivers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MessageRouter.Server
{
    /// <summary>
    /// Basic implementation of active server process that accepts and responds to incoming requests
    /// </summary>
    public class MessageServer : IMessageServer<IServerInfo>
    {
        private readonly IMessageFactory messageFactory;
        private readonly IReceiverManager receiverManager;
        private readonly IRequestDispatcher requestDispatcher;
        private readonly ServerInfo serverInfo;
        private readonly object runLock = new object();
        private bool running = false;

        public IServerInfo ServerInfo => serverInfo;


        /// <summary>
        /// Initializes a new instance of MessageServer
        /// </summary>
        /// <param name="messageFactory"><see cref="IMessageFactory"/> dependency for constructing <see cref="Message"/>s and extracting request objects</param>
        /// <param name="receiverManager"><see cref="IReceiverManager"/> dependency for managing <see cref="IReceiver"/>s</param>
        /// <param name="requestDispatcher"><see cref="IRequestDispatcher"/> dependency for routing and handling incoming requests</param>
        /// <param name="name">Name identifying the server</param>
        public MessageServer(IMessageFactory messageFactory, IReceiverManager receiverManager, IRequestDispatcher requestDispatcher, string name)
        {
            this.messageFactory = messageFactory ?? throw new ArgumentNullException(nameof(messageFactory));
            this.receiverManager = receiverManager ?? throw new ArgumentNullException(nameof(receiverManager));
            this.requestDispatcher = requestDispatcher ?? throw new ArgumentNullException(nameof(requestDispatcher));

            serverInfo = new Server.ServerInfo
            {
                Name = name,
                Running = false,
                StartUpTimeStamp = null
            };
        }


        /// <summary>
        /// Synchronously starts the server accepting requests
        /// </summary>
        public void Run()
        {
            var tokenSource = new CancellationTokenSource();
            Run(tokenSource.Token);
        }


        /// <summary>
        /// Synchronously starts the server accepting requests
        /// </summary>
        /// <param name="cancellationToken"></param>
        public void Run(CancellationToken cancellationToken)
        {
            lock (runLock)
            {
                if (running)
                    throw new InvalidOperationException("Server is already running");

                running = true;
                serverInfo.StartUpTimeStamp = DateTime.Now;
            }

            // Main loop
            do
            {
                var task = receiverManager.Receive();

                HandleAndRespond(task);

                Thread.Yield();
            }
            while (!cancellationToken.IsCancellationRequested);

            lock (runLock)
            {
                running = false;
            }
        }


        /// <summary>
        /// Extracts and responds to requests
        /// </summary>
        /// <param name="requestTask">Incoming request task</param>
        public void HandleAndRespond(RequestTask requestTask)
        {
            var requestObject = messageFactory.ExtractRequest(requestTask.Request);
            var responseObject = requestDispatcher.Handle(requestObject);
            var responseMessage = CreateResponse(responseObject);
            requestTask.ResponseHandler(responseMessage);
        }


        /// <summary>
        /// Wraps a response object in a <see cref="Message"/> for returning to remote source
        /// </summary>
        /// <param name="responseObject">Response object</param>
        /// <returns>Response <see cref="Message"/></returns>
        public Message CreateResponse(object responseObject)
        {
            var createResponse = typeof(IMessageFactory)
                .GetMethods(BindingFlags.Public | BindingFlags.Instance)
                .Single(m => m.Name == "CreateResponse");

            var genericMethod = createResponse.MakeGenericMethod(responseObject.GetType());

            return (Message)genericMethod.Invoke(messageFactory, new object[] { responseObject });
        }
    }
}
