using System;

using Pigeon.Addresses;
using Pigeon.Routing;
using Pigeon.Senders;

namespace Pigeon.Fluent.Transport
{
    public class SenderSetup<TSender> : ISenderSetup
        where TSender : ISender
    {
        private IAddress address;
        private IRequestRouter router;

        public SenderSetup(IRequestRouter router, IAddress address)
        {
            this.router = router ?? throw new ArgumentNullException(nameof(router));
            this.address = address ?? throw new ArgumentNullException(nameof(IAddress));
        }

        public ISenderSetup For<TRequest>()
        {
            router.AddRequestRouting<TRequest, TSender>(address);
            return this;
        }
    }
    
}
