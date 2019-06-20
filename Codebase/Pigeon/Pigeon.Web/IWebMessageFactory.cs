using System.Net;
using System.Threading.Tasks;

namespace Pigeon.Web
{
    public interface IWebMessageFactory
    {
        bool IsValidRequest(HttpListenerRequest request);

        object ExtractRequestMessage(HttpListenerRequest request);

        Task SetResponseMessage(HttpListenerResponse response, object message);
    }
}
