using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pigeon.Addresses;
using Pigeon.Fluent;
using Pigeon.Routing;
using Pigeon.Senders;
using Pigeon.Subscribers;

namespace Pigeon.NetMQ.Fluent
{
    public class TransportSetup<TSender, TSubscriber> : ITransportSetup
        where TSender : ISender
        where TSubscriber : ISubscriber
    {
        private IRequestRouter requestRouter;
        private ITopicRouter topicRouter;


        public TransportSetup(IRequestRouter requestRouter, ITopicRouter topicRouter)
        {
            this.requestRouter = requestRouter ?? throw new ArgumentNullException(nameof(requestRouter));
            this.topicRouter = topicRouter ?? throw new ArgumentNullException(nameof(topicRouter));
        }

                
        public ISenderSetup WithSender(IAddress address)
        {
            return new SenderSetup<TSender>(requestRouter, address);
        }

        public ISubscriberSetup WithSubscriber(IAddress address)
        {
            return new SubscriberSetup<TSubscriber>(topicRouter, address);
        }
    }    
}
