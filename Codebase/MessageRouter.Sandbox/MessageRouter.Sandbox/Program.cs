using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessageRouter.Sandbox
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Enter the subprogram name: ");
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

                default:
                    throw new NotImplementedException();
            }

        }
    }
}
