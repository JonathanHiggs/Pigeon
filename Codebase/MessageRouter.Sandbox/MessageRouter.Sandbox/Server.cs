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
            var socket = new RouterSocket();
            var serializer = new BinarySerializer();
            var receiver = new NetMQReceiver(socket, serializer);
            var receiverManager = new NetMQReceiverManager(receiver);
            var messageFactory = new MessageFactory();
            var requestDispatcher = new RequestDispatcher();
            var server = new MessageServer(messageFactory, receiverManager, requestDispatcher, "Server");

            requestDispatcher.Register<TestMessage, TestMessage>(Handler);
            receiver.Add(TcpAddress.Wildcard(5555));
            server.Start();
            
            Console.WriteLine("Press enter to stop server");
            Console.ReadLine();

            server.Stop();
        }


        private static void RunServer(MessageServer server, CancellationToken token)
        {
            try
            {
                server.Start();
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex);
            }
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
