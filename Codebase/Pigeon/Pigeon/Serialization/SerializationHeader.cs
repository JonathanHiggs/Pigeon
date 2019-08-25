using System;
using System.Text;

namespace Pigeon.Serialization
{
    /// <summary>
    /// Data structure of the first few bytes in a pigeon message, used to denote protocol version and serialization used in the rest of the message
    /// </summary>
    /// <remarks>
    /// Byte-wise representation:
    /// 0-3     Pigeon magic bytes
    /// 4       Protocol version major byte
    /// 5       Protocol version minor byte
    /// 6       Length of the invariant name string
    /// 7+      Invariant string
    /// </remarks>
    public readonly struct SerializationHeader
    {
        // magic bytes are used prepended to the message to identify the data as a Pigeon message
        private const string magic = "PIGE";
        private static readonly byte[] magicBytes = Encoding.UTF8.GetBytes(magic);


        /// <summary>
        /// Initializes a new instance of <see cref="SerializationHeader"/>
        /// </summary>
        /// <param name="protocol">Protocol version number</param>
        /// <param name="serializationInvariantName">Invariant name of the <see cref="ISerializer"/> used in the rest of the message</param>
        public SerializationHeader(ProtocolVersion protocol, string serializationInvariantName)
        {
            Protocol = protocol;
            InvariantName = serializationInvariantName;
            InvariantLength = Encoding.UTF8.GetByteCount(InvariantName);
            EncodedLength = 7 + InvariantLength;
        }


        /// <summary>
        /// Gets the length of the header once encoded
        /// </summary>
        public int EncodedLength { get; }


        /// <summary>
        /// Gets the pigeon protocol version number
        /// </summary>
        public ProtocolVersion Protocol { get; }


        /// <summary>
        /// Gets the length of <see cref="InvariantName"/> in UTF8 encoding
        /// </summary>
        public int InvariantLength { get; }


        /// <summary>
        /// Gets the <see cref="ISerializer"/> invariant name
        /// </summary>
        public string InvariantName { get; }


        /// <summary>
        /// Decodes a <see cref="SerializationHeader"/> from the supplied byte array
        /// </summary>
        /// <param name="array"></param>
        /// <returns>Decoded <see cref="SerializationHeader"/></returns>
        public static SerializationHeader FromBytes(byte[] array)
        {
            // Check the message has a Pigeon header
            for (int i = 0; i < 4; i++)
                if (array[i] != magicBytes[i])
                    return new SerializationHeader(new ProtocolVersion(0, 0), string.Empty);

            var stringSize = (ushort)array[6];

#if DEBUG
            if (7 + stringSize > array.Length)
                throw new IndexOutOfRangeException("Serialization header invariant name longer than the supplied array");
#endif

            var version = new ProtocolVersion(array[4], array[5]);
            var serializationName = Encoding.UTF8.GetString(array, 7, stringSize);

            return new SerializationHeader(version, serializationName);
        }


        /// <summary>
        /// Encodes the <see cref="SerializationHeader"/> into a byte array
        /// </summary>
        /// <returns>Byte array containing encoded <see cref="SerializationHeader"/></returns>
        public byte[] ToBytes()
        {
            var array = new byte[EncodedLength];
            ToBytes(array);
            return array;
        }


        /// <summary>
        /// Encodes the <see cref="SerializationHeader"/> into the supplied byte array
        /// </summary>
        /// <param name="array">Byte array containing encoded <see cref="SerializationHeader"/></param>
        public void ToBytes(byte[] array)
        {
            if (array.Length < InvariantLength)
                throw new ArgumentException("Target array is too small");

            Array.Copy(magicBytes, 0, array, 0, 4);
            array[4] = Protocol.Major;
            array[5] = Protocol.Minor;
            array[6] = (byte)(ushort)InvariantLength;
            Encoding.UTF8.GetBytes(InvariantName, 0, InvariantLength, array, 7);
        }
    }
}
