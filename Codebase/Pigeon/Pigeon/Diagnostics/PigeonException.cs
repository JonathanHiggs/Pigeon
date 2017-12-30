using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pigeon.Diagnostics
{
    /// <summary>
    /// Base class for all Pigeon runtime exceptions
    /// </summary>
    [Serializable]
    public abstract class PigeonException : Exception
    {
        public PigeonException() { }
        public PigeonException(string message) : base(message) { }
        public PigeonException(string message, Exception inner) : base(message, inner) { }
        protected PigeonException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}
