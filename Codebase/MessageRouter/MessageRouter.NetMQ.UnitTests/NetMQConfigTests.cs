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
    public class NetMQConfigTests
    {
        private readonly Mock<INetMQFactory> mockFactory = new Mock<INetMQFactory>();
        private INetMQFactory factory;


        [SetUp]
        public void Setup()
        {
            factory = mockFactory.Object;
        }


        [TearDown]
        public void TearDown()
        {
            mockFactory.Reset();
        }


        [Test]
        public void NetMQConfig_WithNullFactory_ThrowsArgumentNullException()
        {
            // Act
            TestDelegate construct = () => new NetMQConfig(null);

            // Assert
            Assert.That(construct, Throws.ArgumentNullException);
        }


        [Test]
        public void SenderFactory_ReturnsFactory()
        {
            // Arrange
            var config = new NetMQConfig(factory);

            // Act
            var senderFactory = config.SenderFactory;

            // Assert
            Assert.That(senderFactory, Is.SameAs(factory));
        }


        [Test]
        public void ReceiverFactory_ReturnsFactory()
        {
            // Arrange
            var config = new NetMQConfig(factory);

            // Act
            var receiverFactory = config.ReceiverFactory;

            // Assert
            Assert.That(receiverFactory, Is.SameAs(factory));
        }


        [Test]
        public void PublisherFactory_ReturnsFactory()
        {
            // Arrange
            var config = new NetMQConfig(factory);

            // Act
            var publisherFactory = config.PublisherFactory;

            // Assert
            Assert.That(publisherFactory, Is.SameAs(factory));
        }


        [Test]
        public void SubscriberFactory_ReturnsRactory()
        {
            // Arrange
            var config = new NetMQConfig(factory);

            // Act
            var subscriberFactory = config.SubscriberFactory;

            // Assert
            Assert.That(subscriberFactory, Is.SameAs(factory));
        }
    }
}
