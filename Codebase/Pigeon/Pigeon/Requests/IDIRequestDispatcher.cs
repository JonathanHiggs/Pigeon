using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pigeon.Requests
{
    public interface IDIRequestDispatcher : IRequestDispatcher
    {
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TRequest"></typeparam>
        /// <typeparam name="TResponse"></typeparam>
        /// <typeparam name="THandler"></typeparam>
        void Register<TRequest, TResponse, THandler>()
            where THandler : IRequestHandler<TRequest, TResponse>;
    }
}
