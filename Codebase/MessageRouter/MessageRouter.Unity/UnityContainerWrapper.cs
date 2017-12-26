using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Practices.Unity;
using Unity;

namespace MessageRouter.Unity
{
    public class UnityContainerWrapper : IContainer
    {
        private readonly UnityContainer unityContainer;


        public UnityContainerWrapper(UnityContainer unityContainer)
        {
            this.unityContainer = unityContainer ?? throw new ArgumentNullException(nameof(unityContainer));
            Register<IContainer>(this);
        }


        public void Register<T>(bool singleton)
        {
            if (singleton)
                unityContainer.RegisterSingleton<T>();
            else
                unityContainer.RegisterType<T>();
        }


        public void Register<T>(T instance)
        {
            unityContainer.RegisterInstance(instance);
        }


        public void Register<TBase, TImpl>(bool singleton) where TImpl : TBase
        {
            if (singleton)
                unityContainer.RegisterSingleton<TBase, TImpl>();
            else
                unityContainer.RegisterType<TBase, TImpl>();
        }


        public T Resolve<T>()
        {
            return unityContainer.Resolve<T>();
        }
    }
}
