using Pigeon.Addresses;

namespace Pigeon.Fluent.Transport
{
    public interface ITransportSetup
    {
        ISenderSetup WithSender(IAddress address);
        ISubscriberSetup WithSubscriber(IAddress address);
        void WithReceiver(IAddress address);
        void WithPublisher(IAddress address);
    }
}
