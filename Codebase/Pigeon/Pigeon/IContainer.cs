using System.Collections.Generic;

namespace Pigeon
{
    /// <summary>
    /// Adapter around a IoC container for registering and resolving components at runtime
    /// </summary>
    public interface IContainer
    {
        // ToDo: Split this into IDIRegistrar and IResolver

        /// <summary>
        /// Checks whether the specified type is already registered
        /// </summary>
        /// <typeparam name="T">Type to check</typeparam>
        /// <returns>true if the specified type is registered; false otherwise</returns>
        bool IsRegistered<T>();


        /// <summary>
        /// Registers the specified type
        /// </summary>
        /// <typeparam name="T">Type to register</typeparam>
        /// <param name="singleton">If true the type is registered as a single instance</param>
        /// <returns>Returns container for fluency</returns>
        IContainer Register<T>(bool singleton);


        /// <summary>
        /// Registers the specified instance to be resolved when the specified type is requested
        /// </summary>
        /// <typeparam name="T">Type to register the instance as</typeparam>
        /// <param name="instance">Instance to return when resolved</param>
        /// <returns>Returns container for fluency</returns>
        IContainer Register<T>(T instance);


        /// <summary>
        /// Registers the specified implementation type to be resolved when the specified base type is requested
        /// </summary>
        /// <typeparam name="TBase">Type of requested object</typeparam>
        /// <typeparam name="TImpl">Type of returned object</typeparam>
        /// <param name="singleton">If true the type is registered as a single instance</param>
        /// <returns>Returns container for fluency</returns>
        IContainer Register<TBase, TImpl>(bool singleton) where TImpl : TBase;


        /// <summary>
        /// Returns an instance of the requested type
        /// </summary>
        /// <typeparam name="T">Type to resolve</typeparam>
        /// <returns></returns>
        T Resolve<T>();


        /// <summary>
        /// Returns all registered instanced of the requested type
        /// </summary>
        /// <typeparam name="T">Type to resolve</typeparam>
        /// <returns></returns>
        IEnumerable<T> ResolveAll<T>();
    }
}
