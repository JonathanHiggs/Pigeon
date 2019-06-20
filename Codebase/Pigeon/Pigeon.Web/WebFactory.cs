using System;
using System.Net;

using Pigeon.Addresses;
using Pigeon.Transport;

namespace Pigeon.Web
{
    public class WebFactory : TransportFactory<IWebSender, IWebReceiver, IWebPublisher, IWebSubscriber>
    {
        private readonly IWebMonitor monitor;
        private readonly IWebMessageFactory messageFactory;


        public WebFactory(IWebMonitor monitor, IWebMessageFactory messageFactory)
            : base(monitor, monitor, monitor, monitor)
        {
            this.monitor = monitor ?? throw new ArgumentNullException(nameof(monitor));
            this.messageFactory = messageFactory ?? throw new ArgumentNullException(nameof(messageFactory));
        }


        protected override IWebPublisher CreateNewPublisher(IAddress address) =>
            throw new NotImplementedException();


        protected override IWebReceiver CreateNewReceiver(IAddress address)
        {
            var receiver = new WebReceiver(new HttpListener(), messageFactory, monitor.AsyncRequestHandler);
            receiver.AddAddress(address);
            return receiver;
        }


        protected override IWebSender CreateNewSender(IAddress address) =>
            throw new NotImplementedException();


        protected override IWebSubscriber CreateNewSubscriber(IAddress address) =>
            throw new NotImplementedException();
    }
}
