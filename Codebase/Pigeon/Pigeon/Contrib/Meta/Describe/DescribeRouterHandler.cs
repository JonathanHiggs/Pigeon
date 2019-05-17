using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Pigeon.Publishers;
using Pigeon.Receivers;
using Pigeon.Requests;

namespace Pigeon.Contrib.Meta.Describe
{
    namespace v1_0 // Version namespaces to allow future backward compatibility
    {
        /// <summary>
        /// Handler for <see cref="DescribeRouter"/> request
        /// </summary>
        public class DescribeRouterHandler : IRequestHandler<DescribeRouter, RouterDescription>
        {
            private readonly Router router;
            private readonly IReceiverCache receiverCache;
            private readonly IPublisherCache publisherCache;


            /// <summary>
            /// Initializes a new instance of <see cref="DescribeRouterHandler"/>
            /// </summary>
            /// <param name="router"><see cref="Router"/> to get the meta description of</param>
            /// <param name="receiverCache">Cache of <see cref="IReceiver"/>s</param>
            /// <param name="publisherCache">Cache of <see cref="IPublisher"/>s</param>
            public DescribeRouterHandler(Router router, IReceiverCache receiverCache, IPublisherCache publisherCache)
            {
                this.router = router ?? throw new ArgumentNullException(nameof(router));
                this.receiverCache = receiverCache ?? throw new ArgumentNullException(nameof(receiverCache));
                this.publisherCache = publisherCache ?? throw new ArgumentNullException(nameof(publisherCache));
            }


            /// <summary>
            /// Prepares the response to a <see cref="DescribeRouter"/> request 
            /// </summary>
            /// <param name="request">Request object</param>
            /// <returns>Description of a <see cref="Router"/></returns>
            public RouterDescription Handle(DescribeRouter request)
            {
                return new RouterDescription(
                    router.Identity,
                    router.Info,
                    receiverCache.Receivers.Select(r => r.Meta),
                    publisherCache.Publishers.Select(p => p.Meta)
                );
            }
        }
    }
}
