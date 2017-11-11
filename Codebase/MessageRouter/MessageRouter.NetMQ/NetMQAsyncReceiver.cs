using MessageRouter.Receivers;
using MessageRouter.Serialization;
using NetMQ;
using NetMQ.Sockets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessageRouter.NetMQ
{
    public class NetMQAsyncReceiver : NetMQReceiver, IAsyncReceiver
    {
        public event RequestTaskDelegate RequestReceived;


        public ISocketPollable PollableSocket => routerSocket;


        public NetMQAsyncReceiver(RouterSocket routerSocket, ISerializer<byte[]> binarySerializer)
            : base(routerSocket, binarySerializer)
        {
            routerSocket.ReceiveReady += OnRequestReceived;
        }


        private void OnRequestReceived(object sender, NetMQSocketEventArgs e)
        {
            RequestReceived?.Invoke(this, Receive());
        }
    }
}
