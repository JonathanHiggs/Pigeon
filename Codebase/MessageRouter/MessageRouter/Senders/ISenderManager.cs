using MessageRouter.Addresses;
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
        /// Registers an <see cref="IAddress"/> as the remote destination for the Request type
        /// </summary>
        /// <typeparam name="TRequest">Type of request object</typeparam>
        /// <param name="address">Address of remote</param>
        void Add<TRequest>(IAddress address);


        void AddAsync<TRequest>(IAddress address);


        /// <summary>
        /// Resolves a <see cref="ISender"/> for the type of the request with the configured routing
        /// </summary>
        /// <typeparam name="TRequest">Request type</typeparam>
        /// <returns>Sender for the request type</returns>
        ISender SenderFor<TRequest>();


        IAsyncSender AsyncSenderFor<TRequest>();
    }
}
