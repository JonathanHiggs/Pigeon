using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace Pigeon.Protocol
{
    /// <summary>
    /// Contains a register of protocols and dispatches de-serialization of binary encoded messages
    /// </summary>
    public class ProtocolsRegister
    {
        private readonly Dictionary<string, IProtocolDeserializer<ProtocolMessage>> builders;


        /// <summary>
        /// Initializes a new instance of <see cref="ProtocolsRegister"/>
        /// </summary>
        public ProtocolsRegister()
        { }


        /// <summary>
        /// Initializes a new instance of <see cref="ProtocolsRegister"/>
        /// </summary>
        /// <param name="deserializers">An enumeration of specific <see cref="IProtocolDeserializer{T}"/> 
        /// that will be registered</param>
        public ProtocolsRegister(IEnumerable<IProtocolDeserializer<ProtocolMessage>> deserializers)
        {
            foreach (var builder in deserializers)
                Register(builder);
        }


        /// <summary>
        /// Registers the supplied <see cref="IProtocolDeserializer{T}"/>
        /// </summary>
        /// <param name="builder"></param>
        public void Register(IProtocolDeserializer<ProtocolMessage> builder)
        { 
            var name = builder.ProtocolType.GetCustomAttribute<ProtocolNameAttribute>();

            if (name is null || string.IsNullOrWhiteSpace(name.Name))
                throw new InvalidOperationException();

            builders.Add(name.Name, builder);
        }


        /// <summary>
        /// Initializes a new instance of <see cref="ProtocolMessage"/> from serialized data
        /// </summary>
        /// <param name="data">Binary data to de-serialize</param>
        /// <returns></returns>
        public ProtocolMessage ReadFrom(byte[] data)
        {
            using (var stream = new MemoryStream(data))
                return ReadFrom(stream);
        }


        /// <summary>
        /// Initializes a new instance of <see cref="ProtocolMessage"/> from serialized data
        /// </summary>
        /// <param name="stream">Data stream to de-serialize</param>
        /// <returns></returns>
        public ProtocolMessage ReadFrom(Stream stream)
        {
            using (var reader = new BinaryReader(stream))
                return ReadFrom(reader);
        }


        /// <summary>
        /// Initializes a new instance of <see cref="ProtocolMessage"/> from serialized data
        /// </summary>
        /// <param name="reader">Data reader to de-serialize</param>
        /// <returns></returns>
        public ProtocolMessage ReadFrom(BinaryReader reader)
        {
            var name = reader.ReadString();
            if (!builders.TryGetValue(name, out var builder))
                throw new InvalidOperationException();

            return builder.ReadFrom(reader);
        }
    }
}
