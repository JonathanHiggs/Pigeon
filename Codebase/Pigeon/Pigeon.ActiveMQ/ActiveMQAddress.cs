using System;
using System.Text.RegularExpressions;

using Pigeon.Addresses;

namespace Pigeon.ActiveMQ
{
    public class ActiveMQAddress : IAddress
    {
        private ushort port;
        private string name;


        /// <summary>
        /// Initializes a new instance of a <see cref="ActiveMQAddress"/>
        /// </summary>
        /// <param name="name">Remote DNS name or IP address</param>
        /// <param name="port">IP port number</param>
        protected ActiveMQAddress(string name, ushort port)
        {
            this.name = name;
            this.port = port;
        }


        /// <summary>
        /// Initializes a wildcard address
        /// </summary>
        /// <param name="port">Port number</param>
        /// <returns></returns>
        public static ActiveMQAddress Wildcard(ushort port) => new ActiveMQAddress("*", port);


        /// <summary>
        /// Initializes a localhost address
        /// </summary>
        /// <param name="port">Port number</param>
        /// <returns></returns>
        public static ActiveMQAddress Localhost(ushort port) => new ActiveMQAddress("localhost", port);


        /// <summary>
        /// Initializes a new instance of <see cref="ActiveMQAddress"/>
        /// </summary>
        /// <param name="name">Remote DNS name or IP address</param>
        /// <returns></returns>
        public static ActiveMQAddress FromName(string name) => FromNameAndPort(name, 61616);


        /// <summary>
        /// Initializes a new instance of a <see cref="ActiveMQAddress"/> after checking the name is valid
        /// </summary>
        /// <param name="name">Remote DNS name or IP address</param>
        /// <param name="port">IP port number</param>
        /// <returns></returns>
        public static ActiveMQAddress FromNameAndPort(string name, ushort port)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Name can not be null, empty or whitespace", nameof(name));

            if (!Regex.IsMatch(name, TcpAddress.NamePattern) && !Regex.IsMatch(name, TcpAddress.IPPattern))
                throw new ArgumentException($"Invalid name, does not match name or IP pattern", nameof(name));

            return new ActiveMQAddress(name, port);
        }


        /// <summary>
        /// Determines whether the two specified <see cref="ActiveMQAddress"/>es have the same value
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static bool operator ==(ActiveMQAddress a, ActiveMQAddress b) => a.name == b.name && a.port == b.port;


        /// <summary>
        /// Determines whether the two specified <see cref="ActiveMQAddress"/>es have the different values
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static bool operator !=(ActiveMQAddress a, ActiveMQAddress b) => !a.Equals(b);


        /// <summary>
        /// Determines whether the supplied object is equal to this instance
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj) => !(obj is null) && obj is ActiveMQAddress other && this == other;


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
        public bool Equals(IAddress other) => !(other is null) && other is ActiveMQAddress otherAddress && this == otherAddress;


        /// <summary>
        /// Returns a string that represents this <see cref="TcpAddress"/>
        /// </summary>
        /// <returns>Address in string form</returns>
        public override string ToString() => $"activemq:tcp://{name}:{port}";
    }
}
