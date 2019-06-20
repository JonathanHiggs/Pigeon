using Pigeon.Publishers;
using Pigeon.Receivers;
using Pigeon.Senders;
using Pigeon.Subscribers;

namespace Pigeon.Web
{
    public interface IWebMonitor : ISenderMonitor<IWebSender>, IReceiverMonitor<IWebReceiver>, IPublisherMonitor<IWebPublisher>, ISubscriberMonitor<IWebSubscriber>
    {
    }
}
