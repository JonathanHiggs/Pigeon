using System.IO;

namespace Pigeon.Protocol.Pigeon.v1_0
{
    public enum ResponseStatus
    {
        OK = 200,
        BAD = 400
    }


    public static class ResponseStatusExtensions
    {
        public static void Write(this BinaryWriter writer, ResponseStatus responseStatus)
            => writer.Write((int)responseStatus);


        public static ResponseStatus ReadResponseStatus(this BinaryReader reader) =>
            (ResponseStatus)reader.ReadInt32();        
    }
}
