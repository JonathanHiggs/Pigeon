namespace Pigeon
{
    /// <summary>
    /// Adapter around a IoC container for registering and resolving components at runtime
    /// </summary>
    public interface IContainer
    {
        /// <summary>
        /// Checks whether the speficied type is already registered
        /// </summary>
        /// <typeparam name="T">Type to check</typeparam>
        /// <returns>true if the specified type is registered; false otherwise</returns>
        bool IsRegistered<T>();


        /// <summary>
        /// Registers the specified type
        /// </summary>
        /// <typeparam name="T">Type to register</typeparam>
        /// <param name="singleton">If true the type is registered as a single instance</param>
        void Register<T>(bool singleton);


        /// <summary>
        /// Registers the speficied isntance to be resolved when the specified type is requested
        /// </summary>
        /// <typeparam name="T">Type to register the instance as</typeparam>
        /// <param name="instance">Instance to return when resolved</param>
        void Register<T>(T instance);


        /// <summary>
        /// Registers the specified implementation type to be resolved when the specified base type is requested
        /// </summary>
        /// <typeparam name="TBase">Type of requested object</typeparam>
        /// <typeparam name="TImpl">Type of returned object</typeparam>
        /// <param name="singleton">If true the type is registered as a single instance</param>
        void Register<TBase, TImpl>(bool singleton) where TImpl : TBase;


        /// <summary>
        /// Initializes a new instance of the requested type
        /// </summary>
        /// <typeparam name="T">Type to initialize</typeparam>
        /// <returns>Initializes instance of T</returns>
        T Resolve<T>();
    }
}
