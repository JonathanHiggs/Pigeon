using System;
using System.Text;

namespace Pigeon.Serialization
{
    public class SerializationHeader
    {
        private const string magic = "Pige";
        private static readonly byte[] magicBytes = Encoding.UTF8.GetBytes(magic);

        private readonly ProtocolVersion protocol;
        private readonly string serializationName;


        public SerializationHeader(ProtocolVersion protocol, string serializationName)
        {
            this.protocol = protocol;
            this.serializationName = serializationName;
        }


        public ProtocolVersion Protocol => protocol;


        public String SerializationName => serializationName;


        public static (SerializationHeader header, int byteCount) FromBytes(byte[] array)
        {
            // Check the message has a Pigeon header
            for (int i = 0; i < 4; i++)
                if (array[i] != magicBytes[i])
                    return (null, 0);

            var version = new ProtocolVersion
            {
                Major = array[4],
                Minor = array[5]
            };

            var stringSize = (ushort)array[6];
            var serializationName = Encoding.UTF8.GetString(array, 7, stringSize);

            return (new SerializationHeader(version, serializationName), 7 + stringSize);
        }


        public byte[] ToBytes()
        {
            var length = Encoding.UTF8.GetByteCount(serializationName);
            var array = new byte[length + 7];

            ToBytes(array, length);

            return array;
        }


        public void ToBytes(byte[] array)
        {
            var length = Encoding.UTF8.GetByteCount(serializationName);
            ToBytes(array, length);
        }


        private void ToBytes(byte[] array, int length)
        {
            if (array.Length < length + 7)
                throw new ArgumentException("Target array is too small");

            Array.Copy(magicBytes, 0, array, 0, 4);
            array[4] = protocol.Major;
            array[5] = protocol.Minor;
            array[6] = (byte)(ushort)length;
            Encoding.UTF8.GetBytes(serializationName, 0, length, array, 7);
        }


        public int ToBytesCount()
        {
            return 7 + Encoding.UTF8.GetByteCount(serializationName);
        }
    }
}
