using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessageRouter.Messages
{
    /// <summary>
    /// Interface for indentifying a message
    /// </summary>
    public interface IMessageId : IEquatable<IMessageId>
    { }
}
