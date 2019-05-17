using System;
using System.Text.RegularExpressions;

namespace Pigeon.Addresses
{
    /// <summary>
    /// Represents tcp addresses
    /// </summary>
    [Serializable]
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
        /// Initializes a new instance of a TcpAddress after checking the name is valid
        /// </summary>
        /// <param name="name">Remote DNS name or IP address</param>
        /// <param name="port">IP port number</param>
        /// <returns></returns>
        public static TcpAddress FromNameAndPort(string name, ushort port)
        {
            if (String.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Name can not be null, empty or whitespace", nameof(name));

            if (!Regex.IsMatch(name, NamePattern) && !Regex.IsMatch(name, IPPattern))
                throw new ArgumentException($"Invalid name, does not match name or IP pattern", nameof(name));

            return new TcpAddress(name, port);
        }


        /// <summary>
        /// Initializes a new instance of a TcpAddress
        /// </summary>
        /// <param name="name">Remote DNS name or IP address</param>
        /// <param name="port">IP port number</param>
        private TcpAddress(string name, ushort port)
        {
            this.name = name;
            this.port = port;
        }


        /// <summary>
        /// Determines whether this instance and the other supplied object, which must be a <see cref="TcpAddress"/> are the the same
        /// </summary>
        /// <param name="other">The address to compare to this instance</param>
        /// <returns></returns>
        public bool Equals(IAddress other) => this.ToString() == other.ToString();


        /// <summary>
        /// Returns a string that represents the address
        /// </summary>
        /// <returns>Address in string form</returns>
        public override string ToString() => $"tcp://{name}:{port}";
    }
}
