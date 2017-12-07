using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessageRouter.Messages
{
    /// <summary>
    /// Message identifier
    /// </summary>
    [Serializable]
    public class GuidMessageId : IMessageId
    {
        private Guid guid;


        /// <summary>
        /// Initializes a new instance of GuidMessageId and generates a random inner <see cref="Guid"/>
        /// </summary>
        public GuidMessageId()
        {
            guid = Guid.NewGuid();
        }


        /// <summary>
        /// Initializes a new instance of GuidMessageId with the supplied <see cref="Guid"/>
        /// </summary>
        /// <param name="guid"></param>
        public GuidMessageId(Guid guid)
        {
            this.guid = guid;
        }


        /// <summary>
        /// Indicates whether the current object is equal to another <see cref="IMessageId"/>
        /// </summary>
        /// <param name="other">An object to compare with this</param>
        /// <returns>true if the current object is equal to the other <see cref="IMessage"/>; otherwise, false</returns>
        public bool Equals(IMessageId other)
        {
            var guidMessageId = other as GuidMessageId;

            if (null == guidMessageId) return false;

            return guidMessageId.guid == guid;
        }
    }
}
