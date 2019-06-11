using NUnit.Framework;

using Pigeon.Fluent;
using Pigeon.Requests;
using Pigeon.Topics;
using Pigeon.Unity;

using Unity;

namespace Pigeon.IntegrationTests.Fluent
{
    [TestFixture]
    public class ContainerBuilderTests
    {
        private readonly string name = "name";

        [Test]
        public void ContainerBuilder_WithContainer_ContainerResolvesSameInstanceForTopicDispatchers()
        {
            // Arrange
            var container = new UnityContainer();
            var adapter = new UnityContainerAdapter(container);

            // Act
            var containerBuilder = new ContainerBuilder(name, adapter);
            var dispatcher = container.Resolve<ITopicDispatcher>();
            var dispatcher2 = container.Resolve<IDITopicDispatcher>();

            // Assert
            Assert.That(dispatcher, Is.SameAs(dispatcher2));
        }


        [Test]
        public void ContainerBuilder_WithContainer_ContainerResolvesSameInstanceForRequestDispatcher()
        {
            // Arrange
            var container = new UnityContainer();
            var adapter = new UnityContainerAdapter(container);

            // Act
            var containerBuilder = new ContainerBuilder(name, adapter);
            var dispatcher = container.Resolve<IRequestDispatcher>();
            var dispatcher2 = container.Resolve<IDIRequestDispatcher>();

            // Assert
            Assert.That(dispatcher, Is.SameAs(dispatcher2));
        }
    }
}
