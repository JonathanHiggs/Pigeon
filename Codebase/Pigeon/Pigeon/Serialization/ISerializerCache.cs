using System.Collections.Generic;

namespace Pigeon.Serialization
{
    /// <summary>
    /// A runtime cache of <see cref="ISerializer"/>s to register and retrieve from
    /// </summary>
    public interface ISerializerCache
    {
        /// <summary>
        /// Gets the default serializer to use
        /// </summary>
        ISerializer DefaultSerializer { get; }


        /// <summary>
        /// Gets an enumeration of all <see cref="ISerializer"/>s in the cache
        /// </summary>
        IEnumerable<ISerializer> AllSerializers { get; }


        /// <summary>
        /// Adds the specified <see cref="ISerializer"/> to the cache
        /// </summary>
        /// <param name="serializer">The <see cref="ISerializer"/> to add to the cache</param>
        void AddSerializer(ISerializer serializer);

        
        /// <summary>
        /// Tries to retrieve a <see cref="ISerializer"/> from the cache where the <see cref="ISerializer.Descriptor"/>'s name matches the supplied name
        /// </summary>
        /// <param name="name">The name of the <see cref="ISerializer"/> to retrieve from the cache</param>
        /// <param name="serializer"></param>
        /// <returns>true if the cache contains a matching <see cref="ISerializer"/>; false otherwise</returns>
        bool SerializerFor(string name, out ISerializer serializer);


        /// <summary>
        /// Tries to retrieve a <see cref="ISerializer"/> from the cache where the <see cref="ISerializer.Descriptor"/>'s name matches the supplied name
        /// </summary>
        /// <param name="serializationDescriptor">The name of the <see cref="ISerializer"/> to retrieve from the cache</param>
        /// <param name="serializer"></param>
        /// <returns>true if the cache contains a matching <see cref="ISerializer"/>; false otherwise</returns>
        bool SerializerFor(SerializationDescriptor serializationDescriptor, out ISerializer serializer);


        /// <summary>
        /// Sets the default serialization
        /// </summary>
        /// <param name="serializer"><see cref="ISerializer"/> to use by default</param>
        void SetDefaultSerializer(ISerializer serializer);


        /// <summary>
        /// Sets the default serialization
        /// </summary>
        /// <param name="serializationDescriptor"><see cref="SerializationDescriptor"/> of the <see cref="ISerializer"/> to use as the default</param>
        void SetDefaultSerializer(SerializationDescriptor serializationDescriptor);
    }
}