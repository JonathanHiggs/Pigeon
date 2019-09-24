using Moq;

using NUnit.Framework;

using Pigeon.Publishers;
using Pigeon.Receivers;
using Pigeon.Routing;

namespace Pigeon.NetMQ.UnitTests
{
    [TestFixture]
    public class NetMQConfigTests
    {
        private readonly Mock<INetMQFactory> mockFactory = new Mock<INetMQFactory>();
        private INetMQFactory factory;

        private readonly Mock<IRequestRouter> mockRequestRouter = new Mock<IRequestRouter>();
        private IRequestRouter requestRouter;

        private readonly Mock<IReceiverCache> mockReceiverCache = new Mock<IReceiverCache>();
        private IReceiverCache receiverCache;

        private readonly Mock<IPublisherCache> mockPublisherCache = new Mock<IPublisherCache>();
        private IPublisherCache publisherCache;

        private readonly Mock<ITopicRouter> mockTopicRouter = new Mock<ITopicRouter>();
        private ITopicRouter topicRouter;


        [SetUp]
        public void Setup()
        {
            factory = mockFactory.Object;
            requestRouter = mockRequestRouter.Object;
            topicRouter = mockTopicRouter.Object;
            receiverCache = mockReceiverCache.Object;
            publisherCache = mockPublisherCache.Object;
        }


        [TearDown]
        public void TearDown()
        {
            mockFactory.Reset();
            mockRequestRouter.Reset();
            mockTopicRouter.Reset();
            mockReceiverCache.Reset();
            mockPublisherCache.Reset();
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
            TestDelegate construct = () => NetMQTransport.Create(null, requestRouter, receiverCache, topicRouter, publisherCache);

            // Assert
            Assert.That(construct, Throws.ArgumentNullException);
        }


        [Test]
        public void Create_WithNullRequestRouter_ThrowsArgumentNullException()
        {
            // Act
            TestDelegate construct = () => NetMQTransport.Create(factory, null, receiverCache, topicRouter, publisherCache);

            // Assert
            Assert.That(construct, Throws.ArgumentNullException);
        }


        [Test]
        public void Create_WithNullReceiverCache_ThrowsArgumentNullException()
        {
            // Act
            TestDelegate construct = () => NetMQTransport.Create(factory, requestRouter, null, topicRouter, publisherCache);

            // Assert
            Assert.That(construct, Throws.ArgumentNullException);
        }


        [Test]
        public void Create_WithNullTopicRouter_ThrowsArgumentNullException()
        {
            // Act
            TestDelegate construct = () => NetMQTransport.Create(factory, requestRouter, receiverCache, null, publisherCache);

            // Assert
            Assert.That(construct, Throws.ArgumentNullException);
        }


        [Test]
        public void Create_WithNullPublisherCache_ThrowsArgumentNullException()
        {
            // Act
            TestDelegate construct = () => NetMQTransport.Create(factory, requestRouter, receiverCache, topicRouter, null);

            // Assert
            Assert.That(construct, Throws.ArgumentNullException);
        }
        #endregion


        [Test]
        public void SenderFactory_ReturnsFactory()
        {
            // Arrange
            var config = NetMQTransport.Create(factory, requestRouter, receiverCache, topicRouter, publisherCache);

            // Act
            var senderFactory = config.SenderFactory;

            // Assert
            Assert.That(senderFactory, Is.SameAs(factory));
        }


        [Test]
        public void ReceiverFactory_ReturnsFactory()
        {
            // Arrange
            var config = NetMQTransport.Create(factory, requestRouter, receiverCache, topicRouter, publisherCache);

            // Act
            var receiverFactory = config.ReceiverFactory;

            // Assert
            Assert.That(receiverFactory, Is.SameAs(factory));
        }


        [Test]
        public void PublisherFactory_ReturnsFactory()
        {
            // Arrange
            var config = NetMQTransport.Create(factory, requestRouter, receiverCache, topicRouter, publisherCache);

            // Act
            var publisherFactory = config.PublisherFactory;

            // Assert
            Assert.That(publisherFactory, Is.SameAs(factory));
        }


        [Test]
        public void SubscriberFactory_ReturnsRactory()
        {
            // Arrange
            var config = NetMQTransport.Create(factory, requestRouter, receiverCache, topicRouter, publisherCache);

            // Act
            var subscriberFactory = config.SubscriberFactory;

            // Assert
            Assert.That(subscriberFactory, Is.SameAs(factory));
        }
    }
}
