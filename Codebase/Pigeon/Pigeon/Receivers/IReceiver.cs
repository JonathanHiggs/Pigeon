using Pigeon.Addresses;
using Pigeon.Common;

namespace Pigeon.Receivers
{
    /// <summary>
    /// Interface encapsulates a connection that is able to bind to <see cref="IAddress"/> to receive and reply 
    /// to incoming messages from remote <see cref="ISender"/>
    /// </summary>
    public interface IReceiver : IConnection
    { }
}
