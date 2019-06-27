using System;
using System.Text.RegularExpressions;

namespace Pigeon.Addresses
{
    /// <summary>
    /// Represents tcp addresses
    /// </summary>
    public class TcpAddress : IAddress
    {
        private ushort port;
        private string name;


        /// <summary>
        /// Name regex matching pattern
        /// </summary>
        public static readonly string NamePattern = @"^((([a-zA-Z]|[a-zA-Z][a-zA-Z0-9\-]*[a-zA-Z0-9])\.)*([A-Za-z]|[A-Za-z][A-Za-z0-9\-]*[A-Za-z0-9])|[\*])$";


        /// <summary>
        /// IP regex matching pattern
        /// </summary>
        public static readonly string IPPattern = @"^(([0-9]|[1-9][0-9]|1[0-9]{2}|2[0-4][0-9]|25[0-5])\.){3}([0-9]|[1-9][0-9]|1[0-9]{2}|2[0-4][0-9]|25[0-5])$";


        /// <summary>
        /// Initializes a new instance of a <see cref="TcpAddress"/>
        /// </summary>
        /// <param name="name">Remote DNS name or IP address</param>
        /// <param name="port">IP port number</param>
        protected TcpAddress(string name, ushort port)
        {
            this.name = name;
            this.port = port;
        }


        /// <summary>
        /// Initializes a wildcard address
        /// </summary>
        /// <param name="port">Port number</param>
        /// <returns></returns>
        public static TcpAddress Wildcard(ushort port) => new TcpAddress("*", port);
            

        /// <summary>
        /// Initializes a localhost address
        /// </summary>
        /// <param name="port">Port number</param>
        /// <returns></returns>
        public static TcpAddress Localhost(ushort port) => new TcpAddress("localhost", port);


        /// <summary>
        /// Initializes a new instance of a <see cref="TcpAddress"/> after checking the name is valid
        /// </summary>
        /// <param name="name">Remote DNS name or IP address</param>
        /// <param name="port">IP port number</param>
        /// <returns></returns>
        public static TcpAddress FromNameAndPort(string name, ushort port)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Name can not be null, empty or whitespace", nameof(name));

            if (!Regex.IsMatch(name, NamePattern) && !Regex.IsMatch(name, IPPattern))
                throw new ArgumentException($"Invalid name, does not match name or IP pattern", nameof(name));

            return new TcpAddress(name, port);
        }


        /// <summary>
        /// Determines whether the two specified <see cref="TcpAddress"/>es have the same value
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static bool operator ==(TcpAddress a, TcpAddress b) => a.name == b.name && a.port == b.port;


        /// <summary>
        /// Determines whether the two specified <see cref="TcpAddress"/>es have the different values
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static bool operator !=(TcpAddress a, TcpAddress b) => !a.Equals(b);


        /// <summary>
        /// Determines whether the supplied object is equal to this instance
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj) => !(obj is null) && obj is TcpAddress other && this == other;


        /// <summary>
        /// Returns the hash code for this address
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode() => ToString().GetHashCode();


        /// <summary>
        /// Determines whether this instance and the other supplied object, which must be a <see cref="TcpAddress"/> are the the same
        /// </summary>
        /// <param name="other">The address to compare to this instance</param>
        /// <returns></returns>
        public bool Equals(IAddress other) => !(other is null) && other is TcpAddress otherAddress && this == otherAddress;


        /// <summary>
        /// Returns a string that represents this <see cref="TcpAddress"/>
        /// </summary>
        /// <returns>Address in string form</returns>
        public override string ToString() => $"tcp://{name}:{port}";
    }
}
