using System;

using Pigeon.Sandbox.Programs;

namespace Pigeon.Sandbox
{
    class Program
    {
        static void Main(string[] args)
        {
            while (true)
            {
                Console.WriteLine("Exit to quit");
                Console.Write("Enter the subprogram name: ");
                var name = Console.ReadLine();

                switch (name)
                {
                    case "SenderReceiver":
                        SenderReceiver.Run();
                        break;

                    case "Server":
                        Server.Run();
                        break;

                    case "Client":
                        Client.Run();
                        break;

                    case "Publisher":
                        Publisher.Run();
                        break;

                    case "Subscriber":
                        Subscriber.Run();
                        break;

                    case "WebServer":
                        WebServer.Run();
                        break;

                    case "Clear":
                        Console.Clear();
                        break;

                    case "Exit":
                        return;

                    default:
                        Console.WriteLine($"Subprogram not recognised");
                        break;
                }

                Console.WriteLine();
            }
        }
    }
}
