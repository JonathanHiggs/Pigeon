using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessageRouter.Messages
{
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


        public bool Equals(IMessageId other)
        {
            var guidMessageId = other as GuidMessageId;

            if (null == guidMessageId) return false;

            return guidMessageId.guid == guid;
        }
    }
}
