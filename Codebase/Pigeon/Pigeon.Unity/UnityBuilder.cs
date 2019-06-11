using Pigeon.Fluent;

using Unity;

namespace Pigeon.Unity
{
    public class UnityBuilder : INamedBuilder<ContainerBuilder>
    {
        private UnityContainerAdapter adapter;

        public UnityBuilder(UnityContainer container)
            : this(new UnityContainerAdapter(container))
        { }


        private UnityBuilder(UnityContainerAdapter adapter)
        {
            this.adapter = adapter;
        }


        public static ContainerBuilder Named(string name)
        {
            var container = new UnityContainerAdapter(new UnityContainer());
            return new ContainerBuilder(name, container);
        }


        public static INamedBuilder<ContainerBuilder> FromContainer(UnityContainer container)
        {
            return new UnityBuilder(container);
        }
        

        public ContainerBuilder WithName(string name)
        {
            return new ContainerBuilder(name, adapter);
        }
    }
}
