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
using System.Threading;
using System.Threading.Tasks;

namespace MessageRouter.Sandbox
{
    public class AsyncServer
    {
        public static void Run()
        {
            var socket = new RouterSocket();
            var serializer = new BinarySerializer();
            var receiver = new NetMQAsyncReceiver(socket, serializer);
            var receiverManager = new NetMQAsyncReceiverManager(receiver);
            var messageFactory = new MessageFactory();
            var requestDispatcher = new RequestDispatcher();
            var server = new AsyncMessageServer(messageFactory, receiverManager, requestDispatcher, "Server");

            requestDispatcher.Register<TestMessage, TestMessage>(Handler);

            receiver.Bind(TcpAddress.Wildcard(5555));

            server.Start();

            Console.WriteLine("Press enter to stop server");
            Console.ReadLine();

            server.Stop();
        }


        private static TestMessage Handler(TestMessage request)
        {
            Console.WriteLine($"Received {request.Num}");
            Thread.Sleep(5000);
            Console.WriteLine($"Responding to {request.Num}");
            return new TestMessage { Num = request.Num + 1 };
        }
    }
}
