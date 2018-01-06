using Pigeon.Requests;

namespace Pigeon.Fluent.Handlers
{
    public interface IHandlerSetup
    {
        IHandlerSetup WithRequestHandler<TRequest, TResponse, THandler>()
            where TRequest : class
            where TResponse : class
            where THandler : IRequestHandler<TRequest, TResponse>;

        IHandlerSetup WithRequestHandler<TRequest, TResponse>(IRequestHandler<TRequest, TResponse> handler)
            where TRequest : class
            where TResponse : class;

        IHandlerSetup WithRequestHandler<TRequest, TResponse>(RequestHandlerDelegate<TRequest, TResponse> handler)
            where TRequest : class
            where TResponse : class;

        IHandlerSetup WithAsyncRequestHandler<TRequest, TResponse>(AsyncRequestHandlerDelegate<TRequest, TResponse> handler)
            where TRequest : class
            where TResponse : class;
    }
}