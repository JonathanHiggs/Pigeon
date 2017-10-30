using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessageRouter.Senders
{
    [Serializable]
    public class SenderNotRegisteredException<TRequest> : Exception
    {
        public SenderNotRegisteredException() { }
        public SenderNotRegisteredException(string message) : base(message) { }
        public SenderNotRegisteredException(string message, Exception inner) : base(message, inner) { }
        protected SenderNotRegisteredException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}
