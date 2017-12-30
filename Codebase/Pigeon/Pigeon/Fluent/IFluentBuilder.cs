using Pigeon.Addresses;
using Pigeon.Publishers;
using Pigeon.Receivers;
using Pigeon.Requests;
using Pigeon.Senders;
using Pigeon.Subscribers;
using Pigeon.Topics;
using Pigeon.Transport;

namespace Pigeon.Fluent
{
    public interface IFluentBuilder
    {
        Router Build();
        Router BuildAndStart();
        IFluentBuilder WithPublisher<TPublisher>(IAddress address) where TPublisher : IPublisher;
        IFluentBuilder WithReceiver<TReceiver>(IAddress address) where TReceiver : IReceiver;
        IFluentBuilder WithRequestHandler<TRequest, TResponse>(IRequestHandler<TRequest, TResponse> handler)
            where TRequest : class
            where TResponse : class;
        IFluentBuilder WithRequestHandler<TRequest, TResponse>(RequestHandlerDelegate<TRequest, TResponse> handler)
            where TRequest : class
            where TResponse : class;
        IFluentBuilder WithSenderRouting<TSender, TRequest>(IAddress address)
            where TSender : ISender
            where TRequest : class;
        IFluentBuilder WithSubscriber<TSubscriber, TTopic>(IAddress address) where TSubscriber : ISubscriber;
        IFluentBuilder WithTopicHandler<TTopic>(ITopicHandler<TTopic> handler);
        IFluentBuilder WithTopicHandler<TTopic>(TopicHandlerDelegate<TTopic> handler);
        IFluentBuilder WithTransport<TTransport>() where TTransport : ITransportConfig;
    }
}