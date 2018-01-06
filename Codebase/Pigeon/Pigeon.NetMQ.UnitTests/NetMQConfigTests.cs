using Moq;
using NUnit.Framework;
using Pigeon.Routing;
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

        private readonly Mock<IRequestRouter> mockRequestRouter = new Mock<IRequestRouter>();
        private IRequestRouter requestRouter;

        private readonly Mock<ITopicRouter> mockTopicRouter = new Mock<ITopicRouter>();
        private ITopicRouter topicRouter;
        

        [SetUp]
        public void Setup()
        {
            factory = mockFactory.Object;
            requestRouter = mockRequestRouter.Object;
            topicRouter = mockTopicRouter.Object;
        }


        [TearDown]
        public void TearDown()
        {
            mockFactory.Reset();
            mockRequestRouter.Reset();
            mockTopicRouter.Reset();
        }


        [Test]
        public void NetMQConfig_WithNullContainer_ThrowsArgumentNullException()
        {
            // Act
            TestDelegate construct = () => new NetMQTransport(null);

            // Assert
            Assert.That(construct, Throws.ArgumentNullException);
        }


        #region Create
        [Test]
        public void Create_WithNullNetMQFactory_ThrowsArgumentNullException()
        {
            // Act
            TestDelegate construct = () => NetMQTransport.Create(null, requestRouter, topicRouter);

            // Assert
            Assert.That(construct, Throws.ArgumentNullException);
        }


        [Test]
        public void Create_WithNullRequestRouter_ThrowsArgumentNullException()
        {
            // Act
            TestDelegate construct = () => NetMQTransport.Create(factory, null, topicRouter);

            // Assert
            Assert.That(construct, Throws.ArgumentNullException);
        }


        [Test]
        public void Create_WithNullTopicRouter_ThrowsArgumentNullException()
        {
            // Act
            TestDelegate construct = () => NetMQTransport.Create(factory, requestRouter, null);

            // Assert
            Assert.That(construct, Throws.ArgumentNullException);
        }
        #endregion


        [Test]
        public void SenderFactory_ReturnsFactory()
        {
            // Arrange
            var config = NetMQTransport.Create(factory, requestRouter, topicRouter);

            // Act
            var senderFactory = config.SenderFactory;

            // Assert
            Assert.That(senderFactory, Is.SameAs(factory));
        }


        [Test]
        public void ReceiverFactory_ReturnsFactory()
        {
            // Arrange
            var config = NetMQTransport.Create(factory, requestRouter, topicRouter);

            // Act
            var receiverFactory = config.ReceiverFactory;

            // Assert
            Assert.That(receiverFactory, Is.SameAs(factory));
        }


        [Test]
        public void PublisherFactory_ReturnsFactory()
        {
            // Arrange
            var config = NetMQTransport.Create(factory, requestRouter, topicRouter);

            // Act
            var publisherFactory = config.PublisherFactory;

            // Assert
            Assert.That(publisherFactory, Is.SameAs(factory));
        }


        [Test]
        public void SubscriberFactory_ReturnsRactory()
        {
            // Arrange
            var config = NetMQTransport.Create(factory, requestRouter, topicRouter);

            // Act
            var subscriberFactory = config.SubscriberFactory;

            // Assert
            Assert.That(subscriberFactory, Is.SameAs(factory));
        }
    }
}
