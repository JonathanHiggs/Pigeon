using System.IO;

using Pigeon.Protocol.v1_0;

namespace Pigeon.Protocol
{
    public class PigeonMessageFactory
    {
        public PigeonMessage DeserializeMessage(byte[] data)
        {
            using (var stream = new MemoryStream(data))
            using (var reader = new BinaryReader(stream))
            {
                return PigeonMessage.ReadFrom(reader);
            }
        }


        public byte[] SerializeMessage(PigeonMessage message)
        {
            using (var stream = new MemoryStream())
            using (var writer = new BinaryWriter(stream))
            {
                message.WriteTo(writer);
                return stream.ToArray();
            }
        }
    }
}
