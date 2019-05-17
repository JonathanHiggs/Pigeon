using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Pigeon.Addresses;
using Pigeon.Publishers;
using Pigeon.Receivers;

namespace Pigeon.Contrib.Meta.Describe
{
    namespace v1_0 // Version namespaces to allow future backward compatibility
    {
        /// <summary>
        /// Response object for the <see cref="DescribeRouter"/> request
        /// </summary>
        [Serializable]
        public class RouterDescription
        {
            /// <summary>
            /// Gets the <see cref="Router"/>'s <see cref="NodeIdentity"/>
            /// </summary>
            public NodeIdentity Identity { get; private set; }


            /// <summary>
            /// Gets the runtime <see cref="RouterInfo"/>
            /// </summary>
            public RouterInfo RuntimeInfo { get; private set; }


            /// <summary>
            /// Gets a list of metadata for the <see cref="Router"/>'s <see cref="IReceiver"/>s
            /// </summary>
            public List<ReceiverMeta> Receivers { get; private set; }


            /// <summary>
            /// Gets a list of metadata for the <see cref="Router"/>'s <see cref="IPublisher"/>s
            /// </summary>
            public List<PublisherMeta> Publishers { get; private set; }


            /// <summary>
            /// Initializes a new instance of <see cref="RouterDescription"/>
            /// </summary>
            /// <param name="identity">Meta identity</param>
            /// <param name="info">Runtime info</param>
            /// <param name="receivers">List of metadata for the <see cref="Router"/>'s <see cref="IReceiver"/>s</param>
            /// <param name="publishers">List of metadata for the <see cref="Router"/>'s <see cref="IPublisher"/>s</param>
            public RouterDescription(NodeIdentity identity, RouterInfo info, IEnumerable<ReceiverMeta> receivers, IEnumerable<PublisherMeta> publishers)
            {
                Identity = identity;
                RuntimeInfo = info;
                Receivers = receivers.ToList();
                Publishers = publishers.ToList();
            }
        }
    }
}
