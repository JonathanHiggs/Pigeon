using System;

using Pigeon.Serialization;

namespace Pigeon.Fluent
{
    public interface ISerializerBuilder
    {
        ContainerBuilder WithSerializer<TSerializer>(bool defaultSerializer = false, Action<TSerializer> setup = null)
            where TSerializer : ISerializer;
    }
}
