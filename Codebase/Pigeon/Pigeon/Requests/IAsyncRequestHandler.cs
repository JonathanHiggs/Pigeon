using System.Threading.Tasks;

namespace Pigeon.Requests
{
    /// <summary>
    /// Interface for defining the handling the specified request type that gets
    /// registered to a <see cref="IRequestDispatcher"/>
    /// </summary>
    /// <typeparam name="TRequest">Type of request message</typeparam>
    /// <typeparam name="TResponse">Type of response message</typeparam>
    public interface IAsyncRequestHandler<TRequest, TResponse>
    {
        /// <summary>
        /// Dispatches the execution of a request message to return the response message
        /// </summary>
        /// <param name="request">Request message</param>
        /// <returns>Response message</returns>
        Task<TResponse> Handle(TRequest request);
    }
}
