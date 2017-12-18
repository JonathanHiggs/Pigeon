namespace MessageRouter.Serialization
{
    /// <summary>
    /// A runtime cache of <see cref="ISerializer"/>s to register and retrieve from
    /// </summary>
    public interface ISerializerCache
    {
        /// <summary>
        /// Adds the specified <see cref="ISerializer"/> to the cache
        /// </summary>
        /// <param name="serializer">The <see cref="ISerializer"/> to add to the cache</param>
        void AddSerializer(ISerializer serializer);

        
        /// <summary>
        /// Trys to retrieve a <see cref="ISerializer"/> from the cache where the <see cref="ISerializer.Descriptor"/>'s name matches the supplied name
        /// </summary>
        /// <param name="name">The name of the <see cref="ISerializer"/> to retrieve from the cache</param>
        /// <param name="serializer"></param>
        /// <returns>true if the cache contains a matching <see cref="ISerializer"/>; false otherwise</returns>
        bool SerializerFor(string name, out ISerializer serializer);
    }
}