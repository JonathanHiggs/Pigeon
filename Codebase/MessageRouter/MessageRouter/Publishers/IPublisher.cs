using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MessageRouter.Addresses;
using MessageRouter.Packages;
using MessageRouter.Subscribers;
using MessageRouter.Transport;

namespace MessageRouter.Publishers
{
    /// <summary>
    /// Interface encapsulates a connection that is able to publish <see cref="Package"/>s to <see cref="ISubscriber"/>s
    /// </summary>
    public interface IPublisher : IEndPoint
    {
        /// <summary>
        /// Adds an <see cref="IAddress"/> to the collection of endpoints to which the <see cref="IPublisher"/> publishes <see cref="Package"/>s
        /// </summary>
        /// <param name="address"></param>
        void AddAddress(IAddress address);


        /// <summary>
        /// Removes an <see cref="IAddress"/> from the collection of endpoints to which the <see cref="IPublisher"/> publishes <see cref="Package"/>s
        /// </summary>
        /// <param name="address"></param>
        void RemoveAddress(IAddress address);


        /// <summary>
        /// Initializes the bindings to which the <see cref="IPublisher"/> publishes <see cref="Package"/>s allowing <see cref="ISubscribers"/>s to receive them
        /// </summary>
        void BindAll();


        /// <summary>
        /// Terminates the bindings to which the <see cref="IPublisher"/> publishes <see cref="Package"/>s preventing <see cref="ISubscriber"/>s from receiving them
        /// </summary>
        void UnbindAll();


        /// <summary>
        /// Transmits the <see cref="Package"/> to all connected <see cref="ISubscriber"/>s
        /// </summary>
        /// <param name="package"></param>
        void Publish(Package package);
    }
}
