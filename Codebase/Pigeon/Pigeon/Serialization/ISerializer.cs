namespace Pigeon.Serialization
{
    /// <summary>
    /// Interface defining transforming objects to data and back
    /// </summary>
    public interface ISerializer
    {
        /// <summary>
        /// Gets the serialization description
        /// </summary>
        SerializationDescriptor Descriptor { get; }


        /// <summary>
        /// Transforms serialized data back to the requested object type
        /// </summary>
        /// <typeparam name="TObj">Type of object to reconstruct</typeparam>
        /// <param name="data">Data to be deserialized</param>
        /// <returns>Deserialized object</returns>
        TObj Deserialize<TObj>(byte[] data);


        /// <summary>
        /// Transforms serialized data back to the requested object type
        /// </summary>
        /// <typeparam name="TObj">Type of object to reconstruct</typeparam>
        /// <param name="data">Data to be deserialized</param>
        /// <param name="offset">Offset to start reading from</param>
        /// <returns>Deserialized object</returns>
        TObj Deserialize<TObj>(byte[] data, int offset);


        /// <summary>
        /// Transforms the supplied object to the serializer output data type
        /// </summary>
        /// <typeparam name="TObj">Type of object to be serialized</typeparam>
        /// <param name="obj">Object instance to be serialized</param>
        /// <returns>Serialized data</returns>
        byte[] Serialize<TObj>(TObj obj);


        /// <summary>
        /// Transforms the supplied object ot the serializer output data type with an offset
        /// </summary>
        /// <typeparam name="TObj">Type of object to be serialized</typeparam>
        /// <param name="obj">Object instance to be serialized</param>
        /// <param name="offset">Data offset</param>
        /// <returns>Serialized data</returns>
        byte[] Serialize<TObj>(TObj obj, int offset);
    }
}
