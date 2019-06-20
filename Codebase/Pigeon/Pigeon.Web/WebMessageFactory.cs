using System;
using System.Net;
using System.Text;
using System.Threading.Tasks;

using Pigeon.Serialization;

namespace Pigeon.Web
{
    public class WebMessageFactory : IWebMessageFactory
    {
        private readonly ISerializer serializer;
        private readonly IRoutingTable routingTable;



        public object ExtractRequestMessage(HttpListenerRequest request)
        {
            if (!routingTable.TryGetRouting(request, out var routing))
                throw new Exception();

            var requestMessage = routing.ExtractRequest(request, serializer);

            return requestMessage;
        }


        public bool IsValidRequest(HttpListenerRequest request) => true;


        public async Task SetResponseMessage(HttpListenerResponse response, object message)
        {
            var data = serializer.Serialize(message);

            response.StatusCode = (int)HttpStatusCode.OK;
            response.ContentEncoding = Encoding.UTF8;
            response.ContentType = "application/json";
            response.ContentLength64 = data.Length;

            await response.OutputStream.WriteAsync(data, 0, data.Length);

            response.Close();
        }
    }
}
