using System;
using System.Collections.Generic;
using System.Text;
using Pigeon.Fluent.Transport;
using Pigeon.Publishers;
using Pigeon.Receivers;
using Pigeon.Senders;
using Pigeon.Subscribers;
using Pigeon.Transport;

namespace Pigeon.ActiveMQ
{
    public class ActiveMQTransport : ITransportConfig
    {
        // https://www.pmichaels.net/2016/09/29/a-c-programmers-guide-to-installing-running-and-messaging-with-activemq/

        public ISenderFactory SenderFactory => throw new NotImplementedException();

        public IReceiverFactory ReceiverFactory => throw new NotImplementedException();

        public IPublisherFactory PublisherFactory => throw new NotImplementedException();

        public ISubscriberFactory SubscriberFactory => throw new NotImplementedException();

        public ITransportSetup Configurer => throw new NotImplementedException();
    }
}
