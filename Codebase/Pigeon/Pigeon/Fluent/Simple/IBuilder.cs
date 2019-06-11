using Pigeon.Addresses;
using Pigeon.Publishers;
using Pigeon.Receivers;
using Pigeon.Requests;
using Pigeon.Senders;
using Pigeon.Subscribers;
using Pigeon.Transport;

namespace Pigeon.Fluent.Simple
{
    public interface IFluentBuilder<TBuilder>
    {
        Router Build();

        Router BuildAndStart();

        TBuilder WithPublisher<TPublisher>(IAddress address) where TPublisher : IPublisher;

        TBuilder WithReceiver<TReceiver>(IAddress address) where TReceiver : IReceiver;

        TBuilder WithRequestHandler<TRequest, TResponse>(IRequestHandler<TRequest, TResponse> handler)
            where TRequest : class
            where TResponse : class;

        TBuilder WithRequestHandler<TRequest, TResponse>(RequestHandlerDelegate<TRequest, TResponse> handler)
            where TRequest : class
            where TResponse : class;

        TBuilder WithAsyncRequestHandler<TRequest, TResponse>(AsyncRequestHandlerDelegate<TRequest, TResponse> handler)
            where TRequest : class
            where TResponse : class;

        TBuilder WithSenderRouting<TSender, TRequest>(IAddress address)
            where TSender : ISender
            where TRequest : class;

        TBuilder WithSubscriber<TSubscriber, TTopic>(IAddress address) where TSubscriber : ISubscriber;

        //TBuilder WithTopicHandler<TTopic>(ITopicHandler<TTopic> handler);

        //TBuilder WithTopicHandler<TTopic>(TopicHandlerDelegate<TTopic> handler);

        //TBuilder WithAsyncTopicHandler<TTopic>(AsyncTopicHandlerDelegate<TTopic> handler);

        TBuilder WithTransport<TTransport>() where TTransport : ITransportConfig;
    }
}