using MessageRouter.Addresses;
using MessageRouter.Routing;
using MessageRouter.Senders;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessageRouter.UnitTests.Senders
{
    [TestFixture]
    public class SenderCacheTests
    {
        private readonly Mock<IRequestRouter> mockRouter = new Mock<IRequestRouter>();
        private IRequestRouter router;

        private readonly Mock<IMonitorCache> mockMonitorCache = new Mock<IMonitorCache>();
        private IMonitorCache monitorCache;
        
        private readonly Mock<ISenderFactory<ISender>> mockSenderFactory = new Mock<ISenderFactory<ISender>>();
        private ISenderFactory<ISender> senderFactory;

        private readonly Mock<ISenderMonitor<ISender>> mockSenderMonitor = new Mock<ISenderMonitor<ISender>>();
        private ISenderMonitor<ISender> senderMonitor;
        
        private readonly Mock<ISender> mockSender = new Mock<ISender>();
        private ISender sender;

        private readonly Mock<IAddress> mockAddress = new Mock<IAddress>();
        private IAddress address;


        [SetUp]
        public void Setup()
        {
            router = mockRouter.Object;
            monitorCache = mockMonitorCache.Object;
            senderFactory = mockSenderFactory.Object;
            senderMonitor = mockSenderMonitor.Object;
            sender = mockSender.Object;
            address = mockAddress.Object;
        }


        [TearDown]
        public void TearDown()
        {
            mockRouter.Reset();
            mockMonitorCache.Reset();
            mockSenderFactory.Reset();
            mockSenderMonitor.Reset();
            mockSender.Reset();
            mockAddress.Reset();
        }


        #region Constructor
        [Test]
        public void SenderCache_WithNullMessageRouter_ThrowsArgumentNullException()
        {
            // Act
            TestDelegate construct = () => new SenderCache(null, monitorCache);

            // Assert
            Assert.That(construct, Throws.ArgumentNullException);
        }


        [Test]
        public void SenderCache_WithNullMonitorCache_ThrowsArgumentNullException()
        {
            // Act
            TestDelegate construct = () => new SenderCache(router, null);

            // Assert
            Assert.That(construct, Throws.ArgumentNullException);
        }
        #endregion


        #region AddFactory
        [Test]
        public void AddFactory_WithFactory_AddsToFactoryList()
        {
            // Arrange
            var senderCache = new SenderCache(router, monitorCache);

            // Act
            senderCache.AddFactory(senderFactory);

            // Assert
            CollectionAssert.Contains(senderCache.Factories, senderFactory);
        }


        [Test]
        public void AddFactory_WithFactoryAlreadyRegistered_ThrowsInvalidOperationException()
        {
            // Arrange
            var senderCache = new SenderCache(router, monitorCache);
            senderCache.AddFactory(senderFactory);

            // Act
            TestDelegate addFactory = () => senderCache.AddFactory(senderFactory);

            // Assert
            Assert.That(addFactory, Throws.InvalidOperationException);
        }
        #endregion


        #region SenderFor
        [Test]
        public void SenderFor_WithNoRouting_ThrowsKeyNotFoundException()
        {
            // Arrange
            var senderCache = new SenderCache(router, monitorCache);

            // Act
            TestDelegate senderFor = () => senderCache.SenderFor<object>();

            // Assert
            Assert.That(senderFor, Throws.TypeOf<KeyNotFoundException>());
        }


        [Test]
        public void SenderFor_WithNoFactory_ThrowsKeyNotFoundException()
        {
            // Arrange
            var senderCache = new SenderCache(router, monitorCache);
            var senderRouting = SenderRouting.For<ISender>(address);

            mockRouter
                .Setup(m => m.RoutingFor<object>(out senderRouting))
                .Returns(true);

            // Act
            TestDelegate senderFor = () => senderCache.SenderFor<object>();

            // Assert
            Assert.That(senderFor, Throws.TypeOf<KeyNotFoundException>());
        }


        [Test]
        public void SenderFor_WithRoutingAndFactory_ReturnsSender()
        {
            // Arrange
            var senderCache = new SenderCache(router, monitorCache);
            var senderRouting = SenderRouting.For<ISender>(address);

            mockRouter
                .Setup(m => m.RoutingFor<object>(out senderRouting))
                .Returns(true);

            mockSenderFactory
                .Setup(m => m.CreateSender(It.IsAny<IAddress>()))
                .Returns(sender);

            senderCache.AddFactory(senderFactory);

            // Act
            var resolvedSender = senderCache.SenderFor<object>();

            // Assert
            Assert.That(resolvedSender, Is.EqualTo(sender));
        }


        [Test]
        public void SenderFor_WithNoPreresolvedSender_AddsMonitorToMonitorCache()
        {
            // Arrange
            var senderCache = new SenderCache(router, monitorCache);
            var senderRouting = SenderRouting.For<ISender>(address);

            mockRouter
                .Setup(m => m.RoutingFor<object>(out senderRouting))
                .Returns(true);

            mockSenderFactory
                .Setup(m => m.CreateSender(It.IsAny<IAddress>()))
                .Returns(sender);

            mockSenderFactory
                .SetupGet(m => m.SenderMonitor)
                .Returns(senderMonitor);

            senderCache.AddFactory(senderFactory);

            // Act
            var resolvedSender = senderCache.SenderFor<object>();

            // Assert
            mockMonitorCache.Verify(m => m.AddMonitor(It.IsIn(senderMonitor)), Times.Once);
        }
        #endregion
    }
}
