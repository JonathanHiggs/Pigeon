using System;

using Pigeon.Fluent.Transport;
using Pigeon.Publishers;
using Pigeon.Receivers;
using Pigeon.Senders;
using Pigeon.Subscribers;
using Pigeon.Transport;

namespace Pigeon.Web
{
    public class WebTransport : ITransportConfig
    {
        private readonly WebFactory factory;


        public WebTransport(IContainer container)
        {
            if (container is null)
                throw new ArgumentNullException(nameof(container));

            factory = container.Resolve<WebFactory>();
            Configurer = container.Resolve<TransportSetup<IWebSender, IWebReceiver, IWebPublisher, IWebSubscriber>>();
        }


        public ISenderFactory SenderFactory => factory;


        public IReceiverFactory ReceiverFactory => factory;


        public IPublisherFactory PublisherFactory => factory;


        public ISubscriberFactory SubscriberFactory => factory;


        public ITransportSetup Configurer { get; }
    }
}
