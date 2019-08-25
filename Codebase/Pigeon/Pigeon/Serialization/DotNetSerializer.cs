using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace Pigeon.Serialization
{
    /// <summary>
    /// Default data serializer to transform object to byte arrays
    /// Uses .Net internal BinaryFormatter and requires all types have the Serializable attribute
    /// </summary>
    public class DotNetSerializer : ISerializer
    {
        private readonly BinaryFormatter binaryFormatter = new BinaryFormatter();


        /// <summary>
        /// Initializes a new instance of <see cref="DotNetSerializer"/>
        /// </summary>
        public DotNetSerializer()
        { }

        
        /// <summary>
        /// Gets the serialization description
        /// </summary>
        public SerializationDescriptor Descriptor => SerializationDescriptor.DotNet;


        /// <summary>
        /// Transforms the supplied object to the serializer output data type
        /// </summary>
        /// <typeparam name="TObj">Type of object to be serialized</typeparam>
        /// <param name="obj">Object to be serialized</param>
        /// <returns>Serialized data</returns>
        public TObj Deserialize<TObj>(byte[] data)
        {
            using (var stream = new MemoryStream(data))
            {
                stream.Seek(0, SeekOrigin.Begin);
                return (TObj)binaryFormatter.Deserialize(stream);
            }
        }


        /// <summary>
        /// Transforms serialized data back to the requested object type
        /// </summary>
        /// <typeparam name="TObj">Type of object to reconstruct</typeparam>
        /// <param name="data">Data to be de-serialized</param>
        /// <param name="offset">Offset to start reading from</param>
        /// <returns>De-serialized object</returns>
        public TObj Deserialize<TObj>(byte[] data, int offset)
        {
            using (var stream = new MemoryStream(data, offset, data.Length - offset))
            {
                stream.Seek(0, SeekOrigin.Begin);
                return (TObj)binaryFormatter.Deserialize(stream);
            }
        }


        /// <summary>
        /// Transforms serialized data back to the requested object type
        /// </summary>
        /// <typeparam name="TObj">Type of object to reconstruct</typeparam>
        /// <param name="data">Data to be de-serialized</param>
        /// <returns>De-serialized object</returns>
        public byte[] Serialize<TObj>(TObj obj)
        {
            using (var stream = new MemoryStream())
            {
                binaryFormatter.Serialize(stream, obj);
                stream.Flush();
                stream.Seek(0, SeekOrigin.Begin);
                return stream.ToArray();
            }
        }


        /// <summary>
        /// Transforms the supplied object to the serializer output data type with an offset
        /// </summary>
        /// <typeparam name="TObj">Type of object to be serialized</typeparam>
        /// <param name="obj">Object instance to be serialized</param>
        /// <param name="offset">Data offset</param>
        /// <returns>Serialized data</returns>
        public byte[] Serialize<TObj>(TObj obj, int offset)
        {
            using (var stream = new MemoryStream())
            {
                stream.Seek(offset, SeekOrigin.Begin);
                binaryFormatter.Serialize(stream, obj);
                stream.Flush();
                stream.Seek(0, SeekOrigin.Begin);
                return stream.ToArray();
            }
        }
    }
}
