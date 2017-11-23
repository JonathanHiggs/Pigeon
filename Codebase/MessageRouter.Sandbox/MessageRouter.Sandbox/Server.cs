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
    public class Server
    {
        public static void Run()
        {
            var address = TcpAddress.Wildcard(5555);
            var socket = new RouterSocket();
            var serializer = new BinarySerializer();
            var receiver = new NetMQReceiver(socket, serializer);
            //var receiverFactory = new NetMQReceiverFactory(receiver);
            var receiverManager = new ReceiverManager(receiver, address);
            var messageFactory = new MessageFactory();
            var requestDispatcher = new RequestDispatcher();
            var server = new MessageServer(messageFactory, receiverManager, requestDispatcher, "Server");

            requestDispatcher.Register<TestMessage, TestMessage>(Handler);
            
            var cancel = new CancellationTokenSource();
            var serverTask = Task.Factory.StartNew(() => server.Run(cancel.Token));
            
            Console.WriteLine("Press enter to stop server");
            Console.ReadLine();

            cancel.Cancel();
        }


        private static TestMessage Handler(TestMessage request)
        {
            Console.WriteLine($"Received {request.Num}");
            Thread.Sleep(3000);
            Console.WriteLine($"Returning {request.Num + 1}");
            return new TestMessage { Num = request.Num + 1 };
        }
    }
}
