using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MessageRouter.Common;
using MessageRouter.Verbs;

namespace MessageRouter.Senders
{
    /// <summary>
    /// Manages the resolution and lifecycle of <see cref="ISender"/>s
    /// </summary>
    public interface ISenderCache : ISend, ICache<ISender>
    {
        /// <summary>
        /// Gets a readonly collection of <see cref="ISenderFactory"/>s for creating <see cref="ISender"/>s at runtime
        /// </summary>
        IReadOnlyCollection<ISenderFactory> Factories { get; }


        /// <summary>
        /// Retrieves a <see cref="ISender"/> for the request type depending on registered routing
        /// </summary>
        /// <typeparam name="TRequest">Request type</typeparam>
        /// <returns>Matching <see cref="ISender"/> for the given request type</returns>
        ISender SenderFor<TRequest>();


        /// <summary>
        /// Adds a <see cref="ISenderFactory{TSender}"/> to the registered factories
        /// </summary>
        /// <param name="factory">Factory used to create <see cref="ISender"/>s at when required to send first message to remote <see cref="IReceiver"/></param>
        void AddFactory(ISenderFactory factory);
    }
}
