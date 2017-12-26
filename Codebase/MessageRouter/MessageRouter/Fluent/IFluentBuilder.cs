using MessageRouter.Addresses;
using MessageRouter.Publishers;
using MessageRouter.Receivers;
using MessageRouter.Requests;
using MessageRouter.Senders;
using MessageRouter.Subscribers;
using MessageRouter.Topics;
using MessageRouter.Transport;

namespace MessageRouter.Fluent
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