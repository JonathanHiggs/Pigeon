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
        private UnityContainerAdapter container;

        public UnityBuilder(UnityContainerAdapter container)
        {
            this.container = container;
        }

        public static ContainerBuilder Named(string name)
        {
            var container = new UnityContainerAdapter(new UnityContainer());
            return new ContainerBuilder(name, container);
        }

        public static INamedBuilder<ContainerBuilder> FromContainer(UnityContainerAdapter container)
        {
            return new UnityBuilder(container);
        }
        
        public ContainerBuilder WithName(string name)
        {
            return new ContainerBuilder(name, container);
        }
    }
}
