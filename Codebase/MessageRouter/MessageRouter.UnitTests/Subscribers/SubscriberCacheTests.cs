using System.Collections.Generic;

using MessageRouter.Addresses;
using MessageRouter.Packages;
using MessageRouter.Monitors;
using MessageRouter.Routing;
using MessageRouter.Subscribers;
using MessageRouter.Topics;

using Moq;

using NUnit.Framework;
using MessageRouter.Diagnostics;

namespace MessageRouter.UnitTests.Subscribers
{
    [TestFixture]
    public class SubscriberCacheTests
    {
        public class Topic { }
        public class OtherTopic { }
        
        private readonly Mock<ITopicRouter> mockTopicRouter = new Mock<ITopicRouter>();
        private ITopicRouter topicRouter;
        
        private readonly Mock<IMonitorCache> mockMonitorCache = new Mock<IMonitorCache>();
        private IMonitorCache monitorCache;

        private readonly Mock<IPackageFactory> mockPackageFactory = new Mock<IPackageFactory>();
        private IPackageFactory packageFactory;

        private readonly Mock<ITopicDispatcher> mockDispatcher = new Mock<ITopicDispatcher>();
        private ITopicDispatcher dispatcher;

        private readonly Mock<ISubscriptionsCache> mockSubscriptionsCache = new Mock<ISubscriptionsCache>();
        private ISubscriptionsCache subscriptionsCache;
        
        private readonly Mock<ISubscriberFactory<ISubscriber>> mockSubscriberFactory = new Mock<ISubscriberFactory<ISubscriber>>();
        private ISubscriberFactory<ISubscriber> subscriberFactory;

        private readonly Mock<ISubscriber> mockSubscriber = new Mock<ISubscriber>();
        private ISubscriber subscriber;
        
        private readonly Mock<ISubscriberMonitor<ISubscriber>> mockMonitor = new Mock<ISubscriberMonitor<ISubscriber>>();
        private ISubscriberMonitor<ISubscriber> monitor;

        private IAddress address;
        private SubscriberRouting routing;
        private TopicEventHandler handler = (sub, topic) => { };


        [SetUp]
        public void Setup()
        {
            topicRouter = mockTopicRouter.Object;
            monitorCache = mockMonitorCache.Object;
            packageFactory = mockPackageFactory.Object;
            dispatcher = mockDispatcher.Object;
            subscriptionsCache = mockSubscriptionsCache.Object;
            subscriberFactory = mockSubscriberFactory.Object;
            subscriber = mockSubscriber.Object;
            monitor = mockMonitor.Object;
            address = TcpAddress.Localhost(5555);
            routing = SubscriberRouting.For<ISubscriber>(address);

            mockTopicRouter
                .Setup(m => m.RoutingFor<Topic>(out routing))
                .Returns(true);

            mockSubscriberFactory
                .Setup(m => m.CreateSubscriber(It.IsAny<IAddress>(), It.IsAny<TopicEventHandler>()))
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
            mockMonitor.Reset();
            mockDispatcher.Reset();
            mockSubscriber.Reset();
            mockTopicRouter.Reset();
            mockMonitorCache.Reset();
            mockPackageFactory.Reset();
            mockSubscriberFactory.Reset();
            mockSubscriptionsCache.Reset();
        }


        #region Constructor
        [Test]
        public void SubscriberCache_WithNullTopicRouter_ThrowsArgumentNullException()
        {
            // Act
            TestDelegate construct = () => new SubscriberCache(null, monitorCache, packageFactory, dispatcher, subscriptionsCache);

            // Assert
            Assert.That(construct, Throws.ArgumentNullException);
        }

        [Test]
        public void SubscriberCache_WithNullMonitorCache_ThrowsArgumentNullException()
        {
            // Act
            TestDelegate construct = () => new SubscriberCache(topicRouter, null, packageFactory, dispatcher, subscriptionsCache);

            // Assert
            Assert.That(construct, Throws.ArgumentNullException);
        }


        [Test]
        public void SubscriberCache_WithNullPackageFactory_ThrowsArgumentNullException()
        {
            // Act
            TestDelegate construct = () => new SubscriberCache(topicRouter, monitorCache, null, dispatcher, subscriptionsCache);

            // Assert
            Assert.That(construct, Throws.ArgumentNullException);
        }


        [Test]
        public void SubscriberCache_WithNullSubscriptionEventDispatcher_ThrowsArgumentNullException()
        {
            // Act
            TestDelegate construct = () => new SubscriberCache(topicRouter, monitorCache, packageFactory, null, subscriptionsCache);

            // Assert
            Assert.That(construct, Throws.ArgumentNullException);
        }


        [Test]
        public void SubscriberCache_WithNullSubscriptionsCache_ThrowsArgumentNullException()
        {
            // Act
            TestDelegate construct = () => new SubscriberCache(topicRouter, monitorCache, packageFactory, dispatcher, null);

            // Assert
            Assert.That(construct, Throws.ArgumentNullException);
        }
        #endregion


        #region AddFactory
        [Test]
        public void AddFactory_WithNullFactory_ThrowsArgumentNullException()
        {
            // Arrange
            var cache = new SubscriberCache(topicRouter, monitorCache, packageFactory, dispatcher, subscriptionsCache);

            // Act
            TestDelegate addFactory = () => cache.AddFactory(null);

            // Assert
            Assert.That(addFactory, Throws.ArgumentNullException);
        }


