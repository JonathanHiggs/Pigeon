using MessageRouter.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessageRouter.Senders
{
    public interface IAsyncSender : ISender
    {
        Task<Message> SendAndReceiveAsync(Message message);
    }
}
