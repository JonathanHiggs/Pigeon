using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessageRouter.Diagnostics
{

    [Serializable]
    public class RequestNotMappedException : MessageRouterException
    {
        private readonly Type requestType;


        public Type RequestType => requestType;


        public RequestNotMappedException(Type requestType)
            : this(requestType, $"No request mapped for {requestType.Name}", null)
        { }


        public RequestNotMappedException(Type requestType, string message) 
            : this(requestType, message, null)
        { }


        public RequestNotMappedException(Type requestType, string message, Exception inner) 
            : base(message, inner)
        {
            this.requestType = requestType;
        }


        protected RequestNotMappedException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}
