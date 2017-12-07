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
        private readonly Mock<IRouter> mockMessageRouter = new Mock<IRouter>();
        private IRouter messageRouter;

        private readonly Mock<ISenderFactory<ISender>> mockSenderFactory = new Mock<ISenderFactory<ISender>>();
        private ISenderFactory<ISender> senderFactory;

        private readonly Mock<ISender> mockSender = new Mock<ISender>();
        private ISender sender;

        private readonly Mock<IAddress> mockAddress = new Mock<IAddress>();
        private IAddress address;


        [SetUp]
        public void Setup()
        {
            messageRouter = mockMessageRouter.Object;
            senderFactory = mockSenderFactory.Object;
            sender = mockSender.Object;
            address = mockAddress.Object;
        }


        [TearDown]
        public void TearDown()
        {
            mockMessageRouter.Reset();
            mockSenderFactory.Reset();
        }


        #region Constructor
        [Test]
        public void SenderCache_WithMissingMessageRouter_ThrowsArgumentNullException()
        {
            // Act
            TestDelegate construct = () => new SenderCache(null);

            // Assert
            Assert.That(construct, Throws.ArgumentNullException);
        }
        #endregion


        #region AddFactory
        [Test]
        public void AddFactory_WithFactory_AddsToFactoryList()
        {
            // Arrange
            var senderCache = new SenderCache(messageRouter);

            // Act
            senderCache.AddFactory(senderFactory);

            // Assert
            CollectionAssert.Contains(senderCache.Factories, senderFactory);
        }


        [Test]
        public void AddFactory_WithFactoryAlreadyRegistered_ThrowsInvalidOperationException()
        {
            // Arrange
            var senderCache = new SenderCache(messageRouter);
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
            var senderCache = new SenderCache(messageRouter);

            // Act
            TestDelegate senderFor = () => senderCache.SenderFor<object>();

            // Assert
            Assert.That(senderFor, Throws.TypeOf<KeyNotFoundException>());
        }


        [Test]
        public void SenderFor_WithNoFactory_ThrowsKeyNotFoundException()
        {
            // Arrange
            var senderCache = new SenderCache(messageRouter);
            var senderRouting = SenderRouting.For<ISender>(address);

            mockMessageRouter
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
            var senderCache = new SenderCache(messageRouter);
            var senderRouting = SenderRouting.For<ISender>(address);

            mockMessageRouter
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
        #endregion
    }
}
