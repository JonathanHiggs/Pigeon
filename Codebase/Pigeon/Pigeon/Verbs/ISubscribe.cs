using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pigeon.Publishers;

namespace Pigeon.Verbs
{
    /// <summary>
    /// Common verb interface that defines now a node is able to initialize a subscription to published topic messages
    /// </summary>
    public interface ISubscribe
    {
        /// <summary>
        /// Initializes a subscription to the topic message stream from a remote <see cref="IPublisher"/>
        /// </summary>
        /// <typeparam name="TTopic">The type of the published topic message</typeparam>
        /// <returns>A representation of the subscription, the dispose method can be used to terminate the subscription</returns>
        IDisposable Subscribe<TTopic>();


        /// <summary>
        /// Terminates a subscription to the topic message stream
        /// </summary>
        /// <typeparam name="TTopic">The type of the published topic message</typeparam>
        void Unsubscribe<TTopic>();
    }
}
