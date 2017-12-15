using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessageRouter.Subscriptions
{
    /// <summary>
    /// Takes an incoming message and resolves a handler to process it
    /// </summary>
    public interface ISubscriptionEventDispatcher
    {
        /// <summary>
        /// Dispatches the handling of a published message
        /// </summary>
        /// <param name="eventMessage">The message published by a remote source to be handled</param>
        void Handle(object eventMessage);
    }
}
