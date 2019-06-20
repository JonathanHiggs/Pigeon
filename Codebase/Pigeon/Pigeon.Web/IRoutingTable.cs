using System.Net;

namespace Pigeon.Web
{
    public interface IRoutingTable
    {
        WebRouting GetRouting(HttpListenerRequest request);

        bool TryGetRouting(HttpListenerRequest request, out WebRouting routing);
    }
}
