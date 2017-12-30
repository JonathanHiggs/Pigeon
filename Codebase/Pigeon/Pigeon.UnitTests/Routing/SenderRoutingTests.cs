using Pigeon.Addresses;
using Pigeon.Routing;
using Pigeon.Senders;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pigeon.UnitTests.Routing
{
    [TestFixture]
    public class SenderRoutingTests
    {
        private readonly IAddress address = TcpAddress.Localhost(5555);
        

        [Test]
        public void For_WithNullAddress_ThrowsArgumentNullException()
        {
            // Act
            TestDelegate create = () => SenderRouting.For<ISender>(null);

            // Assert
            Assert.That(create, Throws.ArgumentNullException);
        }


        [Test]
        public void For_WithAddress_HasSenderType()
        {
            // Arrange
            var routing = SenderRouting.For<ISender>(address);

            // Act
            var senderType = routing.SenderType;

            // Assert
            Assert.That(senderType, Is.EqualTo(typeof(ISender)));
        }


        [Test]
        public void For_WithAddress_ReturnsSameAddress()
        {
            // Arrange
            var routing = SenderRouting.For<ISender>(address);

            // Act
            var senderAddress = routing.Address;

            // Assert
            Assert.That(senderAddress, Is.EqualTo(address));
        }
    }
}
