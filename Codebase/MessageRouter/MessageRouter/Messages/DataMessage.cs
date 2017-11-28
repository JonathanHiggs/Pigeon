using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessageRouter.Messages
{
    /// <summary>
    /// Basic <see cref="Message"/> derivative that contains type safe data
    /// </summary>
    /// <typeparam name="T">Type of data payload</typeparam>
    [Serializable]
    [ImmutableObject(true)]
    public class DataMessage<T> : Message
        where T : class
    {
        /// <summary>
        /// Stores a readonly reference to the message data
        /// </summary>
        public readonly T Data;


        /// <summary>
        /// Gets the message body
        /// </summary>
        public override object Body => Data;


        /// <summary>
        /// Initializes an instance of DataMessage
        /// </summary>
        /// <param name="id">Message indentifer</param>
        /// <param name="data">Message body and data</param>
        public DataMessage(IMessageId id, T data)
            : base(id)
        {
            Data = data ?? throw new ArgumentNullException(nameof(data));
        }
    }
}
