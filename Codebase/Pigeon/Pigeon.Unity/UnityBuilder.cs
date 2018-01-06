using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pigeon.Fluent;
using Unity;

namespace Pigeon.Unity
{
    public class UnityBuilder : INamedBuilder<ContainerBuilder>
    {
        private UnityContainerWrapper container;

        public UnityBuilder(UnityContainerWrapper container)
        {
            this.container = container;
        }

        public static ContainerBuilder Named(string name)
        {
            var container = new UnityContainerWrapper(new UnityContainer());
            return new ContainerBuilder(name, container);
        }

        public static INamedBuilder<ContainerBuilder> FromContainer(UnityContainerWrapper container)
        {
            return new UnityBuilder(container);
        }
        
        public ContainerBuilder WithName(string name)
        {
            return new ContainerBuilder(name, container);
        }
    }
}
