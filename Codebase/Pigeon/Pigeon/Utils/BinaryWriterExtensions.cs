using System;
using System.IO;

namespace Pigeon.Utils
{
    public static class BinaryWriterExtensions
    {
        public static void Write(this BinaryWriter writer, Guid guid) =>
            writer.Write(guid.ToByteArray());


        public static void Write(this BinaryWriter writer, Type value) =>
            writer.Write(value.ToString());
    }
}
