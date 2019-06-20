using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

using Pigeon.Addresses;
using Pigeon.Receivers;

namespace Pigeon.Web
{
    /// <summary>
    /// Implementation of <see cref="IReceiver"/> that wraps a <see cref="HttpListener"/> to receive and respond to incoming
    /// http requests
    /// </summary>
    public class WebReceiver : IWebReceiver
    {
        private readonly IWebMessageFactory messageFactory;

        private readonly List<IAddress> addresses = new List<IAddress>();
        private readonly ManualResetEvent requestWait = new ManualResetEvent(false);

        private HttpListener listener;
        private Thread thread;

        private readonly object lockObj = new object();


        /// <summary>
        /// Initializes a new instance of <see cref="WebReceiver"/>
        /// </summary>
        /// <param name="listener">Inner <see cref="HttpListener"/> that creates a socket and listens for requests</param>
        /// <param name="handler"><see cref="RequestTaskHandler"/> is called when an incoming message is received</param>
        public WebReceiver(HttpListener listener, IWebMessageFactory messageFactory, AsyncRequestTaskHandler handler)
        {
            this.listener = listener ?? throw new ArgumentNullException(nameof(listener));
            this.messageFactory = messageFactory ?? throw new ArgumentNullException(nameof(messageFactory));

            Handler = handler ?? throw new ArgumentNullException(nameof(handler));
        }
        

        /// Gets an enumerable of <see cref="IAddress"/> that the receiver is listening to
        /// </summary>
        public IEnumerable<IAddress> Addresses => addresses;


        /// <summary>
        /// Gets a bool that returns true when the <see cref="IConnection"/> is connected; otherwise false
        /// </summary>
        public bool IsConnected { get; private set; } = false;


        /// <summary>
        /// Gets the <see cref="RequestTaskHandler"/> delegate the <see cref="IReceiver"/> calls when
        /// an incoming message is received
        /// </summary>
        public AsyncRequestTaskHandler Handler { get; }


        /// <summary>
        /// Adds the <see cref="IAddress"/> to the set of adresses the <see cref="IConnection"/> will listen to
        /// for incoming <see cref="Package"/>s
        /// </summary>
        /// <param name="address"><see cref="IAddress"/> to add</param>
        public void AddAddress(IAddress address)
        {
            addresses.Add(address);
            listener.Prefixes.Add(address.ToString());
        }


        /// <summary>
        /// Removes the <see cref="IAddress"/> from the set of addresses the <see cref="IConnection"/> will listen to
        /// for incoming <see cref="Packages.Package"/>s
        /// </summary>
        /// <param name="address"><see cref="IAddress"/> to remove</param>
        public void RemoveAddress(IAddress address)
        {
            addresses.Remove(address);
            listener.Prefixes.Remove(address.ToString());
        }


        /// <summary>
        /// Removes all <see cref="IAddress"/> from the set of addresses the <see cref="IConnection"/> will listen to
        /// for incoming <see cref="Packages.Package"/>s
        /// </summary>
        public void RemoveAllAddresses()
        {
            listener.Prefixes.Clear();
            addresses.Clear();
        }


        /// <summary>
        /// Initializes the connections to all added <see cref="IAddress"/>es
        /// </summary>
        public void InitializeConnection()
        {
            lock (lockObj)
            {
                if (IsConnected)
                    return;

                listener.Start();

                thread = new Thread(new ThreadStart(Run));
                thread.Name = $"WebReceiver-{addresses.First().ToString()}";
                thread.IsBackground = false;
                thread.Start();

                IsConnected = true;
            }
        }


        /// <summary>
        /// Inner thread loop
        /// </summary>
        public void Run()
        {
            while (listener.IsListening)
            {
                requestWait.Reset();

                Task.Run(async () =>
                {
                    var context = await listener.GetContextAsync();

                    requestWait.Set(); // Allow outer loop to continue and create new 

                    await OnRequestReceived(context);
                });

                requestWait.WaitOne(); // Wait until a request is received before spawning a new task listening for a request
            }
        }


        /// <summary>
        /// Terminates the connection to all added <see cref="IAddress"/>es
        /// </summary>
        public void TerminateConnection()
        {
            lock (lockObj)
            {
                if (!IsConnected)
                    return;

                listener.Stop();
                listener.Close();
                requestWait.Set();
                thread.Join(TimeSpan.FromMinutes(1));

                IsConnected = false;
            }
        }


        private async Task OnRequestReceived(HttpListenerContext context)
        {
            var request = messageFactory.ExtractRequestMessage(context.Request);

            var requestTask = new AsyncRequestTask(request, async (response) =>
            {
                await messageFactory.SetResponseMessage(context.Response, response);
                context.Response.Close();
            });

            await Handler(this, requestTask);
        }
    }
}