        [Test]
        public void AddFactory_WithFactory_AddsToFactoriesCollection()
        {
            // Arrange
            var cache = new SubscriberCache(topicRouter, monitorCache, packageFactory, dispatcher, subscriptionsCache);

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
            var cache = new SubscriberCache(topicRouter, monitorCache, packageFactory, dispatcher, subscriptionsCache);

            // Act
            cache.AddFactory(subscriberFactory);

            // Assert
            mockMonitorCache.Verify(m => m.AddMonitor(It.IsIn(monitor)), Times.Once);
        }


        [Test]
        public void AddFactory_WithFactoryAlreadyAdded_DoesNothing()
        {
            // Arrange
            var cache = new SubscriberCache(topicRouter, monitorCache, packageFactory, dispatcher, subscriptionsCache);
            cache.AddFactory(subscriberFactory);

            // Act
            cache.AddFactory(subscriberFactory);

            // Assert
            CollectionAssert.Contains(cache.SubscriberFactories, subscriberFactory);
            Assert.That(cache.SubscriberFactories.Count, Is.EqualTo(1));
            mockMonitorCache.Verify(m => m.AddMonitor(It.IsIn(monitor)), Times.Once);
        }
        #endregion


        #region SubscriberFor
        [Test]
        public void SubscriberFor_WithNoRouting_ThrowsKeyNotFoundException()
        {
            // Arrange
            var cache = new SubscriberCache(topicRouter, monitorCache, packageFactory, dispatcher, subscriptionsCache);

            // Act
            TestDelegate subscriberFor = () => cache.SubscriberFor<OtherTopic>();

            // Assert
            Assert.That(subscriberFor, Throws.TypeOf<KeyNotFoundException>());
        }


        [Test]
        public void SubscriberFor_WithNoFactory_ThrowsMissingFactoryException()
        {
            // Arrange
            var cache = new SubscriberCache(topicRouter, monitorCache, packageFactory, dispatcher, subscriptionsCache);

            // Act
            TestDelegate subscriberFor = () => cache.SubscriberFor<Topic>();

            // Assert
            Assert.That(subscriberFor, Throws.TypeOf<MissingFactoryException>());
        }


        [Test]
        public void SubscriberFor_WithRoutingAndFactory_CallsFactoryCreate()
        {
            // Arrange
            var cache = new SubscriberCache(topicRouter, monitorCache, packageFactory, dispatcher, subscriptionsCache);
            cache.AddFactory(subscriberFactory);

            // Act
            var subscriber = cache.SubscriberFor<Topic>();

            // Assert
            mockSubscriberFactory.Verify(m => m.CreateSubscriber(It.IsIn(address), It.IsIn(cache.Handler)), Times.Once);
        }


        [Test]
        public void SubscriberFor_WhenCalledTwice_CallsFactoryCreateOnce()
        {
            // Arrange
            var cache = new SubscriberCache(topicRouter, monitorCache, packageFactory, dispatcher, subscriptionsCache);
            cache.AddFactory(subscriberFactory);
            cache.SubscriberFor<Topic>();

            // Act
            cache.SubscriberFor<Topic>();

            // Assert
            mockSubscriberFactory.Verify(m => m.CreateSubscriber(It.IsIn(address), It.IsIn(cache.Handler)), Times.Once);
        }
        #endregion


        #region Subscribe
        [Test]
        public void Subscribe_WithSubscriber_CallsSubscribe()
        {
            // Arrange
            var cache = new SubscriberCache(topicRouter, monitorCache, packageFactory, dispatcher, subscriptionsCache);
            cache.AddFactory(subscriberFactory);

            // Act
            cache.Subscribe<Topic>();

            // Assert
            mockSubscriber.Verify(m => m.Subscribe<Topic>(), Times.Once);
        }


        [Test]
        public void Subscribe_WithSubscriber_AddsSubscriptionToSubscriptionsCache()
        {
            // Arrange
            var cache = new SubscriberCache(topicRouter, monitorCache, packageFactory, dispatcher, subscriptionsCache);
            cache.AddFactory(subscriberFactory);

            // Act
            cache.Subscribe<Topic>();

            // Assert
            mockSubscriptionsCache.Verify(m => m.Add<Topic>(It.IsIn(subscriber)), Times.Once);
        }


        [Test]
        public void Subscribe_WithNoFactory_ThrowsMissingFactoryExcpetion()
        {
            // Arrange
            var cache = new SubscriberCache(topicRouter, monitorCache, packageFactory, dispatcher, subscriptionsCache);

            // Act
            TestDelegate subscriberFor = () => cache.Subscribe<Topic>();

            // Assert
            Assert.That(subscriberFor, Throws.TypeOf<MissingFactoryException>());
        }


        [Test]
        public void Subscribe_WithNoRouting_ThrowsKeyNotFoundException()
        {
            // Arrange
            var cache = new SubscriberCache(topicRouter, monitorCache, packageFactory, dispatcher, subscriptionsCache);

            // Act
            TestDelegate subscribe = () => cache.Subscribe<OtherTopic>();

            // Assert
            Assert.That(subscribe, Throws.TypeOf<KeyNotFoundException>());
        }
        #endregion


        #region Unsubscribe
        [Test]
        public void Unsubscribe_WithFactory_ForwardsToSubscriptionsCache()
        {
            // Arrange
            var cache = new SubscriberCache(topicRouter, monitorCache, packageFactory, dispatcher, subscriptionsCache);
            cache.AddFactory(subscriberFactory);

            // Act
            cache.Unsubscribe<Topic>();

            // Assert
            mockSubscriptionsCache.Verify(m => m.Remove<Topic>(It.IsIn(subscriber)), Times.Once);
        }
        #endregion
    }
}
