using MessageRouter.Addresses;
using MessageRouter.Messages;
using MessageRouter.Monitors;
using MessageRouter.Routing;
using MessageRouter.Senders;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessageRouter.UnitTests.Senders
{
    [TestFixture]
    public class SenderCacheTests
    {
        private readonly Mock<IRequestRouter> mockRequestRouter = new Mock<IRequestRouter>();
        private IRequestRouter requestRouter;

        private readonly Mock<IMonitorCache> mockMonitorCache = new Mock<IMonitorCache>();
        private IMonitorCache monitorCache;
        
        private readonly Mock<ISenderFactory<ISender>> mockFactory = new Mock<ISenderFactory<ISender>>();
        private ISenderFactory<ISender> factory;

        private readonly Mock<ISenderMonitor<ISender>> mockSenderMonitor = new Mock<ISenderMonitor<ISender>>();
        private ISenderMonitor<ISender> senderMonitor;
        
        private readonly Mock<ISender> mockSender = new Mock<ISender>();
        private ISender sender;

        private readonly Mock<IAddress> mockAddress = new Mock<IAddress>();
        private IAddress address;

        private readonly Mock<IMessageFactory> mockMessageFactory = new Mock<IMessageFactory>();
        private IMessageFactory messageFactory;

        private SenderRouting senderRouting;




        [SetUp]
        public void Setup()
        {
            requestRouter = mockRequestRouter.Object;
            monitorCache = mockMonitorCache.Object;
            factory = mockFactory.Object;
            senderMonitor = mockSenderMonitor.Object;
            sender = mockSender.Object;
            address = mockAddress.Object;
            messageFactory = mockMessageFactory.Object;
            senderRouting = SenderRouting.For<ISender>(address);

            mockFactory
                .SetupGet(m => m.SenderType)
                .Returns(typeof(ISender));

            mockFactory
                .SetupGet(m => m.SenderMonitor)
                .Returns(senderMonitor);
            
            mockRequestRouter
                .Setup(m => m.RoutingFor<object>(out senderRouting))
                .Returns(true);

            mockFactory
                .Setup(m => m.CreateSender(It.IsAny<IAddress>()))
                .Returns(sender);

            mockFactory
                .SetupGet(m => m.SenderMonitor)
                .Returns(senderMonitor);
        }


        [TearDown]
        public void TearDown()
        {
            mockRequestRouter.Reset();
            mockMonitorCache.Reset();
            mockFactory.Reset();
            mockSenderMonitor.Reset();
            mockSender.Reset();
            mockAddress.Reset();
            mockMessageFactory.Reset();
        }


        public void SetupMirroredResponse<TRequest>(SenderCache senderCache, TRequest response) where TRequest : class
        {
            mockMessageFactory
                .Setup(m => m.CreateMessage(It.IsAny<TRequest>()))
                .Returns<TRequest>(t => new DataMessage<TRequest>(new GuidMessageId(), t));

            mockMessageFactory
                .Setup(m => m.ExtractMessage<TRequest>(It.IsAny<Message>()))
                .Returns<DataMessage<TRequest>>(m => m.Data);
            
            mockSender
                .Setup(m => m.SendAndReceive(It.IsAny<Message>(), It.IsAny<TimeSpan>()))
                .ReturnsAsync(new DataMessage<TRequest>(new GuidMessageId(), response));

            var senderRouting = SenderRouting.For<ISender>(address);
            mockRequestRouter
                .Setup(m => m.RoutingFor<object>(out senderRouting))
                .Returns(true);

            mockFactory
                .Setup(m => m.CreateSender(It.IsAny<IAddress>()))
                .Returns(sender);

            senderCache.AddFactory(factory);
        }
        

        #region Constructor
        [Test]
        public void SenderCache_WithNullRequestRouter_ThrowsArgumentNullException()
        {
            // Act
            TestDelegate construct = () => new SenderCache(null, monitorCache, messageFactory);

            // Assert
            Assert.That(construct, Throws.ArgumentNullException);
        }


        [Test]
        public void SenderCache_WithNullMonitorCache_ThrowsArgumentNullException()
        {
            // Act
            TestDelegate construct = () => new SenderCache(requestRouter, null, messageFactory);

            // Assert
            Assert.That(construct, Throws.ArgumentNullException);
        }


        [Test]
        public void SenderCache_WithNullMessageFactory_ThrowsArgumentNullException()
        {
            // Act
            TestDelegate construct = () => new SenderCache(requestRouter, monitorCache, null);

            // Assert
            Assert.That(construct, Throws.ArgumentNullException);
        }
        #endregion


        #region AddFactory
        [Test]
        public void AddFactory_WithNullFactory_ThrowsArgumentNullException()
        {
            // Arrange
            var cache = new SenderCache(requestRouter, monitorCache, messageFactory);

            // Act
            TestDelegate addFactory = () => cache.AddFactory<ISender>(null);

            // Assert
            Assert.That(addFactory, Throws.ArgumentNullException);
        }


        [Test]
        public void AddFactory_WithFactory_AddsToFactory()
        {
            // Arrange
            var cache = new SenderCache(requestRouter, monitorCache, messageFactory);

            // Act
            cache.AddFactory(factory);

            // Assert
            CollectionAssert.Contains(cache.Factories, factory);
        }


        [Test]
        public void AddFactory_WithFactory_AddsSenderMonitorToMonitorCache()
        {
            // Arrange
            var cache = new SenderCache(requestRouter, monitorCache, messageFactory);

            // Act
            cache.AddFactory(factory);

            // Assert
            mockMonitorCache.Verify(m => m.AddMonitor(It.IsIn(senderMonitor)), Times.Once);
        }


        [Test]
        public void AddFactory_WithFactoryAlreadyRegistered_DoesNothing()
        {
            // Arrange
            var cache = new SenderCache(requestRouter, monitorCache, messageFactory);
            cache.AddFactory(factory);

            // Act
            cache.AddFactory(factory);

            // Assert
            Assert.That(cache.Factories.Count, Is.EqualTo(1));
            mockMonitorCache.Verify(m => m.AddMonitor(It.IsIn(senderMonitor)), Times.Once);
        }
        #endregion


        #region Send
        [Test]
        public async Task Send_WithSenderReturnRequestObject_ReceivesSameObject()
        {
            // Arrange
            var cache = new SenderCache(requestRouter, monitorCache, messageFactory);
            var request = "Hello";
            SetupMirroredResponse<string>(cache, request);

            // Act
            var response = await cache.Send<string, string>(request);

            // Assert
            Assert.That(response, Is.SameAs(request));
        }


        [Test]
        public async Task Send_WithRequest_CreatesRequestMessage()
        {
            // Arrange
            var cache = new SenderCache(requestRouter, monitorCache, messageFactory);
            var request = "Hello";
            SetupMirroredResponse<string>(cache, request);


            // Act
            var response = await cache.Send<string, string>(request);

            // Assert
            mockMessageFactory
                .Verify(
                    m => m.CreateMessage<object>(It.IsAny<object>()),
                    Times.Once);
        }


        [Test]
        public async Task Send_WithRequest_SendsMessage()
        {
            // Arrange
            var cache = new SenderCache(requestRouter, monitorCache, messageFactory);
            var request = "Hello";
            SetupMirroredResponse<string>(cache, request);

            // Act
            var response = await cache.Send<string, string>(request);

            // Assert
            mockSender
                .Verify(
                    m => m.SendAndReceive(It.IsAny<Message>(), It.IsAny<TimeSpan>()),
                    Times.Once);
        }


        [Test]
        public async Task Send_WithRequest_ExtractsResponse()
        {
            // Arrange
            var cache = new SenderCache(requestRouter, monitorCache, messageFactory);
            var request = "Hello";
            SetupMirroredResponse<string>(cache, request);

            // Act
            var response = await cache.Send<string, string>(request);

            // Assert
            mockMessageFactory
                .Verify(
                    m => m.ExtractMessage<string>(It.IsAny<Message>()),
                    Times.Once);
        }


        [Test]
        public void Send_WithNullRequestObject_ThrowsArgumentException()
        {
            // Arrange
            var cache = new SenderCache(requestRouter, monitorCache, messageFactory);

            // Act
            AsyncTestDelegate send = async () => await cache.Send<object, object>(null);

            // Act & Assert
            Assert.ThrowsAsync<ArgumentNullException>(send);
        }
        #endregion


        #region SenderFor
        [Test]
        public void SenderFor_WithNoRouting_ThrowsKeyNotFoundException()
        {
            // Arrange
            var cache = new SenderCache(requestRouter, monitorCache, messageFactory);

            // Act
            TestDelegate senderFor = () => cache.SenderFor<object>();

            // Assert
            Assert.That(senderFor, Throws.TypeOf<KeyNotFoundException>());
        }


        [Test]
        public void SenderFor_WithNoFactory_ThrowsKeyNotFoundException()
        {
            // Arrange
            var cache = new SenderCache(requestRouter, monitorCache, messageFactory);
            var senderRouting = SenderRouting.For<ISender>(address);

            mockRequestRouter
                .Setup(m => m.RoutingFor<object>(out senderRouting))
                .Returns(true);

            // Act
            TestDelegate senderFor = () => cache.SenderFor<object>();

            // Assert
            Assert.That(senderFor, Throws.TypeOf<KeyNotFoundException>());
        }


        [Test]
        public void SenderFor_WithRoutingAndFactory_ReturnsSender()
        {
            // Arrange
            var cache = new SenderCache(requestRouter, monitorCache, messageFactory);
            var senderRouting = SenderRouting.For<ISender>(address);

            mockRequestRouter
                .Setup(m => m.RoutingFor<object>(out senderRouting))
                .Returns(true);

            mockFactory
                .Setup(m => m.CreateSender(It.IsAny<IAddress>()))
                .Returns(sender);

            cache.AddFactory(factory);

            // Act
            var resolvedSender = cache.SenderFor<object>();

            // Assert
            Assert.That(resolvedSender, Is.EqualTo(sender));
        }


        [Test]
        public void SenderFor_WithNoPreresolvedSender_AddsMonitorToMonitorCache()
        {
            // Arrange
            var cache = new SenderCache(requestRouter, monitorCache, messageFactory);
            cache.AddFactory(factory);

            // Act
            var resolvedSender = cache.SenderFor<object>();

            // Assert
            mockMonitorCache.Verify(m => m.AddMonitor(It.IsIn(senderMonitor)), Times.Once);
        }


        [Test]
        public void SenderFor_WhenCalledTwice_ReturnsTheSameInstance()
        {
            // Arrange
            var cache = new SenderCache(requestRouter, monitorCache, messageFactory);
            cache.AddFactory(factory);

            // Act
            var sender1 = cache.SenderFor<object>();
            var sender2 = cache.SenderFor<object>();

            // Assert
            Assert.That(sender1, Is.SameAs(sender2));
        }


        [Test]
        public void SenderFor_WhenCalledTwice_CallsFactoryOnce()
        {
            // Arrange
            var cache = new SenderCache(requestRouter, monitorCache, messageFactory);
            cache.AddFactory(factory);

            // Act
            var sender1 = cache.SenderFor<object>();
            var sender2 = cache.SenderFor<object>();

            // Assert
            mockFactory.Verify(m => m.CreateSender(It.IsIn(address)), Times.Once);
        }
        #endregion
    }
}
