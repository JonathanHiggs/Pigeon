using MessageRouter.Addresses;
using MessageRouter.Messages;
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
        
        private readonly Mock<ISenderFactory<ISender>> mockSenderFactory = new Mock<ISenderFactory<ISender>>();
        private ISenderFactory<ISender> senderFactory;

        private readonly Mock<ISenderMonitor<ISender>> mockSenderMonitor = new Mock<ISenderMonitor<ISender>>();
        private ISenderMonitor<ISender> senderMonitor;
        
        private readonly Mock<ISender> mockSender = new Mock<ISender>();
        private ISender sender;

        private readonly Mock<IAddress> mockAddress = new Mock<IAddress>();
        private IAddress address;

        private readonly Mock<IMessageFactory> mockMessageFactory = new Mock<IMessageFactory>();
        private IMessageFactory messageFactory;
        

        [SetUp]
        public void Setup()
        {
            requestRouter = mockRequestRouter.Object;
            monitorCache = mockMonitorCache.Object;
            senderFactory = mockSenderFactory.Object;
            senderMonitor = mockSenderMonitor.Object;
            sender = mockSender.Object;
            address = mockAddress.Object;
            messageFactory = mockMessageFactory.Object;
        }


        [TearDown]
        public void TearDown()
        {
            mockRequestRouter.Reset();
            mockMonitorCache.Reset();
            mockSenderFactory.Reset();
            mockSenderMonitor.Reset();
            mockSender.Reset();
            mockAddress.Reset();
            mockMessageFactory.Reset();
        }


        public void SetupMirroredResponse<TRequest>(SenderCache senderCache, TRequest response) where TRequest : class
        {
            mockMessageFactory
                .Setup(m => m.CreateRequest(It.IsAny<TRequest>()))
                .Returns<TRequest>(t => new DataMessage<TRequest>(new GuidMessageId(), t));

            mockMessageFactory
                .Setup(m => m.ExtractResponse<TRequest>(It.IsAny<Message>()))
                .Returns<DataMessage<TRequest>>(m => m.Data);
            
            mockSender
                .Setup(m => m.SendAndReceive(It.IsAny<Message>(), It.IsAny<TimeSpan>()))
                .ReturnsAsync(new DataMessage<TRequest>(new GuidMessageId(), response));

            var senderRouting = SenderRouting.For<ISender>(address);
            mockRequestRouter
                .Setup(m => m.RoutingFor<object>(out senderRouting))
                .Returns(true);

            mockSenderFactory
                .Setup(m => m.CreateSender(It.IsAny<IAddress>()))
                .Returns(sender);

            senderCache.AddFactory(senderFactory);
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
        public void AddFactory_WithFactory_AddsToFactoryList()
        {
            // Arrange
            var senderCache = new SenderCache(requestRouter, monitorCache, messageFactory);

            // Act
            senderCache.AddFactory(senderFactory);

            // Assert
            CollectionAssert.Contains(senderCache.Factories, senderFactory);
        }


        [Test]
        public void AddFactory_WithFactoryAlreadyRegistered_ThrowsInvalidOperationException()
        {
            // Arrange
            var senderCache = new SenderCache(requestRouter, monitorCache, messageFactory);
            senderCache.AddFactory(senderFactory);

            // Act
            TestDelegate addFactory = () => senderCache.AddFactory(senderFactory);

            // Assert
            Assert.That(addFactory, Throws.InvalidOperationException);
        }
        #endregion


        #region Send
        [Test]
        public async Task Send_WithSenderReturnRequestObject_ReceivesSameObject()
        {
            // Arrange
            var senderCache = new SenderCache(requestRouter, monitorCache, messageFactory);
            var request = "Hello";
            SetupMirroredResponse<string>(senderCache, request);

            // Act
            var response = await senderCache.Send<string, string>(request);

            // Assert
            Assert.That(response, Is.SameAs(request));
        }


        [Test]
        public async Task Send_WithRequest_CreatesRequestMessage()
        {
            // Arrange
            var senderCache = new SenderCache(requestRouter, monitorCache, messageFactory);
            var request = "Hello";
            SetupMirroredResponse<string>(senderCache, request);


            // Act
            var response = await senderCache.Send<string, string>(request);

            // Assert
            mockMessageFactory
                .Verify(
                    m => m.CreateRequest<object>(It.IsAny<object>()),
                    Times.Once);
        }


        [Test]
        public async Task Send_WithRequest_SendsMessage()
        {
            // Arrange
            var senderCache = new SenderCache(requestRouter, monitorCache, messageFactory);
            var request = "Hello";
            SetupMirroredResponse<string>(senderCache, request);

            // Act
            var response = await senderCache.Send<string, string>(request);

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
            var senderCache = new SenderCache(requestRouter, monitorCache, messageFactory);
            var request = "Hello";
            SetupMirroredResponse<string>(senderCache, request);

            // Act
            var response = await senderCache.Send<string, string>(request);

            // Assert
            mockMessageFactory
                .Verify(
                    m => m.ExtractResponse<string>(It.IsAny<Message>()),
                    Times.Once);
        }


        [Test]
        public void Send_WithNullRequestObject_ThrowsArgumentException()
        {
            // Arrange
            var senderCache = new SenderCache(requestRouter, monitorCache, messageFactory);

            // Act
            AsyncTestDelegate send = async () => await senderCache.Send<object, object>(null);

            // Act & Assert
            Assert.ThrowsAsync<ArgumentNullException>(send);
        }
        #endregion


        #region SenderFor
        [Test]
        public void SenderFor_WithNoRouting_ThrowsKeyNotFoundException()
        {
            // Arrange
            var senderCache = new SenderCache(requestRouter, monitorCache, messageFactory);

            // Act
            TestDelegate senderFor = () => senderCache.SenderFor<object>();

            // Assert
            Assert.That(senderFor, Throws.TypeOf<KeyNotFoundException>());
        }


        [Test]
        public void SenderFor_WithNoFactory_ThrowsKeyNotFoundException()
        {
            // Arrange
            var senderCache = new SenderCache(requestRouter, monitorCache, messageFactory);
            var senderRouting = SenderRouting.For<ISender>(address);

            mockRequestRouter
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
            var senderCache = new SenderCache(requestRouter, monitorCache, messageFactory);
            var senderRouting = SenderRouting.For<ISender>(address);

            mockRequestRouter
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
            var senderCache = new SenderCache(requestRouter, monitorCache, messageFactory);
            var senderRouting = SenderRouting.For<ISender>(address);

            mockRequestRouter
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
