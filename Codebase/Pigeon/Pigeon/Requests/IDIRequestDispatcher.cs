using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pigeon.Requests
{
    /// <summary>
    /// Takes incoming request messages and resolved a registered <see cref="IRequestHandler{TRequest, TResponse}"/>
    /// prepare a response message
    /// </summary>
    public interface IDIRequestDispatcher : IRequestDispatcher
    {
        /// <summary>
        /// Registers a handler that will be resolved when needed
        /// </summary>
        /// <typeparam name="TRequest">Type of request message</typeparam>
        /// <typeparam name="TResponse">Type of response message</typeparam>
        /// <typeparam name="THandler">Type of handler</typeparam>
        void Register<TRequest, TResponse, THandler>()
            where THandler : IRequestHandler<TRequest, TResponse>;
    }
}
