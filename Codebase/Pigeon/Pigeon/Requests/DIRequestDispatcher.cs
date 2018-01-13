using System;
using System.Threading.Tasks;

using Pigeon.Diagnostics;

namespace Pigeon.Requests
{
    /// <summary>
    /// Stores and resolves registered handlers for an incoming request to invoke to prepare responses and respond to
    /// </summary>
    public class DIRequestDispatcher : RequestDispatcher, IDIRequestDispatcher
    {
        private IContainer container;


        /// <summary>
        /// Initializes a new instance of <see cref="DIRequestDispatcher"/>
        /// </summary>
        /// <param name="container">Inner <see cref="IContainer"/> adapter around a third party IoC container</param>
        public DIRequestDispatcher(IContainer container)
        {
            this.container = container ?? throw new ArgumentNullException(nameof(container));
        }


        /// <summary>
        /// Registers a handler that will be resolved when needed
        /// </summary>
        /// <typeparam name="TRequest">Type of request message</typeparam>
        /// <typeparam name="TResponse">Type of response message</typeparam>
        /// <typeparam name="THandler">Type of handler</typeparam>
        public void Register<TRequest, TResponse, THandler>() where THandler : IRequestHandler<TRequest, TResponse>
        {
            ValidateTypes<TRequest, TResponse>();
            if (!container.IsRegistered<THandler>())
                throw new NotRegisteredException(typeof(THandler));

            requestHandlers.Add(typeof(TRequest), request => Task.Run(() => (object)container.Resolve<THandler>().Handle((TRequest)request)));
        }
    }
}
