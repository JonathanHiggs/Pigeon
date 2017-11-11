using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessageRouter.Diagnostics
{
    /// <summary>
    /// Base class for all MessageRouter runtime exceptions
    /// </summary>
    [Serializable]
    public abstract class MessageRouterException : Exception
    {
        public MessageRouterException() { }
        public MessageRouterException(string message) : base(message) { }
        public MessageRouterException(string message, Exception inner) : base(message, inner) { }
        protected MessageRouterException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}
