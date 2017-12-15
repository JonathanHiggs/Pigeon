using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MessageRouter.Messages;

namespace MessageRouter.Subscribers
{
    public delegate void TopicEventHandler(ISubscriber subscriber, Message message);
}
