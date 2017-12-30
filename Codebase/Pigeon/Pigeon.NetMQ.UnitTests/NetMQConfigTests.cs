using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pigeon.NetMQ.UnitTests
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
        public void NetMQConfig_WithNullContainer_ThrowsArgumentNullException()
        {
            // Act
            TestDelegate construct = () => new NetMQConfig(null);

            // Assert
            Assert.That(construct, Throws.ArgumentNullException);
        }


        [Test]
        public void Create_WithNullNetMQFactory_ThrowsArgumentNullException()
        {
            // Act
            TestDelegate construct = () => NetMQConfig.Create(null);

            // Assert
            Assert.That(construct, Throws.ArgumentNullException);
        }


        [Test]
        public void SenderFactory_ReturnsFactory()
        {
            // Arrange
            var config = NetMQConfig.Create(factory);

            // Act
            var senderFactory = config.SenderFactory;

            // Assert
            Assert.That(senderFactory, Is.SameAs(factory));
        }


        [Test]
        public void ReceiverFactory_ReturnsFactory()
        {
            // Arrange
            var config = NetMQConfig.Create(factory);

            // Act
            var receiverFactory = config.ReceiverFactory;

            // Assert
            Assert.That(receiverFactory, Is.SameAs(factory));
        }


        [Test]
        public void PublisherFactory_ReturnsFactory()
        {
            // Arrange
            var config = NetMQConfig.Create(factory);

            // Act
            var publisherFactory = config.PublisherFactory;

            // Assert
            Assert.That(publisherFactory, Is.SameAs(factory));
        }


        [Test]
        public void SubscriberFactory_ReturnsRactory()
        {
            // Arrange
            var config = NetMQConfig.Create(factory);

            // Act
            var subscriberFactory = config.SubscriberFactory;

            // Assert
            Assert.That(subscriberFactory, Is.SameAs(factory));
        }
    }
}
