using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessageRouter.Addresses
{
    public class TcpAddress : IAddress
    {
        private int port;
        private string address;


        public TcpAddress(string address, int port)
        {
            this.port = port;
            this.address = address;
        }


        public bool Equals(IAddress other)
        {
            return this.ToString() == other.ToString();
        }


        public override string ToString()
        {
            return $"tcp://{address}:{port}";
        }


        private static ServerBuilder serverBuilder = new ServerBuilder();
        public static ServerBuilder Server => serverBuilder;

        private static ClientBuilder clientBuilder = new ClientBuilder();
        public static ClientBuilder Client => clientBuilder;


        public class ServerBuilder
        {
            public TcpAddress Port(int port)
            {
                return new TcpAddress("*", port);
            }
        }


        public class ClientBuilder
        {
            public TcpAddress Localhost(int port)
            {
                return new TcpAddress("localhost", port);
            }

            public TcpAddress Named(string name, int port)
            {
                return new TcpAddress(name, port);
            }
        }
    }
}
