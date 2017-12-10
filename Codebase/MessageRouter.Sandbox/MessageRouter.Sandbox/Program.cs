using System;

namespace MessageRouter.Sandbox
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

                    case "Clear":
                        Console.Clear();
                        break;

                    case "Exit":
                        return;

                    default:
                        Console.WriteLine($"Subprogram not recognised");
                        break;
                }
            }
        }
    }
}
