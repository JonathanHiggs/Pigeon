using System;
using System.IO;

namespace Pigeon.Utils
{
    public static class BinaryReaderExtensions
    {
        public static Guid ReadGuid(this BinaryReader reader) =>
            new Guid(reader.ReadBytes(16));


        public static Type ReadType(this BinaryReader reader) =>
            Type.GetType(reader.ReadString());
    }
}