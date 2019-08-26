using System;
using System.IO;

namespace Pigeon.Protocol.Pigeon
{
    public class PigeonDeserializer : IProtocolDeserializer<PigeonMessage>
    {
        /// <summary>
        /// Gets the protocol root type
        /// </summary>
        public Type ProtocolType => typeof(PigeonMessage);


        /// <summary>
        /// Initializes a new instance of <see cref="PigeonMessage"/> from serialized data
        /// </summary>
        /// <param name="data">Binary data to de-serialize</param>
        /// <returns></returns>
        public PigeonMessage ReadFrom(byte[] data)
        {
            using (var stream = new MemoryStream(data))
                return ReadFrom(stream);
        }


        /// <summary>
        /// Initializes a new instance of <see cref="PigeonMessage"/> from serialized data
        /// </summary>
        /// <param name="stream">Data stream to de-serialize</param>
        /// <returns></returns>
        public PigeonMessage ReadFrom(Stream stream)
        {
            using (var reader = new BinaryReader(stream))
                return ReadFrom(reader);
        }


        /// <summary>
        /// Initializes a new instance of <see cref="PigeonMessage"/> from serialized data
        /// </summary>
        /// <param name="reader">Data reader to de-serialize</param>
        /// <returns></returns>
        public PigeonMessage ReadFrom(BinaryReader reader) =>
            PigeonMessage.ReadFrom(reader);
    }
}
