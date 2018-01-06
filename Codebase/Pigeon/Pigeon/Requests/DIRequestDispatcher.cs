using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pigeon.Requests
{
    public class DIRequestDispatcher : RequestDispatcher, IDIRequestDispatcher
    {
        private IContainer container;


        public DIRequestDispatcher(IContainer container)
        {
            this.container = container ?? throw new ArgumentNullException(nameof(container));
        }


        public void Register<TRequest, TResponse, THandler>() where THandler : IRequestHandler<TRequest, TResponse>
        {
            ValidateTypes<TRequest, TResponse>();
            if (!container.IsRegistered<THandler>())
                throw new NotReigsteredException(typeof(THandler));

            requestHandlers.Add(typeof(TRequest), request => Task.Run(() => (object)container.Resolve<THandler>().Handle((TRequest)request)));
        }
    }
}
