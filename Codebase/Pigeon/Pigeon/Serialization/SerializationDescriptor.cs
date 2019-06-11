using System;

namespace Pigeon.Serialization
{
    /// <summary>
    /// Names and describes an <see cref="ISerializer"/> in a strongly typed but extensible 
    /// </summary>
    public struct SerializationDescriptor
    {
        private readonly string description;


        /// <summary>
        /// Gets a string that names a <see cref="ISerializer"/>
        /// </summary>
        public string Name { get; private set; }


        /// <summary>
        /// Gets the type that the <see cref="ISerializer"/> converts to and from
        /// </summary>
        public Type Type { get; private set; }


        /// <summary>
        /// Initialzies a new instance of <see cref="SerializationDescriptor"/>
        /// </summary>
        /// <param name="name"></param>
        /// <param name="description"></param>
        /// <param name="type"></param>
        public SerializationDescriptor(string name, string description, Type type)
        {
            this.Name = name ?? throw new ArgumentNullException(nameof(name));
            this.description = description ?? throw new ArgumentNullException(nameof(description));
            this.Type = type ?? throw new ArgumentNullException(nameof(type));
        }


        /// <summary>
        /// Determines whether the specified object is equal to the current object
        /// </summary>
        /// <param name="obj">The object to compare with the current object</param>
        /// <returns>true if the specified object is equal to the current object; otherwise, false</returns>
        public override bool Equals(object obj) => obj is SerializationDescriptor && ((SerializationDescriptor)obj).Name == Name;


        /// <summary>
        /// Returns a hash code for this object
        /// </summary>
        /// <returns>A hash code for this object</returns>
        public override int GetHashCode() => Name.GetHashCode();

        
        public static bool operator ==(SerializationDescriptor a, SerializationDescriptor b) => a.Equals(b);


        public static bool operator !=(SerializationDescriptor a, SerializationDescriptor b) => !a.Equals(b);


        public static readonly SerializationDescriptor DotNet = new SerializationDescriptor("DotNet", "Inbuilt .Net serialization", typeof(byte));
        public static readonly SerializationDescriptor CSV = new SerializationDescriptor("CSV", "Comma seperated values", typeof(string));
        public static readonly SerializationDescriptor JSON = new SerializationDescriptor("JSON", "json", typeof(string));
        public static readonly SerializationDescriptor BSON = new SerializationDescriptor("BSON", "binary json", typeof(byte));
        public static readonly SerializationDescriptor HTML = new SerializationDescriptor("HTML", "Hypertext markup language", typeof(string));
        public static readonly SerializationDescriptor XML = new SerializationDescriptor("XML", "eXtensible markup language", typeof(string));
    }
}
