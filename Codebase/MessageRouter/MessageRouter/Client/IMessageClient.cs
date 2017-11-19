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
        /// <typeparam name="TRequest">Request type</typeparam>
        /// <typeparam name="TResponse">Expected reponse type</typeparam>
        /// <param name="request">Request object</param>
        /// <returns>Response object</returns>
        TResponse Send<TRequest, TResponse>(TRequest request)
            where TRequest : class
            where TResponse : class;


        Task<TResponse> SendAsync<TRequest, TResponse>(TRequest request, double timeout)
            where TRequest : class
            where TResponse : class;


        void Start();


        void Stop();
    }
}
