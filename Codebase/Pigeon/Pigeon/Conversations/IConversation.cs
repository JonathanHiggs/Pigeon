using System;
using System.Collections.Generic;
using System.Text;
using Pigeon.Protocol;

namespace Pigeon.Conversations
{
    /// <summary>
    /// Finite state machine
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IConversation<T> where T : ProtocolMessage
    {
        Guid Id { get; }

        void MoveNext(T message);
    }
}
