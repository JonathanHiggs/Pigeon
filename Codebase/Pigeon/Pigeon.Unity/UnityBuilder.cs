using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pigeon.Fluent;
using Unity;

namespace Pigeon.Unity
{
    public interface INamedBuilder<TBuilder>
    {
        IFluentBuilder<TBuilder> WithName(string name);
    }

    public class UnityBuilder : INamedBuilder<DependencyInjectionBuilder>
    {
        private UnityContainerWrapper container;

        public UnityBuilder(UnityContainerWrapper container)
        {
            this.container = container;
        }

        public static DependencyInjectionBuilder Named(string name)
        {
            var container = new UnityContainerWrapper(new UnityContainer());
            return new DependencyInjectionBuilder(name, container);
        }

        public static INamedBuilder<DependencyInjectionBuilder> FromContainer(UnityContainerWrapper container)
        {
            return new UnityBuilder(container);
        }
        
        public IFluentBuilder<DependencyInjectionBuilder> WithName(string name)
        {
            return new DependencyInjectionBuilder(name, container);
        }
    }
}
