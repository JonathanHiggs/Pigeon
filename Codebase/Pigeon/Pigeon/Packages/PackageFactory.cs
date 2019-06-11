using System;
using System.Linq;
using System.Reflection;

namespace Pigeon.Packages
{
    /// <summary>
    /// Default implementation of <see cref="IPackageFactory"/>, wraps messages in <see cref="Package"/>s
    /// </summary>
    public class PackageFactory : IPackageFactory
    {
        private readonly MethodInfo unboundCreateResponse;


        /// <summary>
        /// Initializes a new instance of <see cref="PackageFactory"/>
        /// </summary>
        public PackageFactory()
        {
            unboundCreateResponse = GetType()
                .GetMethods(BindingFlags.Public | BindingFlags.Instance)
                .Single(m => m.Name == nameof(Pack) && m.IsGenericMethod);
        }


        /// <summary>
        /// Wraps the supplied object in a <see cref="Package"/>
        /// </summary>
        /// <typeparam name="TMessage">Type of the wrapped message object</typeparam>
        /// <param name="message">Message object</param>
        /// <returns>Serializable <see cref="Package"/> wrapping the object</returns>
        public Package Pack<TMessage>(TMessage message) where TMessage : class
        {
            return Create(message);
        }


        /// <summary>
        /// Wraps the supplied request object in a <see cref="Package"/>
        /// </summary>
        /// <param name="message">Message object</param>
        /// <returns>Serializable <see cref="Package"/> wrapping the object</returns>
        public Package Pack(object message)
        {
            var method = unboundCreateResponse.MakeGenericMethod(message.GetType());
            return (Package)method.Invoke(this, new object[] { message });
        }


        /// <summary>
        /// Extracts a message object from the supplied <see cref="Package"/>. An exception will be throw if the message response is an unexpected type
        /// </summary>
        /// <param name="package">Packed message wrapper</param>
        /// <returns>Message object</returns>
        public object Unpack(Package package)
        {
            return package.Body;
        }


        /// <summary>
        /// Extracts a message object from the supplied <see cref="Package"/>. An exception will be throw if the inner message object is an unexpected type
        /// </summary>
        /// <typeparam name="TMessage">Type of the message object</typeparam>
        /// <param name="package">Message wrapper</param>
        /// <returns>Message object</returns>
        public TMessage Unpack<TMessage>(Package package) where TMessage : class
        {
            if (typeof(TMessage).IsAssignableFrom(package.GetType()) && typeof(TMessage) != typeof(object))
                return package as TMessage;

            else if (package is DataPackage<TMessage>)
                return (package as DataPackage<TMessage>).Data;

            else if (package is ExceptionPackage)
                throw (package as ExceptionPackage).Exception;

            else
                throw new InvalidCastException($"Unable to extract type {typeof(TMessage).Name} from message");
        }


        private Package Create<T>(T data)
            where T : class
        {
            if (data is Package)
                return data as Package;
            else if (data is Exception)
                return new ExceptionPackage(new GuidPackageId(), data as Exception);
            return new DataPackage<T>(new GuidPackageId(), data);
        }
    }
}
