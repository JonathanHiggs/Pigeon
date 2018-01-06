using System;
using Pigeon.Fluent.Transport;
using Pigeon.Transport;

namespace Pigeon.Fluent
{
    public interface ITransportBuilder : IHandlerBuilder
    {
        ITransportBuilder WithTransport<TTransport>(Action<ITransportSetup> config) where TTransport : ITransportConfig;
    }
}
