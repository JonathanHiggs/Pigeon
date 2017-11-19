using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace MessageRouter.Diagnostics
{
    public interface ILogger
    {
        void Debug(string message, [CallerMemberName] string method = "");
        void Info(string message, [CallerMemberName] string method = "");
        void Warning(string message, [CallerMemberName] string method = "");
        void Exception(string message, Exception exception, [CallerMemberName] string method = "");
    }
}
