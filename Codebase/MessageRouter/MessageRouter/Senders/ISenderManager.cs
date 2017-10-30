using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessageRouter.Senders
{
    /// <summary>
    /// Manages the state of <see cref="ISender"/>s and resolves a sender for a given request object type
    /// </summary>
    public interface ISenderManager
    {
        /// <summary>
        /// Registers a 
        /// </summary>
        /// <typeparam name="TRequest"></typeparam>
        /// <param name="address"></param>
        void Add<TRequest>(IAddress address);


        /// <summary>
        /// Resolves a <see cref="ISender"/> for the type of the request with the configured routing
        /// </summary>
        /// <typeparam name="TRequest">Request type</typeparam>
        /// <returns>Sender for the request type</returns>
        ISender SenderFor<TRequest>();
    }
}
