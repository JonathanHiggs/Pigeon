using Pigeon.Publishers;
using Pigeon.Receivers;
using Pigeon.Senders;
using Pigeon.Subscribers;

namespace Pigeon.Transport
{
    /// <summary>
    /// Combined factory for <see cref="ISender"/>s, <see cref="IReceiver"/>s, <see cref="IPublisher"/>s and <see cref="ISubscriber"/>s
    /// </summary>
    /// <typeparam name="TSender">The transport specific implementation of <see cref="ISender"/> this factory creates</typeparam>
    /// <typeparam name="TReceiver">The transport specific implementation of <see cref="IReceiver"/> this factory creates</typeparam>
    /// <typeparam name="TPublisher">The transport specific implementation of <see cref="IPublisher"/> this factory creates</typeparam>
    /// <typeparam name="TSubscriber">The transport specific implementation of <see cref="ISubscriber"/> this factory creates</typeparam>
    public interface ITransportFactory<TSender, TReceiver, TPublisher, TSubscriber> 
        : ISenderFactory<TSender>, IReceiverFactory<TReceiver>, IPublisherFactory<TPublisher>, ISubscriberFactory<TSubscriber>
        where TSender : ISender
        where TReceiver : IReceiver
        where TPublisher : IPublisher
        where TSubscriber : ISubscriber
    {
    }
}
