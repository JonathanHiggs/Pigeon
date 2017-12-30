using Pigeon.Addresses;
using Pigeon.Diagnostics;
using Pigeon.Routing;
using Pigeon.Senders;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pigeon.UnitTests.Routing
{
    [TestFixture]
    public class RequestRouterTests
    {
        class Request { }
        class SubRequest : Request { }

        private readonly IAddress address = TcpAddress.Wildcard(5555);
        private readonly IAddress address2 = TcpAddress.Wildcard(5556);


        #region AddRequestRouting
        [Test]
        public void AddRequestRouting_WhenNotAlreadyAdded_AddsToRoutingTable()
        {
            // Arrange
            var router = new RequestRouter();

            // Act
            router.AddRequestRouting<Request, ISender>(address);

            // Assert
            Assert.That(router.RoutingTable.ContainsKey(typeof(Request)), Is.True);
        }


        [Test]
        public void AddRequestRouting_WithDifferentAddressToExistingRouting_ThrowsRoutingAlreadyRegisteredException()
        {
            // Arrange
            var router = new RequestRouter();
            router.AddRequestRouting<Request, ISender>(address);

            // Act
            TestDelegate addRouting = () => router.AddRequestRouting<Request, ISender>(address2);

            // Assert
            Assert.That(addRouting, Throws.TypeOf<RoutingAlreadyRegisteredException<SenderRouting>>());
        }


        [Test]
        public void AddRequestRouting_WithSameAddressAsExistingRouting_DoesNothing()
        {
            // Arrange
            var router = new RequestRouter();
            router.AddRequestRouting<Request, ISender>(address);

            // Act
            TestDelegate addTopicRouting = () => router.AddRequestRouting<Request, ISender>(address);

            // Assert
            Assert.That(addTopicRouting, Throws.Nothing);
        }


        [Test]
        public void AddRequestRouting_WithNullAddress_ThrowsArgumentNullException()
        {
            // Arrange
            var router = new RequestRouter();

            // Act
            TestDelegate addRouting = () => router.AddRequestRouting<Request, ISender>(null);

            // Assert
            Assert.That(addRouting, Throws.ArgumentNullException);
        }


        [Test]
        public void AddRequestRouting_WithRequestBaseClassAdded_AddsToRoutingTable()
        {
            // Arrange
            var router = new RequestRouter();
            router.AddRequestRouting<Request, ISender>(address);

            // Act
            router.AddRequestRouting<SubRequest, ISender>(address);

            // Assert
            Assert.That(router.RoutingTable.ContainsKey(typeof(SubRequest)), Is.True);
        }
        #endregion


        #region RoutingFor
        [Test]
        public void RoutingFor_WithNoRouting_ReturnsFalse()
        {
            // Arrange
            var router = new RequestRouter();

            // Act
            var hasRouting = router.RoutingFor<Request>(out var routing);

            // Assert
            Assert.That(hasRouting, Is.False);
        }


        [Test]
        public void RoutingFor_WithRoutingAdded_ReturnsTrue()
        {
            // Arrange
            var router = new RequestRouter();
            router.AddRequestRouting<Request, ISender>(address);

            // Act
            var hasRouting = router.RoutingFor<Request>(out var routing);

            // Assert
            Assert.That(hasRouting, Is.True);
        }


        [Test]
        public void RoutingFor_WithRoutingAdded_ReturnsRoutingWithSameAddress()
        {
            // Arrange
            var router = new RequestRouter();
            router.AddRequestRouting<Request, ISender>(address);

            // Act
            var hasRouting = router.RoutingFor<Request>(out var routing);

            // Assert
            Assert.That(routing.Address, Is.EqualTo(address));
        }


        [Test]
        public void RoutingFor_WithRoutingAdded_ReturnsRoutingWithSameSenderType()
        {
            // Arrange
            var router = new RequestRouter();
            router.AddRequestRouting<Request, ISender>(address);

            // Act
            var hasRouting = router.RoutingFor<Request>(out var routing);

            // Assert
            Assert.That(routing.SenderType, Is.EqualTo(typeof(ISender)));
        }


        [Test]
        public void RoutingFor_WithBaseClassRoutingAdded_ReturnsFalse()
        {
            // Arrange
            var router = new RequestRouter();
            router.AddRequestRouting<Request, ISender>(address);

            // Act
            var hasRouting = router.RoutingFor<SubRequest>(out var routing);

            // Assert
            Assert.That(hasRouting, Is.False);
        }


        [Test]
        public void RoutingFor_WithSubClassRoutingAdded_ReturnsFalse()
        {
            // Arrange
            var router = new RequestRouter();
            router.AddRequestRouting<SubRequest, ISender>(address);

            // Act
            var hasRouting = router.RoutingFor<Request>(out var routing);

            // Assert
            Assert.That(hasRouting, Is.False);
        }
        #endregion
    }
}
