using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MessageRouter.Addresses;

namespace MessageRouter.Receivers
{
    /// <summary>
    /// Manages the state of Receivers and can be used to combine several Receivers to create a remote that
    /// can receiver requests from multiple sources
    /// </summary>
    public class ReceiverManager : IReceiverManager
    {
        private IReceiver receiver;
        private bool running = false;
        private object lockObj = new object();


        /// <summary>
        /// Raised when an incoming message is received
        /// </summary>
        public event RequestTaskDelegate RequestReceived;


        /// <summary>
        /// Initializes a ReceiverManager
        /// </summary>
        /// <param name="receiver">IReceiver endpoint</param>
        public ReceiverManager(IReceiver receiver)
        {
            this.receiver = receiver ?? throw new ArgumentNullException(nameof(receiver));
        }


        /// <summary>
        /// Synchronously retrieves a <see cref="RequestTask"/> from a managed <see cref="IReceiver"/>
        /// </summary>
        /// <returns></returns>
        public RequestTask Receive()
        {
            if (!running)
                throw new InvalidOperationException($"{GetType().Name} not started");

            return receiver.Receive();
        }


        /// <summary>
        /// Synchronously tries receiving a <see cref="RequestTask"/> from a managed <see cref="IReceiver"/>
        /// </summary>
        /// <param name="requestTask">RequestTask</param>
        /// <returns></returns>
        public bool TryReceive(out RequestTask requestTask)
        {
            if (!running)
                throw new InvalidOperationException($"{GetType().Name} not runnng");

            return receiver.TryReceive(out requestTask);
        }


        /// <summary>
        /// Binds the <see cref="IReceiver"/> to the <see cref="IAddress"/> to start accepting incoming requests
        /// </summary>
        public void Start()
        {
            lock (lockObj)
            {
                if (running)
                    return;
                
                receiver.Bind();

                running = true;
            }
        }


        /// <summary>
        /// Unbinds the <see cref="IReceiver"/> from the <see cref="IAddress"/> to stop accepting incoming requests
        /// </summary>
        public void Stop()
        {
            lock (lockObj)
            {
                if (!running)
                    return;

                receiver.UnbindAll();
                running = false;
            }
        }
    }
}
