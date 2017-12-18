using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessageRouter.Serialization
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
        /// Transforms the supplied object to the serializer output data type
        /// </summary>
        /// <typeparam name="TObj">Type of object to be serialized</typeparam>
        /// <param name="obj">Object to be serialized</param>
        /// <returns>Serialized data</returns>
        byte[] Serialize<TObj>(TObj obj);


        /// <summary>
        /// Transforms serialized data back to the requested object type
        /// </summary>
        /// <typeparam name="TObj">Type of object to reconstruct</typeparam>
        /// <param name="data">Data to be deserialized</param>
        /// <returns>Deserialized object</returns>
        TObj Deserialize<TObj>(byte[] data);
    }
}
