using MessageRouter.Addresses;
using MessageRouter.Messages;
using MessageRouter.NetMQ;
using MessageRouter.Receivers;
using MessageRouter.Serialization;
using MessageRouter.Server;
using NetMQ.Sockets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessageRouter.Sandbox
{
    public class Server
    {
        public static void Run()
        {
            var socket = new RouterSocket();
            var serializer = new BinarySerializer();
            var receiver = new NetMQReceiver(socket, serializer);
            //var receiverFactory = new NetMQReceiverFactory(receiver);
            var receiverManager = new ReceiverManager(receiver);
            var messageFactory = new MessageFactory();
            var requestDispatcher = new RequestDispatcher();
            var server = new MessageServer(messageFactory, receiverManager, requestDispatcher, "Server");

            requestDispatcher.Register<TestMessage, TestMessage>(Handler);

            receiver.Bind(TcpAddress.Server.Port(5555));

            server.Run();
        }


        private static TestMessage Handler(TestMessage request)
        {
            Console.WriteLine($"Received {request.Num}");
            return new TestMessage { Num = request.Num + 1 };
        }
    }
}
