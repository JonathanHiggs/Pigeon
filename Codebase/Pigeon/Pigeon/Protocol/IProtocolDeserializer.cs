using System;
using System.IO;

namespace Pigeon.Protocol
{
    /// <summary>
    /// Exposes the ability to de-serialize a <see cref="ProtocolMessage"/> from binary data
    /// </summary>
    public interface IProtocolDeserializer<T> where T : ProtocolMessage
    {
        /// <summary>
        /// Gets the protocol root type
        /// </summary>
        Type ProtocolType { get; }


        /// <summary>
        /// Initializes a new instance of <see cref="ProtocolMessage"/> from serialized data
        /// </summary>
        /// <param name="data">Binary data to de-serialize</param>
        /// <returns></returns>
        T ReadFrom(byte[] data);


        /// <summary>
        /// Initializes a new instance of <see cref="ProtocolMessage"/> from serialized data
        /// </summary>
        /// <param name="stream">Data stream to de-serialize</param>
        /// <returns></returns>
        T ReadFrom(Stream stream);


        /// <summary>
        /// Initializes a new instance of <see cref="ProtocolMessage"/> from serialized data
        /// </summary>
        /// <param name="reader">Data reader to de-serialize</param>
        /// <returns></returns>
        T ReadFrom(BinaryReader reader);
    }
}
