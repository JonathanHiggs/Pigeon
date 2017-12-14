using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessageRouter.Messages
{
    /// <summary>
    /// Interface defines an abstract factory to control the creation and extraction of data from <see cref="Message"/>s
    /// </summary>
    public interface IMessageFactory
    {
        /// <summary>
        /// Wraps the supplied object in a <see cref="Message"/>
        /// </summary>
        /// <typeparam name="TMessage">Type of the wrapped message object</typeparam>
        /// <param name="message">Message object</param>
        /// <returns>Serializable Message wrapping the object</returns>
        Message CreateMessage<TMessage>(TMessage message) where TMessage : class;
        

        /// <summary>
        /// Wraps the supplied request object in a <see cref="Message"/>
        /// </summary>
        /// <param name="message">Message object</param>
        /// <returns>Serializable Message wrapping the object</returns>
        Message CreateMessage(object message);


        /// <summary>
        /// Extracts an object from the supplied <see cref="Message"/>
        /// </summary>
        /// <param name="message">Message wrapper</param>
        /// <returns>Message object</returns>
        object ExtractMessage(Message message);


        /// <summary>
        /// Extracts an object from the supplied <see cref="Message"/>. An exception will be throw if the inner message object is an unexpected type
        /// ToDo: Add Action<Exception> exception handler
        /// </summary>
        /// <typeparam name="TMessage">Type of the message object</typeparam>
        /// <param name="message">Message wrapper</param>
        /// <returns>Message object</returns>
        TMessage ExtractMessage<TMessage>(Message message) where TMessage : class;
    }
}
