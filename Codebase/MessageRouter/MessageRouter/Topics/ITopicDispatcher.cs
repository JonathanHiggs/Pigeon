using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessageRouter.Topics
{
    /// <summary>
    /// Takes an incoming topic message and resolves a handler to process it
    /// </summary>
    public interface ITopicDispatcher
    {
        /// <summary>
        /// Dispatches the handling of a published message
        /// </summary>
        /// <param name="message">The message published by a remote source to be handled</param>
        void Handle(object message);
    }
}
