using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessageRouter.Subscriptions
{
    /// <summary>
    /// Finds an appropriate handler from the registered handlers to invoke when a subscription event message is
    /// received
    /// </summary>
    public class SubscriptionEventDispatcher : ISubscriptionEventDispatcher
    {
        private readonly Dictionary<Type, SubscriptionEventFunction> handlers = new Dictionary<Type, SubscriptionEventFunction>();


        /// <summary>
        /// Finds and invokes a registered handler for the event message
        /// </summary>
        /// <param name="eventMessage">Subscription event message</param>
        public void Handle(object eventMessage)
        {
            if (null == eventMessage)
                return;

            var eventType = eventMessage.GetType();
            if (!handlers.TryGetValue(eventType, out var handler))
                return;

            handler(eventMessage);
        }


        /// <summary>
        /// Registeres a <see cref="ISubscriptionHandler{TEvent}"/>
        /// </summary>
        /// <typeparam name="TEvent">Type of the subscription event message</typeparam>
        /// <param name="handler">Subscription event handler</param>
        /// <returns>Returns the same <see cref="SubscriptionEventDispatcher"/> for fluent construction</returns>
        public SubscriptionEventDispatcher Register<TEvent>(ISubscriptionHandler<TEvent> handler)
        {
            handlers.Add(typeof(TEvent), eventMessage => handler.Handle((TEvent)eventMessage));
            return this;
        }


        /// <summary>
        /// Registers a <see cref="SubscriptionEventHandler{TEvent}"/>
        /// </summary>
        /// <typeparam name="TEvent">Type of the subscription event message</typeparam>
        /// <param name="handler">Subscription event handler</param>
        /// <returns>Returns the same <see cref="SubscriptionEventDispatcher"/> for fluent construction</returns>
        public SubscriptionEventDispatcher Register<TEvent>(SubscriptionEventHandler<TEvent> handler)
        {
            handlers.Add(typeof(TEvent), eventMessage => handler((TEvent)eventMessage));
            return this;
        }


        /// <summary>
        /// Initializes a new instance of <see cref="SubscriptionEventDispatcher"/> for fluent construction
        /// </summary>
        /// <returns>An empty <see cref="SubscriptionEventDispatcher"/></returns>
        public static SubscriptionEventDispatcher Create()
        {
            return new SubscriptionEventDispatcher();
        }
    }
}
