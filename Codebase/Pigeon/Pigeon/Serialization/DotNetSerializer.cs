using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace Pigeon.Serialization
{
    /// <summary>
    /// Default data serializer to transform object to byte arrays
    /// Uses .Net interal BinaryFormatter and requires all types have the Serializable attribute
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

        public object Deserialize(byte[] data, int offset)
        {
            using (var stream = new MemoryStream(data, offset, data.Length - offset))
            {
                stream.Seek(0, SeekOrigin.Begin);
                return binaryFormatter.Deserialize(stream);
            }
        }


        /// <summary>
        /// Transforms serialized data back to the requested object type
        /// </summary>
        /// <typeparam name="TObj">Type of object to reconstruct</typeparam>
        /// <param name="data">Data to be deserialized</param>
        /// <returns>Deserialized object</returns>
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


        public byte[] Serialize(object obj, int offset)
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
