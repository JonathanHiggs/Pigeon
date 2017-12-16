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
        [Test]
        public void SenderFactory_ReturnsFactory()
        {
            // Arrange
            var config = new NetMQConfig();

            // Act
            var senderFactory = config.SenderFactory;

            // Assert
            Assert.That(senderFactory, Is.Not.Null);
        }


        [Test]
        public void ReceiverFactory_ReturnsFactory()
        {
            // Arrange
            var config = new NetMQConfig();

            // Act
            var receiverFactory = config.ReceiverFactory;

            // Assert
            Assert.That(receiverFactory, Is.Not.Null);
        }


        [Test]
        public void PublisherFactory_ReturnsFactory()
        {
            // Arrange
            var config = new NetMQConfig();

            // Act
            var publisherFactory = config.PublisherFactory;

            // Assert
            Assert.That(publisherFactory, Is.Not.Null);
        }


        [Test]
        public void SubscriberFactory_ReturnsRactory()
        {
            // Arrange
            var config = new NetMQConfig();

            // Act
            var subscriberFactory = config.SubscriberFactory;

            // Assert
            Assert.That(subscriberFactory, Is.Not.Null);
        }
    }
}
