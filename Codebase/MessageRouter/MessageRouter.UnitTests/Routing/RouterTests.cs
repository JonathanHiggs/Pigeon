using MessageRouter.Addresses;
using MessageRouter.Routing;
using MessageRouter.Senders;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessageRouter.UnitTests.Routing
{
    [TestFixture]
    public class RouterTests
    {
        class Request { }
        class RequestSubClass : Request { }


        [Test]
        public void AddSenderRouting_WhenNotAlreadyAdded_AddsToRoutingTable()
        {
            // Arrange
            var router = new Router();

            // Act
            router.AddSenderRouting<Request, ISender>(TcpAddress.Wildcard(5555));

            // Assert
            Assert.That(router.RoutingTable.ContainsKey(typeof(Request)), Is.True);
        }


        [Test]
        public void AddSenderRouting_WithExistingRouting_ThrowsRoutingAlreadyRegisteredException()
        {
            // Arrange
            var router = new Router();
            router.AddSenderRouting<Request, ISender>(TcpAddress.Wildcard(5555));

            // Act
            TestDelegate addRouting = () => router.AddSenderRouting<Request, ISender>(TcpAddress.Wildcard(5556));

            // Assert
            Assert.That(addRouting, Throws.TypeOf<RoutingAlreadyRegisteredException>());
        }


        [Test]
        public void AddSenderRouting_WithNullAddress_ThrowsArgumentNullException()
        {
            // Arrange
            var router = new Router();

            // Act
            TestDelegate addRouting = () => router.AddSenderRouting<Request, ISender>(null);

            // Assert
            Assert.That(addRouting, Throws.ArgumentNullException);
        }


        [Test]
        public void RoutingFor_WithNoRouting_ReturnsFalse()
        {
            // Arrange
            var router = new Router();

            // Act
            var hasRouting = router.RoutingFor<Request>(out var mapping);

            // Assert
            Assert.That(hasRouting, Is.False);
        }


        [Test]
        public void RoutingFor_WithRoutingAdded_ReturnsTrue()
        {
            // Arrange
            var router = new Router();
            router.AddSenderRouting<Request, ISender>(TcpAddress.Wildcard(5555));

            // Act
            var hasRouting = router.RoutingFor<Request>(out var mapping);

            // Assert
            Assert.That(hasRouting, Is.True);
        }


        [Test]
        public void RoutingFor_WithRoutingAdded_ReturnsRoutingWithCorrectAddress()
        {
            // Arrange
            var address = TcpAddress.Wildcard(5555);
            var router = new Router();
            router.AddSenderRouting<Request, ISender>(address);

            // Act
            var hasRouting = router.RoutingFor<Request>(out var routing);

            // Assert
            Assert.That(routing.Address, Is.EqualTo(address));
        }


        [Test]
        public void RoutingFor_WithSubClassRegistered_ReturnsFalse()
        {
            // Arrange
            var router = new Router();
            router.AddSenderRouting<Request, ISender>(TcpAddress.Wildcard(5555));

            // Act
            var hasRouting = router.RoutingFor<RequestSubClass>(out var mapping);

            // Assert
            Assert.That(hasRouting, Is.False);
        }
    }
}
