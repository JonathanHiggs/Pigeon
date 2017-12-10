using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessageRouter.Messages
{
    /// <summary>
    /// <see cref="Message"/> derivative for returning an exception in a remote process
    /// </summary>
    [Serializable]
    [ImmutableObject(true)]
    public class ExceptionMessage : Message
    {
        /// <summary>
        /// Gets the exception thrown by the remote process
        /// </summary>
        public readonly Exception Exception;


        /// <summary>
        /// Initializes a new instance of <see cref="ExceptionMessage{T}"/>
        /// </summary>
        /// <param name="id">Message identifier</param>
        /// <param name="exception">Exception thrown by the remote process</param>
        public ExceptionMessage(IMessageId id, Exception exception)
            : base(id)
        {
            Exception = exception ?? throw new ArgumentNullException(nameof(exception));
        }


        /// <summary>
        /// Gets the message body
        /// </summary>
        public override object Body => Exception;
    }
}
