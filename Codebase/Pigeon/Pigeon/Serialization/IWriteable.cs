using System.IO;

namespace Pigeon.Serialization
{
    /// <summary>
    /// 
    /// </summary>
    public interface IWriteable
    {
        void WriteTo(BinaryWriter writer);
    }
}
