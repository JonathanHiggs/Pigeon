using System;
using System.IO;

namespace Pigeon.Serialization
{
    /// <summary>
    /// Names and describes an <see cref="ISerializer"/> in a strongly typed but extensible 
    /// </summary>
    public struct SerializationDescriptor
    {
        /// <summary>
        /// Gets the invariant name of the <see cref="ISerializer"/>
        /// </summary>
        public string InvariantName { get; private set; }


        /// <summary>
        /// Initializes a new instance of <see cref="SerializationDescriptor"/>
        /// </summary>
        /// <param name="invariantName">Invariant name of the associated <see cref="ISerializer"/></param>
        /// <param name="type"></param>
        public SerializationDescriptor(string invariantName)
        {
            InvariantName = invariantName ?? throw new ArgumentNullException(nameof(invariantName));
        }


        /// <summary>
        /// Determines whether the specified object is equal to the current object
        /// </summary>
        /// <param name="obj">The object to compare with the current object</param>
        /// <returns>true if the specified object is equal to the current object; otherwise, false</returns>
        public override bool Equals(object obj) => obj is SerializationDescriptor other && (this == other);


        /// <summary>
        /// Returns a hash code for this object
        /// </summary>
        /// <returns>A hash code for this object</returns>
        public override int GetHashCode() => InvariantName.GetHashCode();


        /// <summary>
        /// Determines whether two <see cref="SerializationDescriptor"/>s have the same value
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static bool operator ==(SerializationDescriptor a, SerializationDescriptor b) => a.InvariantName.Equals(b.InvariantName);


        /// <summary>
        /// Determines whether two <see cref="SerializationDescriptor"/>s have different values
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static bool operator !=(SerializationDescriptor a, SerializationDescriptor b) => !a.Equals(b);


        public static readonly SerializationDescriptor DotNet = new SerializationDescriptor("DOTNET");
        public static readonly SerializationDescriptor Json = new SerializationDescriptor("JSON");
    }


    public static class SerializationDescriptorExtensions
    {
        public static void Write(this BinaryWriter writer, SerializationDescriptor serializationDescriptor)
            => writer.Write(serializationDescriptor.InvariantName);


        public static SerializationDescriptor ReadSerializationDescriptor(this BinaryReader reader)
            => new SerializationDescriptor(reader.ReadString());
    }
}
