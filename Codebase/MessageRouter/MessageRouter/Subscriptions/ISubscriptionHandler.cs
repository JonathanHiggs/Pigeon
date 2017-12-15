using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MessageRouter.Publishers;

namespace MessageRouter.Subscriptions
{
    /// <summary>
    /// Interface for handling a specific type of subscription event that gets registered to a
    /// <see cref="ISubscriptionEventDispatcher"/> and is resolved at runtime when a new event is received from
    /// a remote <see cref="IPublisher"/> source
    /// </summary>
    /// <typeparam name="TEvent">Type of the subscription event</typeparam>
    public interface ISubscriptionHandler<TEvent>
    {
        /// <summary>
        /// Performs any necessary actions due to receiving the event message
        /// </summary>
        /// <param name="eventMessage">Event message</param>
        void Handle(TEvent eventMessage);
    }
}
