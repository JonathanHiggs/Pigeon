using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mime;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Pigeon.Sandbox.Programs
{
    public class WebServer
    {
        private HttpListener listener = new HttpListener();
        private Func<HttpListenerRequest, string> respond;
        private Thread thread;
        private ManualResetEvent requestReceived = new ManualResetEvent(false);
        private bool verbose;
        private bool running = false;
        private object lockObj = new object { };


        public WebServer(Func<HttpListenerRequest, string> respond, bool verbose, params string[] prefixes)
        {
            this.respond = respond;
            this.verbose = verbose;

            foreach (var prefix in prefixes)
                listener.Prefixes.Add(prefix);
        }


        public void Start()
        {
            lock (lockObj)
            {
                if (running)
                    return;

                Console.WriteLine("Starting WebServer");
                listener.Start();
                thread = new Thread(new ThreadStart(Listen));
                thread.Name = "WebServerListener";
                thread.IsBackground = false;
                thread.Start();

                running = true;
            }
        }


        public void Stop()
        {
            lock (lockObj)
            {
                if (!running)
                    return;

                Console.WriteLine("Stopping WebServer");
                listener.Stop();
                listener.Close();
                requestReceived.Set();
                thread.Join();
                Console.WriteLine("WebServer stopped");
            }
        }


        public void Listen()
        {
            while (listener.IsListening)
            {
                requestReceived.Reset();
                
                Task.Factory.StartNew(async () =>
                {
                    var taskId = Task.CurrentId; // Capture the id of the task through continuations

                    if (verbose)
                        Console.WriteLine($"[{taskId}] Waiting for request");

                    var context = await listener.GetContextAsync(); // Don't block the thread, yo
                    Console.WriteLine($"[{taskId}] Request received: {context.Request.RawUrl}");
                    requestReceived.Set(); // Allow another task to be created

                    try
                    {
                        if (verbose)
                            Console.WriteLine($"[{taskId}] Responding to request");

                        var response = respond(context.Request);
                        var data = Encoding.UTF8.GetBytes(response);

                        context.Response.ContentType = MediaTypeNames.Text.Html;
                        context.Response.ContentLength64 = data.Length;
                        context.Response.AddHeader("Date", DateTime.Now.ToString("r"));
                        
                        await context.Response.OutputStream.WriteAsync(data, 0, data.Length);
                        context.Response.StatusCode = (int)HttpStatusCode.OK;

                    }
                    finally
                    {
                        context.Response.Close();
                        if (verbose)
                            Console.WriteLine($"[{taskId}] Response sent for {context.Request.RawUrl}");
                    }
                });

                requestReceived.WaitOne(); // Wait until a request is received before spawning a new request task
            }
        }


        public static string SendResponse(HttpListenerRequest request)
        {
            return $"<HTML><BODY>WebServer<br>{DateTime.Now.ToLongTimeString()}</BODY></HTML>";
        }


        public static void Run()
        {
            // https://gist.github.com/aksakalli/9191056
            var server = new WebServer(SendResponse, false, "http://*:80/");
            server.Start();
            Console.WriteLine("Press enter to stop");
            Console.ReadLine();
            server.Stop();
        }
    }
}
