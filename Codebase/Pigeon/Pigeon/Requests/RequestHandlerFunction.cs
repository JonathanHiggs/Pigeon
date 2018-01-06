using System.Threading.Tasks;

namespace Pigeon.Requests
{
    /// <summary>
    /// Delegate for handling responses to requests
    /// </summary>
    /// <param name="request">Request message</param>
    /// <returns>Response message</returns>
    public delegate Task<object> RequestHandlerFunction(object request);
}
