using MessageRouter.Addresses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessageRouter.Senders
{
    /// <summary>
    /// 
    /// </summary>
    public interface ISenderRouter
    {
        /// <summary>
        /// Registers an <see cref="IAddress"/> as the remote destination for the TRequest type
        /// </summary>
        /// <typeparam name="TRequest">Type of request object</typeparam>
        /// <param name="address">Address of remote</param>
        /// <returns></returns>
        ISenderRouter AddRequestMapping<TRequest>(IAddress address);


        ISenderRouter AddSender<TSender>(IAddress address) where TSender : ISender;


        ISenderRouter AddFactory<TSender>(ISenderFactory factory) where TSender : ISender;


        /// <summary>
        /// Resolves an <see cref="ISender"/> for the type of the request with the configured routing
        /// </summary>
        /// <typeparam name="TRequest">Request type</typeparam>
        /// <returns>Sender for the request type</returns>
        ISender SenderFor<TRequest>();


        void Start();


        void Stop();
    }
}
