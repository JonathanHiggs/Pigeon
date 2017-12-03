using MessageRouter.Addresses;
using MessageRouter.NetMQ.Senders;
using MessageRouter.Serialization;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessageRouter.NetMQ.UnitTests.Senders
{
    [TestFixture]
    public class NetMQSenderFactoryTests
    {
        private readonly Mock<INetMQSenderMonitor> mockSenderMonitor = new Mock<INetMQSenderMonitor>();
        private INetMQSenderMonitor senderMonitor;
        
        private readonly Mock<ISerializer<byte[]>> mockSerializer = new Mock<ISerializer<byte[]>>();
        private ISerializer<byte[]> serializer;


        [SetUp]
        public void Setup()
        {
            senderMonitor = mockSenderMonitor.Object;
            serializer = mockSerializer.Object;
        }


        [TearDown]
        public void TearDown()
        {
            mockSenderMonitor.Reset();
            mockSerializer.Reset();
        }


        [Test]
        public void NetMQSenderFactory_WithMissingMonitor_ThrowsArgumentNullException()
        {
            // Act
            TestDelegate construct = () => new NetMQSenderFactory(null, serializer);

            // Assert
            Assert.That(construct, Throws.ArgumentNullException);
        }


        [Test]
        public void NetMQSenderFactory_WithMissingSerializer_ThrowsArgumentNullException()
        {
            // Act
            TestDelegate construct = () => new NetMQSenderFactory(senderMonitor, null);

            // Assert
            Assert.That(construct, Throws.ArgumentNullException);
        }


        [Test]
        public void CreateSender_WithAddress_AddsToSenderMonitor()
        {
            // Arrange
            var factory = new NetMQSenderFactory(senderMonitor, serializer);
            var address = TcpAddress.Wildcard(5555);

            // Act
            var sender = factory.CreateSender(address);

            // Assert
            mockSenderMonitor.Verify(m => m.AddSender(It.IsAny<INetMQSender>()), Times.Once);
        }


        [Test]
        public void CreateSender_WithAddress_ReturnsSenderWithAddressAdded()
        {
            // Arrange
            var factory = new NetMQSenderFactory(senderMonitor, serializer);
            var address = TcpAddress.Wildcard(5555);

            // Act
            var sender = factory.CreateSender(address);

            // Assert
            CollectionAssert.Contains(sender.Addresses, address);
        }


        [Test]
        public void CreateSender_WithNullAddress_ThrowsArgumentNullException()
        {
            // Arrange
            var factory = new NetMQSenderFactory(senderMonitor, serializer);

            // Act
            TestDelegate createSender = () => factory.CreateSender(null);

            // Assert
            Assert.That(createSender, Throws.ArgumentNullException);
        }
    }
}
