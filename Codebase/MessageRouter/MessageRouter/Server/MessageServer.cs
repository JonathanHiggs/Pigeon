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
    public class MessageServer : IMessageServer<IServerInfo>
    {
        private readonly IMessageFactory messageFactory;
        private readonly IReceiverManager receiverManager;
        private readonly IRequestDispatcher requestDispatcher;
        private readonly ServerInfo serverInfo;
        private readonly object runLock = new object();
        private bool running = false;

        public IServerInfo ServerInfo => serverInfo;


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


        public void Run()
        {
            var tokenSource = new CancellationTokenSource();
            Run(tokenSource.Token);
        }


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

                Reply(task);

                Thread.Yield();
            }
            while (!cancellationToken.IsCancellationRequested);

            lock (runLock)
            {
                running = false;
            }
        }


        private void Reply(RequestTask task)
        {
            var requestObject = messageFactory.ExtractRequest<object>(task.Request);
            var responseObject = requestDispatcher.Handle(requestObject);
            var responseMessage = CreateResponse(responseObject);
            task.ResponseHandler(responseMessage);
        }


        private Message CreateResponse(object responseObject)
        {
            var createResponse = typeof(IMessageFactory)
                .GetMethods(BindingFlags.Public | BindingFlags.Instance)
                .Single(m => m.Name == "CreateResponse");

            var genericMethod = createResponse.MakeGenericMethod(responseObject.GetType());

            return (Message)genericMethod.Invoke(messageFactory, new object[] { responseObject });
        }
    }
}
