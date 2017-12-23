using System;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

using Pigeon.Addresses;

namespace Pigeon.Web
{
    /// <summary>
    /// Implementation of <see cref="IReceiver"/> that wraps a <see cref="HttpListener"/> to receive and respond to incoming
    /// http requests
    /// </summary>
    public class WebReceiver : IWebReceiver
    {
        private readonly List<IAddress> addresses = new List<IAddress>();
        private readonly ManualResetEvent requestWait = new ManualResetEvent(false);
        private HttpListener listener;
        private WebTaskHandler handler;
        private Thread thread;
        private bool running = false;
        private object lockObj = new object();


        /// Gets an enumerable of <see cref="IAddress"/> that the receiver is listening to
        /// </summary>
        public IEnumerable<IAddress> Addresses => addresses;


        /// <summary>
        /// Gets a bool that returns true when the <see cref="IConnection"/> is connected; otherwise false
        /// </summary>
        public bool IsConnected => running;


        /// <summary>
        /// Gets the <see cref="WebTaskHandler"/> delegate that the <see cref="WebReceiver"/> calls upon receiving an incoming
        /// http request
        /// </summary>
        public WebTaskHandler Handler => handler;


        /// <summary>
        /// Initializes a new instance of <see cref="WebReceiver"/>
        /// </summary>
        /// <param name="listener">Inner <see cref="HttpListener"/> that creates a socket and listens for requests</param>
        /// <param name="handler"><see cref="WebTaskHandler"/> delegate that is called when an incoming http request is received</param>
        public WebReceiver(HttpListener listener, WebTaskHandler handler)
        {
            this.listener = listener ?? throw new ArgumentNullException(nameof(listener));
            this.handler = handler ?? throw new ArgumentNullException(nameof(handler));
        }


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
                if (running)
                    return;

                listener.Start();

                thread = new Thread(new ThreadStart(Run));
                thread.Name = "WebReceiver";
                thread.IsBackground = false;
                thread.Start();

                running = true;
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

                    handler(this, context); // Delegate generating response to the handler
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
                if (!running)
                    return;

                listener.Stop();
                listener.Close();
                requestWait.Set();
                thread.Join(TimeSpan.FromMinutes(1));

                running = false;
            }
        }
    }
}
