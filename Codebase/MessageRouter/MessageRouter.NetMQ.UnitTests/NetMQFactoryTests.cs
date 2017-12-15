using MessageRouter.Addresses;
using MessageRouter.NetMQ.Receivers;
using MessageRouter.NetMQ.Senders;
using MessageRouter.Receivers;
using MessageRouter.Senders;
using MessageRouter.Serialization;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessageRouter.NetMQ.UnitTests
{
    [TestFixture]
    public class NetMQFactoryTests
    {
        private readonly Mock<INetMQMonitor> mockMonitor = new Mock<INetMQMonitor>();
        private INetMQMonitor monitor;
        
        private readonly Mock<ISerializer<byte[]>> mockSerializer = new Mock<ISerializer<byte[]>>();
        private ISerializer<byte[]> serializer;


        [SetUp]
        public void Setup()
        {
            monitor = mockMonitor.Object;
            serializer = mockSerializer.Object;
        }


        [TearDown]
        public void TearDown()
        {
            mockMonitor.Reset();
            mockSerializer.Reset();
        }


        #region Constructor
        [Test]
        public void NetMQFactory_WithNullMonitor_ThrowsArgumentNullException()
        {
            // Act
            TestDelegate construct = () => new NetMQFactory(null, serializer);

            // Assert
            Assert.That(construct, Throws.ArgumentNullException);
        }


        [Test]
        public void NetMQFactory_WithNullSerializer_ThrowsArgumentNullException()
        {
            // Act
            TestDelegate construct = () => new NetMQFactory(monitor, null);

            // Assert
            Assert.That(construct, Throws.ArgumentNullException);
        }
        #endregion


        [Test]
        public void CreateNewSender_ReturnsSender()
        {
            // Arrange
            var factory = new NetMQFactory(monitor, serializer);

            // Act
            var sender = factory.CreateSender(TcpAddress.Localhost(5555));

            // Assert
            Assert.That(sender, Is.Not.Null);
        }


        [Test]
        public void CreateNewReceiver_ReturnsReceiver()
        {
            // Arrange
            var factory = new NetMQFactory(monitor, serializer);

            // Act
            var receiver = factory.CreateReceiver(TcpAddress.Wildcard(5555));

            // Assert
            Assert.That(receiver, Is.Not.Null);
        }


        [Test]
        public void CreateNewPublisher_ThrowsNotSupportedException()
        {
            // Arrange
            var factory = new NetMQFactory(monitor, serializer);

            // Act
            TestDelegate createPublisher = () => factory.CreatePublisher(TcpAddress.Wildcard(5555));

            // Assert
            Assert.That(createPublisher, Throws.TypeOf<NotSupportedException>());
        }


        [Test]
        public void CreateNewSubscriber_ThrowsNotSupportedException()
        {
            // Arrange
            var factory = new NetMQFactory(monitor, serializer);

            // Act
            TestDelegate createSubscriber = () => factory.CreateSubscriber(TcpAddress.Localhost(5555));

            // Assert
            Assert.That(createSubscriber, Throws.TypeOf<NotSupportedException>());
        }
    }
}
