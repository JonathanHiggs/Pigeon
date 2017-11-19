using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace MessageRouter.Diagnostics
{
    public class ConsoleLogger<T> : ILogger
    {
        private string typeName;


        public ConsoleLogger()
        {
            typeName = typeof(T).Name;
        }


        public void Debug(string message, [CallerMemberName] string method = "")
        {
            Write("Debug", method, message);
        }

        public void Exception(string message, Exception exception, [CallerMemberName] string method = "")
        {
            Write("Exception", method, message);
        }

        public void Info(string message, [CallerMemberName] string method = "")
        {
            Write("Info", method, message);
        }

        public void Warning(string message, [CallerMemberName] string method = "")
        {
            Write("Warning", method, message);
        }


        private void Write(string level, string method, string message)
        {
            Console.WriteLine($"[{DateTime.Now.ToShortTimeString()}][{typeName}][{method}][{level}] {message}");
        }
    }
}
