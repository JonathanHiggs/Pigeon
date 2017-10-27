using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessageRouter.Client
{
    public interface IMessageClient
    {
        /// <summary>
        /// Dispatches a request to a remote routed by the <see cref="ISenderManager"/>
        /// </summary>
        /// <typeparam name="TResponse">Expected reponse type</typeparam>
        /// <typeparam name="TRequest">Request type</typeparam>
        /// <param name="request">Request object</param>
        /// <returns>Response object</returns>
        TResponse Send<TResponse, TRequest>(TRequest request)
            where TRequest : class
            where TResponse : class;


        /// <summary>
        /// Initializes connections to remotes
        /// </summary>
        void Start();


        /// <summary>
        /// Terminates connections to remotes
        /// </summary>
        void Stop();
    }
}
