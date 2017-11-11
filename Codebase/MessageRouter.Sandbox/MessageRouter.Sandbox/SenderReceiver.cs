using NetMQ;
using NetMQ.Sockets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MessageRouter.Sandbox
{
    public class SenderReceiver
    {
        public static void Run()
        {
            var source = new CancellationTokenSource();

            Console.WriteLine("Press any key to quit");

            RunServer(source.Token);
            RunClient(source.Token);

            Console.ReadLine();
            source.Cancel();
        }


        static void RunClient(CancellationToken cancellationToken)
        {
            Task.Factory.StartNew(() =>
            {
                using (var client = new RequestSocket())
                {
                    client.Connect("tcp://localhost:5555");
                    var i = 0;

                    while (true)
                    {
                        Console.WriteLine("Client: Sending Hello");

                        var request = new NetMQMessage();
                        request.Append($"Hello {i++}");
                        request.AppendEmptyFrame();

                        client.SendMultipartMessage(request);

                        var response = client.ReceiveMultipartMessage();
                        var message = response[0].ConvertToString();

                        Console.WriteLine($"Client: Received {message}");

                        if (cancellationToken.IsCancellationRequested)
                            return;
                    }
                }
            });
        }


        static void RunServer(CancellationToken cancellationToken)
        {
            Task.Factory.StartNew(() =>
            {
                var random = new Random();
                using (var server = new RouterSocket())
                {
                    server.Bind("tcp://*:5555");
                    Console.WriteLine("\t\t\tServer: Bound to *:5555");

                    while (!cancellationToken.IsCancellationRequested)
                    {
                        var request = server.ReceiveMultipartMessage();
                        var message = request[2].ConvertToString();

                        Console.WriteLine($"\t\t\tServer: Received {message}");

                        // Simulate work
                        Thread.Sleep(200);

                        var responsestr = $"World {random.Next(100)}";
                        Console.WriteLine($"\t\t\tServer: Sending {responsestr}");

                        var response = new NetMQMessage();
                        response.Append(request[0]);
                        response.AppendEmptyFrame();
                        response.Append(responsestr);
                        response.AppendEmptyFrame();
                        server.SendMultipartMessage(response);
                    }
                }
            });
        }
    }
}
