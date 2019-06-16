using System;

namespace Pigeon.Packages
{
    /// <summary>
    /// Package identifier
    /// </summary>
    [Serializable]
    public class GuidPackageId : IPackageId
    {
        private Guid guid;


        /// <summary>
        /// Initializes a new instance of <see cref="GuidPackageId"/>
        /// </summary>
        public GuidPackageId()
        {
            guid = Guid.NewGuid();
        }


        /// <summary>
        /// Initializes a new instance of <see cref="GuidPackageId"/>
        /// </summary>
        /// <param name="guid"></param>
        public GuidPackageId(Guid guid)
        {
            this.guid = guid;
        }


        /// <summary>
        /// Indicates whether the current object is equal to another <see cref="IPackageId"/>
        /// </summary>
        /// <param name="other">An object to compare with this</param>
        /// <returns>true if the current object is equal to the other <see cref="IPackageId"/>; otherwise, false</returns>
        public bool Equals(IPackageId other)
        {
            var guidMessageId = other as GuidPackageId;

            if (guidMessageId is null) return false;

            return guidMessageId.guid == guid;
        }
    }
}
