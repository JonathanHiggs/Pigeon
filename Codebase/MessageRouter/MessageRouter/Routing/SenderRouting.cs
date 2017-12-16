using System;

using MessageRouter.Addresses;
using MessageRouter.Receivers;
using MessageRouter.Senders;

namespace MessageRouter.Routing
{
    /// <summary>
    /// Attaches a <see cref="ISender"/> type to a remote <see cref="IAddress"/> for runtime transport resolution
    /// </summary>
    public struct SenderRouting
    {
        /// <summary>
        /// Transport specifc type of the local <see cref="ISender"/>
        /// </summary>
        public readonly Type SenderType;


        /// <summary>
        /// Remote <see cref="IAddress"/> for the remote <see cref="IReceiver"/>
        /// </summary>
        public readonly IAddress Address;


        /// <summary>
        /// Initializes a new instance of a <see cref="SenderRouting"/>
        /// </summary>
        /// <param name="senderType">Transport specific type of the local <see cref="ISender"/></param>
        /// <param name="address"><see cref="IAddress"/> of the remote <see cref="IReceiver"/></param>
        private SenderRouting(Type senderType, IAddress address)
        {
            SenderType = senderType ?? throw new ArgumentNullException(nameof(senderType));
            Address = address ?? throw new ArgumentNullException(nameof(address));
        }


        /// <summary>
        /// Initializes a new instance of a <see cref="SenderRouting"/>
        /// </summary>
        /// <typeparam name="TSender">Transport specific type of the local <see cref="ISender"/></typeparam>
        /// <param name="address"><see cref="IAddress"/> of the remote <see cref="IReceiver"/></param>
        /// <returns></returns>
        public static SenderRouting For<TSender>(IAddress address)
            where TSender : ISender
        {
            return new SenderRouting(typeof(TSender), address);
        }


        /// <summary>
        /// Converts the <see cref="SenderRouting"/> to a string representation
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return $"{SenderType.Name} -> {Address.ToString()}";
        }
    }
}
