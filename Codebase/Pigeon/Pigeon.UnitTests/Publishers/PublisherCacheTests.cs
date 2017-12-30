using Pigeon.Addresses;
using Pigeon.Packages;
using Pigeon.Monitors;
using Pigeon.Publishers;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pigeon.Diagnostics;

namespace Pigeon.UnitTests.Publishers
{
    [TestFixture]
    public class PublisherCacheTests
    {
        private readonly Mock<IMonitorCache> mockMonitorCache = new Mock<IMonitorCache>();
        private IMonitorCache monitorCache;

        private readonly Mock<IPublisherFactory<IPublisher>> mockFactory = new Mock<IPublisherFactory<IPublisher>>();
        private IPublisherFactory<IPublisher> factory;

        private readonly Mock<IPublisher> mockPublisher = new Mock<IPublisher>();
        private IPublisher publisher;

        private readonly Mock<IAddress> mockAddress = new Mock<IAddress>();
        private IAddress address;

        private readonly Mock<IPublisherMonitor<IPublisher>> mockMonitor = new Mock<IPublisherMonitor<IPublisher>>();
        private IPublisherMonitor<IPublisher> monitor;

        private readonly Package package = new DataPackage<string>(new GuidPackageId(), "something");

        
        [SetUp]
        public void Setup()
        {
            monitorCache = mockMonitorCache.Object;
            factory = mockFactory.Object;
            publisher = mockPublisher.Object;
            address = mockAddress.Object;
            monitor = mockMonitor.Object;

            mockFactory
                .Setup(m => m.CreatePublisher(It.IsIn(address)))
                .Returns(publisher);

            mockFactory
                .SetupGet(m => m.PublisherMonitor)
                .Returns(monitor);

            mockFactory
                .SetupGet(m => m.PublisherType)
                .Returns(typeof(IPublisher));

            mockAddress
                .Setup(m => m.ToString())
                .Returns("address");

            mockAddress
                .Setup(m => m.GetHashCode())
                .Returns(1);
        }


        [TearDown]
        public void TearDown()
        {
            mockMonitorCache.Reset();
            mockFactory.Reset();
            mockPublisher.Reset();
            mockAddress.Reset();
        }


        #region Constructor
        [Test]
        public void PublisherCache_WithNullMonitorCache_ThrowsArgumentNullException()
        {
            // Act
            TestDelegate construct = () => new PublisherCache(null);

            // Assert
            Assert.That(construct, Throws.ArgumentNullException);
        }
        #endregion


        #region AddFactory
        [Test]
        public void AddFactory_WithNullFactory_ThrowsArgumentNullException()
        {
            // Arrange
            var cache = new PublisherCache(monitorCache);

            // Act
            TestDelegate addFactory = () => cache.AddFactory(null);

            // Assert
            Assert.That(addFactory, Throws.ArgumentNullException);
        }


        [Test]
        public void AddFactory_WithExistingFactoryForPublisherType_DoesNothing()
        {
            // Arrange
            var cache = new PublisherCache(monitorCache);
            cache.AddFactory(factory);

            // Act
            cache.AddFactory(factory);

            // Assert
            mockMonitorCache.Verify(m => m.AddMonitor(It.IsIn(monitor)), Times.Once);
            Assert.That(cache.PublisherFactories.Count, Is.EqualTo(1));
        }


        [Test]
        public void AddFactory_WithFactory_AddsToFactoryList()
        {
            // Arrange
            var cache = new PublisherCache(monitorCache);

            // Act
            cache.AddFactory(factory);

            // Assert
            Assert.That(cache.PublisherFactories.Count, Is.EqualTo(1));
        }


        [Test]
        public void AddFactory_WithFactory_AddsMonitorToMonitorCache()
        {
            // Arrange
            var cache = new PublisherCache(monitorCache);

            // Act
            cache.AddFactory(factory);

            // Assert
            mockMonitorCache.Verify(m => m.AddMonitor(It.IsIn(monitor)), Times.Once);
        }
        #endregion


        #region AddPublisher
        [Test]
        public void AddPublisher_WithNullAddress_ThrowsArgumentNullException()
        {
            // Arrange
            var cache = new PublisherCache(monitorCache);

            // Act
            TestDelegate addPublisher = () => cache.AddPublisher<IPublisher>(null);

            // Assert
            Assert.That(addPublisher, Throws.ArgumentNullException);
        }


        [Test]
        public void AddPublisher_WithNoFactory_ThrowsMissingFactoryException()
        {
            // Arrange
            var cache = new PublisherCache(monitorCache);

            // Act
            TestDelegate addPublisher = () => cache.AddPublisher<IPublisher>(address);

            // Assert
            Assert.That(addPublisher, Throws.TypeOf<MissingFactoryException>());
        }


        [Test]
        public void AddPublisher_WithFactory_CallsFactoryCreatePublisher()
        {
            // Arrange
            var cache = new PublisherCache(monitorCache);
            cache.AddFactory(factory);

            // Act
            cache.AddPublisher<IPublisher>(address);

            // Assert
            mockFactory.Verify(m => m.CreatePublisher(It.IsIn(address)), Times.Once);
        }


        [Test]
        public void AddPublisher_WithExistingPublisherForAddress_ThrowsInvalidOperationException()
        {
            // Arrange
            var address = TcpAddress.Wildcard(5555);  // Doesn't seem to work when using the mockAddress
            var cache = new PublisherCache(monitorCache);
            cache.AddFactory(factory);
            cache.AddPublisher<IPublisher>(address);

            // Act
            TestDelegate addPublisher = () => cache.AddPublisher<IPublisher>(address);

            // Assert
            Assert.That(addPublisher, Throws.InvalidOperationException);
        }
        #endregion


        #region Publish
        [Test]
        public void Publish_WithNullMessage_ThrowsArgumentNullException()
        {
            // Arrange
            var cache = new PublisherCache(monitorCache);

            // Act
            TestDelegate publish = () => cache.Publish<object>(null);

            // Assert
            Assert.That(publish, Throws.ArgumentNullException);
        }


        [Test]
        public void Publish_WithPublisherAdded_CallsPublish()
        {
            // Arrange
            var topicEvent = "something";
            var cache = new PublisherCache(monitorCache);
            cache.AddFactory(factory);
            cache.AddPublisher<IPublisher>(address);

            // Act
            cache.Publish(topicEvent);

            // Assert
            mockPublisher.Verify(m => m.Publish(It.IsIn(topicEvent)), Times.Once);
        }
        #endregion
    }
}
