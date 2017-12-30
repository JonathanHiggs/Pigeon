using Pigeon.Addresses;
using Pigeon.Routing;
using Pigeon.Subscribers;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pigeon.UnitTests.Routing
{
    [TestFixture]
    public class SubscriberRoutingTests
    {
        private readonly IAddress address = TcpAddress.Localhost(5555);


        [Test]
        public void For_WithNullAddress_ThrowsArgumentNullException()
        {
            // Act
            TestDelegate create = () => SubscriberRouting.For<ISubscriber>(null);

            // Assert
            Assert.That(create, Throws.ArgumentNullException);
        }


        [Test]
        public void For_WithAddress_HasSubscriberType()
        {
            // Arrange
            var routing = SubscriberRouting.For<ISubscriber>(address);

            // Act
            var subscriberType = routing.SubscriberType;

            // Assert
            Assert.That(subscriberType, Is.EqualTo(typeof(ISubscriber)));
        }


        [Test]
        public void For_WithAddress_ReturnsSameAddress()
        {
            // Arrange
            var routing = SubscriberRouting.For<ISubscriber>(address);

            // Act
            var subscriberAddress = routing.Address;

            // Assert
            Assert.That(subscriberAddress, Is.EqualTo(address));
        }
    }
}
