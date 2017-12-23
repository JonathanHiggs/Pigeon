using System.Net;
using System.Threading.Tasks;

namespace Pigeon.Web
{
    public delegate Task WebTaskHandler(IWebReceiver receiver, HttpListenerContext context);
}
