using System;
using System.Collections.Generic;
using System.Linq;

namespace Pigeon.Serialization
{
    /// <summary>
    /// A runtime cache of <see cref="ISerializer"/>s to register and retrieve from
    /// </summary>
    public class SerializerCache : ISerializerCache
    {
        private HashSet<ISerializer> serializers = new HashSet<ISerializer>(SerializerComparer.Default);


        /// <summary>
        /// Gets the default serializer to use
        /// </summary>
        public ISerializer DefaultSerializer { get; private set; }


        /// <summary>
        /// Gets an enumeration of all <see cref="ISerializer"/>s in the cache
        /// </summary>
        public IEnumerable<ISerializer> AllSerializers => serializers;


        /// <summary>
        /// Adds the specified <see cref="ISerializer"/> to the cache
        /// </summary>
        /// <param name="serializer">The <see cref="ISerializer"/> to add to the cache</param>
        public void AddSerializer(ISerializer serializer)
        {
            if (serializer is null)
                throw new ArgumentNullException(nameof(serializer));

            if (serializers.Count == 0)
                DefaultSerializer = serializer;

            serializers.Add(serializer);
        }


        /// <summary>
        /// Tries to retrieve a <see cref="ISerializer"/> from the cache where the <see cref="ISerializer.Descriptor"/>'s name matches the supplied name
        /// </summary>
        /// <param name="name">The name of the <see cref="ISerializer"/> to retrieve from the cache</param>
        /// <param name="serializer">Out <see cref="ISerializer"/></param>
        /// <returns>true if the cache contains a matching <see cref="ISerializer"/>; false otherwise</returns>
        public bool SerializerFor(string name, out ISerializer serializer)
        {
            serializer = serializers.SingleOrDefault(s => s.Descriptor.InvariantName == name);
            return !(serializer is null);
        }


        /// <summary>
        /// Tries to retrieve a <see cref="ISerializer"/> from the cache where the <see cref="ISerializer.Descriptor"/>'s name matches the supplied name
        /// </summary>
        /// <param name="descriptor"><see cref="SerializationDescriptor"/> of the <see cref="ISerializer"/> to retrieve from the cache</param>
        /// <param name="serializer">Out <see cref="ISerializer"/></param>
        /// <returns>true if the cache contains a matching <see cref="ISerializer"/>; false otherwise</returns>
        public bool SerializerFor(SerializationDescriptor descriptor, out ISerializer serializer)
        {
            serializer = serializers.SingleOrDefault(s => s.Descriptor == descriptor);
            return !(serializer is null);
        }


        /// <summary>
        /// Sets the default serialization
        /// </summary>
        /// <param name="serializer"><see cref="ISerializer"/> to use by default</param>
        public void SetDefaultSerializer(ISerializer serializer)
        {
            if (!serializers.Contains(serializer))
                AddSerializer(serializer);

            DefaultSerializer = serializer;
        }


        /// <summary>
        /// Sets the default serialization
        /// </summary>
        /// <param name="serializationDescriptor"><see cref="SerializationDescriptor"/> of the <see cref="ISerializer"/> to use as the default</param>
        public void SetDefaultSerializer(SerializationDescriptor serializationDescriptor)
        {
            var serializer = serializers.SingleOrDefault(s => s.Descriptor == serializationDescriptor);

            if (serializer is null)
                throw new InvalidOperationException($"No matching serializer for {serializationDescriptor.InvariantName} found in the cache");

            DefaultSerializer = serializer;
        }
    }
}
