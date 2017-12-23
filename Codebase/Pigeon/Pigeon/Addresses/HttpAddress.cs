using System;

namespace Pigeon.Addresses
{
    /// <summary>
    /// Represents an http address
    /// </summary>
    public class HttpAddress : IAddress
    {
        private ushort port;
        private string name;


        /// <summary>
        /// Gets the default wildcard host on port 80
        /// </summary>
        public static HttpAddress WebHost { get { return new HttpAddress("*", 80); } }


        /// <summary>
        /// Initializes a new wildcard instance of <see cref="HttpAddress"/> 
        /// </summary>
        /// <param name="port">TCP port number</param>
        /// <returns></returns>
        public static HttpAddress Wildcard(ushort port) => new HttpAddress("*", port);


        /// <summary>
        /// Initializes a new named instance of <see cref="HttpAddress"/> on port 80
        /// </summary>
        /// <param name="name">Domain name</param>
        /// <returns></returns>
        public static HttpAddress Named(string name) => new HttpAddress(name, 80);


        /// <summary>
        /// Initializes a new named instance of <see cref="HttpAddress"/>
        /// </summary>
        /// <param name="name">Domain name</param>
        /// <param name="port">Port number</param>
        /// <returns></returns>
        public static object Named(string name, ushort port) => new HttpAddress(name, port);


        /// <summary>
        /// Initializes a new instance of <see cref="HttpAddress"/>
        /// </summary>
        /// <param name="name">Domain name</param>
        /// <param name="port">TCP port number</param>
        private HttpAddress(string name, ushort port)
        {
            if (String.IsNullOrWhiteSpace(name))
                throw new ArgumentException($"'{name}' is an invalid domain name");

            this.name = name;
            this.port = port;
        }


        /// <summary>
        /// Determines whether this instance and the other supplied object, which must be a <see cref="HttpAddress"/> are the the same
        /// </summary>
        /// <param name="other">The address to compare to this instance</param>
        /// <returns></returns>
        public bool Equals(IAddress other) => !(other is null) && other is HttpAddress && ToString() == other.ToString();


        /// <summary>
        /// Returns a string that represents the address
        /// </summary>
        /// <returns>Address in string form</returns>
        public override string ToString() => $"http://{name}:{port}/";
    }
}
