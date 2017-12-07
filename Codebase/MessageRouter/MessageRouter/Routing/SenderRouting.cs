using MessageRouter.Addresses;
using MessageRouter.Senders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessageRouter.Routing
{
    /// <summary>
    /// Combines a <see cref="ISender"/> type and the remote address for it
    /// </summary>
    public struct SenderRouting
    {
        /// <summary>
        /// Type of the <see cref="ISender"/>
        /// </summary>
        public readonly Type SenderType;


        /// <summary>
        /// Remote address for the <see cref="ISender"/>
        /// </summary>
        public readonly IAddress Address;


        /// <summary>
        /// Initializes a new instance of a SenderRouting
        /// </summary>
        /// <param name="senderType">Type of the <see cref="ISender"/></param>
        /// <param name="address">Address of the remote</param>
        private SenderRouting(Type senderType, IAddress address)
        {
            SenderType = senderType;
            Address = address;
        }


        /// <summary>
        /// Initializes a new instance of a SenderRouting
        /// </summary>
        /// <typeparam name="TSender">Type of the <see cref="ISender"/></typeparam>
        /// <param name="address">Address of the remote</param>
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
