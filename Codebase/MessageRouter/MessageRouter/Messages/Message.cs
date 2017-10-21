using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessageRouter.Messages
{
    /// <summary>
    /// Base class for the message protocol
    /// </summary>
    [Serializable]
    [ImmutableObject(true)]
    public abstract class Message
    {
        /// <summary>
        /// Stores a readonly reference to a unique identifier for the message
        /// </summary>
        public readonly IMessageId Id;


        /// <summary>
        /// Initializes a new instance of a Message
        /// </summary>
        /// <param name="id">Message indentifier</param>
        public Message(IMessageId id)
        {
            Id = id ?? throw new ArgumentNullException(nameof(id));
        }


        /// <summary>
        /// Gets the message body
        /// </summary>
        public abstract object Body { get; }
    }
}
