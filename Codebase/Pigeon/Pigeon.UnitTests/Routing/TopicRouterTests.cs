using Pigeon.Addresses;
using Pigeon.Diagnostics;
using Pigeon.Routing;
using Pigeon.Subscribers;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pigeon.UnitTests.TestFixtures;

namespace Pigeon.UnitTests.Routing
{
    [TestFixture]
    public class TopicRouterTests
    {
        private readonly IAddress address = TcpAddress.Localhost(5555);
        private readonly IAddress address2 = TcpAddress.Localhost(5556);


        #region AddTopicRouting
        [Test]
        public void AddTopicRouting_WhenNotAlreadyAdded_AddsToRoutingTable()
        {
            // Arrange
            var router = new TopicRouter();

            // Act
            router.AddTopicRouting<Topic, ISubscriber>(address);

            // Assert
            Assert.That(router.RoutingTable.ContainsKey(typeof(Topic)), Is.True);
        }


        [Test]
        public void AddTopicRouting_WithDifferentAddressToExistingRouting_ThrowsRoutingAlreadyRegisteredException()
        {
            // Arrange
            var router = new TopicRouter();
            router.AddTopicRouting<Topic, ISubscriber>(address);

            // Act
            TestDelegate addTopicRouting = () => router.AddTopicRouting<Topic, ISubscriber>(address2);

            // Assert
            Assert.That(addTopicRouting, Throws.TypeOf<RoutingAlreadyRegisteredException<SubscriberRouting>>());
        }


        [Test]
        public void AddTopicRouting_WithSameAddressAsExistingRouting_DoesNothing()
        {
            // Arrange
            var router = new TopicRouter();
            router.AddTopicRouting<Topic, ISubscriber>(address);

            // Act
            TestDelegate addTopicRouting = () => router.AddTopicRouting<Topic, ISubscriber>(address);

            // Assert
            Assert.That(addTopicRouting, Throws.Nothing);
        }


        [Test]
        public void AddTopicRouting_WithNullAddress_ThrowsArgumentNullException()
        {
            // Arrange
            var router = new TopicRouter();

            // Act
            TestDelegate addTopicRouting = () => router.AddTopicRouting<Topic, ISubscriber>(null);

            // Assert
            Assert.That(addTopicRouting, Throws.ArgumentNullException);
        }


        [Test]
        public void AddTopicRouting_WithTopicBaseClassAdded_AddsToRoutingTable()
        {
            // Arrange
            var router = new TopicRouter();
            router.AddTopicRouting<Topic, ISubscriber>(address);

            // Act
            router.AddTopicRouting<SubTopic, ISubscriber>(address);

            // Assert
            Assert.That(router.RoutingTable.ContainsKey(typeof(SubTopic)), Is.True);
        }
        #endregion


        #region RoutingFor
        [Test]
        public void RoutingFor_WithNoRouting_ReturnsFalse()
        {
            // Arrange
            var router = new TopicRouter();

            // Act
            var hasRouting = router.RoutingFor<Topic>(out var routing);

            // Assert
            Assert.That(hasRouting, Is.False);
        }


        [Test]
        public void RoutingFor_WithRoutingAdded_ReturnsTrue()
        {
            // Arrange
            var router = new TopicRouter();
            router.AddTopicRouting<Topic, ISubscriber>(address);

            // Act
            var hasRouting = router.RoutingFor<Topic>(out var routing);

            // Assert
            Assert.That(hasRouting, Is.True);
        }


        [Test]
        public void RoutingFor_WithRoutingAdded_ReturnsRoutingWithSameAddress()
        {
            // Arrange
            var router = new TopicRouter();
            router.AddTopicRouting<Topic, ISubscriber>(address);

            // Act
            var hasRouting = router.RoutingFor<Topic>(out var routing);

            // Assert
            Assert.That(routing.Address, Is.EqualTo(address));
        }


        [Test]
        public void RoutingFor_WithRoutingAdded_ReturnsRoutingWithSameSubscriberType()
        {
            // Arrange
            var router = new TopicRouter();
            router.AddTopicRouting<Topic, ISubscriber>(address);

            // Act
            var hasRouting = router.RoutingFor<Topic>(out var routing);

            // Assert
            Assert.That(routing.SubscriberType, Is.EqualTo(typeof(ISubscriber)));
        }


        [Test]
        public void RoutingFor_WithBaseClassRoutingAdded_ReturnsFalse()
        {
            // Arrange
            var router = new TopicRouter();
            router.AddTopicRouting<Topic, ISubscriber>(address);

            // Act
            var hasRouting = router.RoutingFor<SubTopic>(out var routing);

            // Assert
            Assert.That(hasRouting, Is.False);
        }


        [Test]
        public void RoutingFor_WithSubClassRoutingAdded_ReturnsFalse()
        {
            // Arrange
            var router = new TopicRouter();
            router.AddTopicRouting<SubTopic, ISubscriber>(address);

            // Act
            var hasRouting = router.RoutingFor<Topic>(out var routing);

            // Assert
            Assert.That(hasRouting, Is.False);
        }
        #endregion
    }
}
