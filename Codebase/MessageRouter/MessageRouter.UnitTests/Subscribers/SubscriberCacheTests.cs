using System.Collections.Generic;

using MessageRouter.Addresses;
using MessageRouter.Messages;
using MessageRouter.Monitors;
using MessageRouter.Subscribers;
using MessageRouter.Topics;

using Moq;

using NUnit.Framework;

namespace MessageRouter.UnitTests.Subscribers
{
    [TestFixture]
    public class SubscriberCacheTests
    {
        private readonly Mock<IMonitorCache> mockMonitorCache = new Mock<IMonitorCache>();
        private IMonitorCache monitorCache;

        private readonly Mock<IMessageFactory> mockMessageFactory = new Mock<IMessageFactory>();
        private IMessageFactory messageFactory;

        private readonly Mock<ITopicDispatcher> mockDispatcher = new Mock<ITopicDispatcher>();
        private ITopicDispatcher dispatcher;

        private readonly Mock<ISubscriberFactory<ISubscriber>> mockSubscriberFactory = new Mock<ISubscriberFactory<ISubscriber>>();
        private ISubscriberFactory<ISubscriber> subscriberFactory;

        private readonly Mock<ISubscriber> mockSubscriber = new Mock<ISubscriber>();
        private ISubscriber subscriber;

        private readonly Mock<IAddress> mockAddress = new Mock<IAddress>();
        private IAddress address;

        private readonly Mock<ISubscriberMonitor<ISubscriber>> mockMonitor = new Mock<ISubscriberMonitor<ISubscriber>>();
        private ISubscriberMonitor<ISubscriber> monitor;


        [SetUp]
        public void Setup()
        {
            monitorCache = mockMonitorCache.Object;
            messageFactory = mockMessageFactory.Object;
            dispatcher = mockDispatcher.Object;
            subscriberFactory = mockSubscriberFactory.Object;
            subscriber = mockSubscriber.Object;
            address = mockAddress.Object;
            monitor = mockMonitor.Object;

            mockSubscriberFactory
                .Setup(m => m.CreateSubscriber(It.IsAny<IAddress>()))
                .Returns(subscriber);

            mockSubscriberFactory
                .SetupGet(m => m.SubscriberType)
                .Returns(typeof(ISubscriber));

            mockSubscriberFactory
                .SetupGet(m => m.SubscriberMonitor)
                .Returns(monitor);
        }


        [TearDown]
        public void TearDown()
        {
            mockMonitorCache.Reset();
            mockMessageFactory.Reset();
            mockDispatcher.Reset();
            mockSubscriberFactory.Reset();
            mockSubscriber.Reset();
            mockAddress.Reset();
            mockMonitor.Reset();
        }


        #region Constructor
        [Test]
        public void SubscriberCache_WithNullMonitorCache_ThrowsArgumentNullException()
        {
            // Act
            TestDelegate construct = () => new SubscriberCache(null, messageFactory, dispatcher);

            // Assert
            Assert.That(construct, Throws.ArgumentNullException);
        }


        [Test]
        public void SubscriberCache_WithNullMessageFactory_ThrowsArgumentNullException()
        {
            // Act
            TestDelegate construct = () => new SubscriberCache(monitorCache, null, dispatcher);

            // Assert
            Assert.That(construct, Throws.ArgumentNullException);
        }


        [Test]
        public void SubscriberCache_WithNullSubscriptionEventDispatcher_ThrowsArgumentNullException()
        {
            // Act
            TestDelegate construct = () => new SubscriberCache(monitorCache, messageFactory, null);

            // Assert
            Assert.That(construct, Throws.ArgumentNullException);
        }
        #endregion


        #region AddFactory
        [Test]
        public void AddFactory_WithNullFactory_ThrowsArgumentNullException()
        {
            // Arrange
            var cache = new SubscriberCache(monitorCache, messageFactory, dispatcher);

            // Act
            TestDelegate addFactory = () => cache.AddFactory(null);

            // Assert
            Assert.That(addFactory, Throws.ArgumentNullException);
        }


        [Test]
        public void AddFactory_WithFactory_AddsToFactoriesCollection()
        {
            // Arrange
            var cache = new SubscriberCache(monitorCache, messageFactory, dispatcher);

            // Act
            cache.AddFactory(subscriberFactory);

            // Assert
            CollectionAssert.Contains(cache.SubscriberFactories, subscriberFactory);
            Assert.That(cache.SubscriberFactories.Count, Is.EqualTo(1));
        }


        [Test]
        public void AddFactory_WithFactory_AddsMonitorToMonitorCache()
        {
            // Arrange
            var cache = new SubscriberCache(monitorCache, messageFactory, dispatcher);

            // Act
            cache.AddFactory(subscriberFactory);

            // Assert
            mockMonitorCache.Verify(m => m.AddMonitor(It.IsIn(monitor)), Times.Once);
        }


        [Test]
        public void AddFactory_WithFactoryAlreadyAdded_DoesNothing()
        {
            // Arrange
            var cache = new SubscriberCache(monitorCache, messageFactory, dispatcher);
            cache.AddFactory(subscriberFactory);

            // Act
            cache.AddFactory(subscriberFactory);

            // Assert
            CollectionAssert.Contains(cache.SubscriberFactories, subscriberFactory);
            Assert.That(cache.SubscriberFactories.Count, Is.EqualTo(1));
            mockMonitorCache.Verify(m => m.AddMonitor(It.IsIn(monitor)), Times.Once);
        }
        #endregion


        #region AddSubscriber
        [Test]
        public void AddSubscriber_WithNullAddress_ThrowsArgumentNullException()
        {
            // Arrange
            var cache = new SubscriberCache(monitorCache, messageFactory, dispatcher);

            // Act
            TestDelegate addSubscriber = () => cache.AddSubscriber<ISubscriber>(null);

            // Assert
            Assert.That(addSubscriber, Throws.ArgumentNullException);
        }


        [Test]
        public void AddSubscriber_WithNoMatchingFactory_ThrowsKeyNotFoundException()
        {
            // Arrange
            var cache = new SubscriberCache(monitorCache, messageFactory, dispatcher);

            // Act
            TestDelegate addSubscriber = () => cache.AddSubscriber<ISubscriber>(address);

            // Assert
            Assert.That(addSubscriber, Throws.TypeOf<KeyNotFoundException>());
        }


        [Test]
        public void AddSubscriber_WithFactory_CallsFactoryCreateSubscriber()
        {
            // Arrange
            var cache = new SubscriberCache(monitorCache, messageFactory, dispatcher);
            cache.AddFactory(subscriberFactory);

            // Act
            cache.AddSubscriber<ISubscriber>(address);

            // Assert
            mockSubscriberFactory.Verify(m => m.CreateSubscriber(It.IsIn(address)), Times.Once);
        }


        [Test]
        public void AddSubscriber_WithExistingSubscriberForAddress_ThrowsInvalidOperationException()
        {
            // Arrange
            var address = TcpAddress.Wildcard(5555);
            var cache = new SubscriberCache(monitorCache, messageFactory, dispatcher);
            cache.AddFactory(subscriberFactory);
            cache.AddSubscriber<ISubscriber>(address);

            // Act
            TestDelegate addSubscriber = () => cache.AddSubscriber<ISubscriber>(address);

            // Assert
            Assert.That(addSubscriber, Throws.InvalidOperationException);
        }
        #endregion
    }
}
