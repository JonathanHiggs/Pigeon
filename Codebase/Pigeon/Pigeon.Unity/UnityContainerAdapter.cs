using System;

using Unity;

namespace Pigeon.Unity
{
    /// <summary>
    /// Adapter around a Unity IoC container for registering and resolving components at runtime
    /// </summary>
    public class UnityContainerAdapter : IContainer, IDisposable
    {
        private readonly IUnityContainer unityContainer;


        /// <summary>
        /// Initializes a new instance of <see cref="UnityContainerAdapter"/>
        /// </summary>
        /// <param name="unityContainer">Inner <see cref="IUnityContainer"/> that is wrapped</param>
        public UnityContainerAdapter(IUnityContainer unityContainer)
        {
            this.unityContainer = unityContainer ?? throw new ArgumentNullException(nameof(unityContainer));
            Register<IContainer>(this);
        }


        /// <summary>
        /// Checks whether the specified type is already registered
        /// </summary>
        /// <typeparam name="T">Type to check</typeparam>
        /// <returns>true if the specified type is registered; false otherwise</returns>
        public bool IsRegistered<T>()
        {
            return unityContainer.IsRegistered<T>();
        }


        /// <summary>
        /// Registers the specified type
        /// </summary>
        /// <typeparam name="T">Type to register</typeparam>
        /// <param name="singleton">If true the type is registered as a single instance</param>
        /// <returns>Returns container for fluency</returns>
        public IContainer Register<T>(bool singleton)
        {
            if (singleton)
                unityContainer.RegisterSingleton<T>();
            else
                unityContainer.RegisterType<T>();

            return this;
        }


        /// <summary>
        /// Registers the specified instance to be resolved when the specified type is requested
        /// </summary>
        /// <typeparam name="T">Type to register the instance as</typeparam>
        /// <param name="instance">Instance to return when resolved</param>
        /// <returns>Returns container for fluency</returns>
        public IContainer Register<T>(T instance)
        {
            unityContainer.RegisterInstance(instance);
            return this;
        }


        /// <summary>
        /// Registers the specified implementation type to be resolved when the specified base type is requested
        /// </summary>
        /// <typeparam name="TBase">Type of requested object</typeparam>
        /// <typeparam name="TImpl">Type of returned object</typeparam>
        /// <param name="singleton">If true the type is registered as a single instance</param>
        /// <returns>Returns container for fluency</returns>
        public IContainer Register<TBase, TImpl>(bool singleton) where TImpl : TBase
        {
            if (singleton)
                unityContainer.RegisterSingleton<TBase, TImpl>();
            else
                unityContainer.RegisterType<TBase, TImpl>();
            return this;
        }


        /// <summary>
        /// Initializes a new instance of the requested type
        /// </summary>
        /// <typeparam name="T">Type to initialize</typeparam>
        /// <returns>Initializes instance of T</returns>
        public T Resolve<T>()
        {
            return unityContainer.Resolve<T>();
        }

        
        public void Dispose()
        {
            unityContainer.Dispose();
        }
    }
}
