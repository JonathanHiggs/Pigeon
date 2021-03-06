﻿using Pigeon.Requests;
using Pigeon.Topics;

namespace Pigeon.Fluent.Handlers
{
    public interface IHandlerSetup
    {
        IHandlerSetup WithRequestHandler<TRequest, TResponse, THandler>()
            where TRequest : class
            where TResponse : class
            where THandler : IRequestHandler<TRequest, TResponse>;


        IHandlerSetup WithAsyncRequestHandler<TRequest, TResponse, THandler>()
            where TRequest : class
            where TResponse : class
            where THandler : IAsyncRequestHandler<TRequest, TResponse>;


        IHandlerSetup WithRequestHandler<TRequest, TResponse>(IRequestHandler<TRequest, TResponse> handler)
            where TRequest : class
            where TResponse : class;


        IHandlerSetup WithRequestHandler<TRequest, TResponse>(IAsyncRequestHandler<TRequest, TResponse> handler)
            where TRequest : class
            where TResponse : class;


        IHandlerSetup WithRequestHandler<TRequest, TResponse>(RequestHandlerDelegate<TRequest, TResponse> handler)
            where TRequest : class
            where TResponse : class;


        IHandlerSetup WithAsyncRequestHandler<TRequest, TResponse>(AsyncRequestHandlerDelegate<TRequest, TResponse> handler)
            where TRequest : class
            where TResponse : class;


        IHandlerSetup WithTopicHandler<TTopic, THandler>()
            where TTopic : class
            where THandler : ITopicHandler<TTopic>;


        IHandlerSetup WithAsyncTopicHandler<TTopic, THandler>()
            where TTopic : class
            where THandler : IAsyncTopicHandler<TTopic>;


        IHandlerSetup WithTopicHandler<TTopic, THandler>(ITopicHandler<TTopic> handler)
            where TTopic : class;


        IHandlerSetup WithTopicHandler<TTopic, THandler>(IAsyncTopicHandler<TTopic> handler)
            where TTopic : class;


        IHandlerSetup WithTopicHandler<TTopic>(TopicHandlerDelegate<TTopic> handler)
            where TTopic : class;


        IHandlerSetup WithTopicHandler<TTopic>(AsyncTopicHandlerDelegate<TTopic> handler)
            where TTopic : class;
    }
}