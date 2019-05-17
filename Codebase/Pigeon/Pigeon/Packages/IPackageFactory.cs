using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pigeon.Packages
{
    /// <summary>
    /// Interface defines an abstract factory to control the creation and extraction of data from <see cref="Package"/>s
    /// </summary>
    public interface IPackageFactory
    {
        /// <summary>
        /// Wraps the supplied object in a <see cref="Package"/>
        /// </summary>
        /// <typeparam name="TMessage">Type of the wrapped message object</typeparam>
        /// <param name="message">Message object</param>
        /// <returns>Serializable <see cref="Package"/> wrapping the object</returns>
        Package Pack<TMessage>(TMessage message) where TMessage : class;
        

        /// <summary>
        /// Wraps the supplied request object in a <see cref="Package"/>
        /// </summary>
        /// <param name="message">Message object</param>
        /// <returns>Serializable <see cref="Package"/> wrapping the object</returns>
        Package Pack(object message);


        /// <summary>
        /// Extracts a message object from the supplied <see cref="Package"/>. An exception will be throw if the message response is an unexpected type
        /// </summary>
        /// <param name="package">Packed message wrapper</param>
        /// <returns>Message object</returns>
        object Unpack(Package package);


        /// <summary>
        /// Extracts a message object from the supplied <see cref="Package"/>. An exception will be throw if the inner message object is an unexpected type
        /// </summary>
        /// <typeparam name="TMessage">Type of the message object</typeparam>
        /// <param name="package">Message wrapper</param>
        /// <returns>Message object</returns>
        TMessage Unpack<TMessage>(Package package) where TMessage : class;
    }
}
