using System;
using Pigeon.Fluent.Handlers;

namespace Pigeon.Fluent
{
    public interface IHandlerBuilder : IRouterBuilder
    {
        IHandlerBuilder WithHandlers(Action<IHandlerSetup> config);
    }
}
