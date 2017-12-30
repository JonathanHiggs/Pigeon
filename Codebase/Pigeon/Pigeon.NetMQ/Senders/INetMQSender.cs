using Pigeon.Addresses;
using Pigeon.NetMQ.Common;
using Pigeon.Receivers;
using Pigeon.Senders;
using NetMQ;

namespace Pigeon.NetMQ.Senders
{
    /// <summary>
    /// Interface that encapsulates a NetMQ <see cref="ISender"/> that is able to connect to a remote
    /// <see cref="IAddress"/> to send and receive response messages from a remote <see cref="IReceiver"/>
    /// </summary>
    public interface INetMQSender : ISender, INetMQConnection
    { }
}