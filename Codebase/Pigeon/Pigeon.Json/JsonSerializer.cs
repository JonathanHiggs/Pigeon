using System.IO;

using Newtonsoft.Json;

using Pigeon.Serialization;

using NewtonsoftJsonSerializer = Newtonsoft.Json.JsonSerializer;

namespace Pigeon.Json
{
    /// <summary>
    /// Wrapper around Newtonsoft.Json serializer
    /// </summary>
    public class JsonSerializer : ISerializer
    {
        /// <summary>
        /// Gets the serialization description
        /// </summary>
        public SerializationDescriptor Descriptor => SerializationDescriptor.Json;


        /// <summary>
        /// Gets the serialization settings
        /// </summary>
        public JsonSerializerSettings Settings { get; } = 
            new JsonSerializerSettings {
                TypeNameHandling = TypeNameHandling.All,
            };


        /// <summary>
        /// Transforms serialized data back to the requested object type
        /// </summary>
        /// <typeparam name="TObj">Type of object to reconstruct</typeparam>
        /// <param name="data">Data to be de-serialized</param>
        /// <returns>De-serialized object</returns>
        public TObj Deserialize<TObj>(byte[] data)
        {
            using (var stream = new MemoryStream(data))
            using (var reader = new StreamReader(stream))
            using (var jsonReader = new JsonTextReader(reader))
            {
                return NewtonsoftJsonSerializer.Create(Settings).Deserialize<TObj>(jsonReader);
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
            using (var reader = new StreamReader(stream))
            using (var jsonReader = new JsonTextReader(reader))
            {
                return NewtonsoftJsonSerializer.Create(Settings).Deserialize<TObj>(jsonReader);
            }
        }


        /// <summary>
        /// Transforms the supplied object to the serializer output data type
        /// </summary>
        /// <typeparam name="TObj">Type of object to be serialized</typeparam>
        /// <param name="obj">Object instance to be serialized</param>
        /// <returns>Serialized data</returns>
        public byte[] Serialize<TObj>(TObj obj)
        {
            using (var stream = new MemoryStream())
            using (var writer = new StreamWriter(stream))
            using (var jsonWriter = new JsonTextWriter(writer))
            {
                NewtonsoftJsonSerializer.Create(Settings).Serialize(writer, obj);
                jsonWriter.Flush();
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
            using (var writer = new StreamWriter(stream))
            using (var jsonWriter = new JsonTextWriter(writer))
            {
                stream.Seek(offset, SeekOrigin.Begin);
                NewtonsoftJsonSerializer.Create(Settings).Serialize(writer, obj);
                jsonWriter.Flush();
                return stream.ToArray();
            }
        }
    }
}
