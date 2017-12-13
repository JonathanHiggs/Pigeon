using MessageRouter.Addresses;
using MessageRouter.Routing;
using MessageRouter.Senders;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessageRouter.UnitTests.Routing
{
    [TestFixture]
    public class SenderRoutingTests
    {
        private readonly Mock<IAddress> mockAddress = new Mock<IAddress>();
        private IAddress address;


        [SetUp]
        public void Setup()
        {
            address = mockAddress.Object;

            mockAddress
                .Setup(m => m.ToString())
                .Returns("something");
        }


        [TearDown]
        public void TearDown()
        {
            mockAddress.Reset();
        }



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
            Assert.That(senderType, Is.Not.Null);
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


        [Test]
        public void ToString_WithAddress_CallsAddressToString()
        {
            // Arrange
            var routing = SenderRouting.For<ISender>(address);

            // Act
            var str = routing.ToString();

            // Assert
            mockAddress.Verify(m => m.ToString(), Times.Once);
        }
    }
}
