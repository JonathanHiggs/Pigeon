using MessageRouter.Client;
using MessageRouter.Messages;
using MessageRouter.Senders;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessageRouter.UnitTests.Client
{
    [TestFixture]
    public class MessageClientTests
    {
        private readonly Mock<ISenderCache> mockSenderCache = new Mock<ISenderCache>();
        private ISenderCache senderCache;

        private readonly Mock<IMonitorCache> mockMonitorCache = new Mock<IMonitorCache>();
        private IMonitorCache monitorCache;
        
        private readonly Mock<IMessageFactory> mockMessageFactory = new Mock<IMessageFactory>();
        private IMessageFactory messageFactory;

        private readonly Mock<ISender> mockSender = new Mock<ISender>();
        private ISender sender;

        private readonly Mock<ISenderMonitor> mockSenderMonitor = new Mock<ISenderMonitor>();
        private ISenderMonitor senderMonitor;

        
        [SetUp]
        public void Setup()
        {
            senderCache = mockSenderCache.Object;
            monitorCache = mockMonitorCache.Object;
            messageFactory = mockMessageFactory.Object;
            sender = mockSender.Object;
            senderMonitor = mockSenderMonitor.Object;
        }


        [TearDown]
        public void TearDown()
        {
            mockSenderCache.Reset();
            mockMonitorCache.Reset();
            mockMessageFactory.Reset();
            mockSender.Reset();
        }


        public void SetupMirroredResponse<T>(T val) where T : class
        {
            mockMessageFactory
                .Setup(m => m.CreateRequest<T>(It.IsAny<T>()))
                .Returns<T>(t => new DataMessage<T>(new GuidMessageId(), t));

            mockMessageFactory
                .Setup(m => m.ExtractResponse<T>(It.IsAny<Message>()))
                .Returns<DataMessage<T>>(m => m.Data);

            mockSenderCache
                .Setup(m => m.SenderFor<T>())
                .Returns(sender);
            
            mockSender
                .Setup(m => m.SendAndReceive(It.IsAny<Message>(), It.IsAny<TimeSpan>()))
                .ReturnsAsync(new DataMessage<T>(new GuidMessageId(), val));
        }


        #region Constructor
        [Test]
        public void MessageClient_WithDependencies_Initializes()
        {
            // Act
            TestDelegate construct = () => new MessageClient(senderCache, monitorCache, messageFactory);

            // Assert
            Assert.That(construct, Throws.Nothing);
        }


        [Test]
        public void MessageClient_WithMissingSenderCache_ThrowsArugmentNullException()
        {
            // Act
            TestDelegate construct = () => new MessageClient(null, monitorCache, messageFactory);

            // Assert
            Assert.That(construct, Throws.ArgumentNullException);
        }


        [Test]
        public void MessageClient_WithMissingMonitorCache_ThrowsArgumentNullException()
        {
            // Act
            TestDelegate construct = () => new MessageClient(senderCache, null, messageFactory);

            // Assert
            Assert.That(construct, Throws.ArgumentNullException);
        }


        [Test]
        public void MessageClient_WithMissingMessageFactory_ThrowsArugmentNullException()
        {
            // Act
            TestDelegate construct = () => new MessageClient(senderCache, monitorCache, null);

            // Assert
            Assert.That(construct, Throws.ArgumentNullException);
        }
        #endregion


        #region Send
        [Test]
        public async Task Send_WithSenderReturnRequestObject_ReceivesSameObject()
        {
            // Arrange
            var messageClient = new MessageClient(senderCache, monitorCache, messageFactory);
            var request = "Hello";
            SetupMirroredResponse<string>(request);

            // Act
            var response = await messageClient.Send<string, string>(request);

            // Assert
            Assert.That(response, Is.SameAs(request));
        }


        [Test]
        public async Task Send_WithRequest_CreatesRequestMessage()
        {
            // Arrange
            var messageClient = new MessageClient(senderCache, monitorCache, messageFactory);
            var request = "Hello";
            SetupMirroredResponse<string>(request);

            // Act
            var response = await messageClient.Send<string, string>(request);

            // Assert
            mockMessageFactory
                .Verify(
                    m => m.CreateRequest<object>(It.IsAny<object>()),
                    Times.Once);
        }


        [Test]
        public async Task Send_WithRequest_ResolvesSender()
        {
            // Arrange
            var messageClient = new MessageClient(senderCache, monitorCache, messageFactory);
            var request = "Hello";
            SetupMirroredResponse<string>(request);

            // Act
            var response = await messageClient.Send<string, string>(request);

            // Assert
            mockSenderCache
                .Verify(
                    m => m.SenderFor<object>(),
                    Times.Once);
        }


        [Test]
        public async Task Send_WithRequest_SendsMessage()
        {
            // Arrange
            var messageClient = new MessageClient(senderCache, monitorCache, messageFactory);
            var request = "Hello";
            SetupMirroredResponse<string>(request);

            // Act
            var response = await messageClient.Send<string, string>(request);

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
            var messageClient = new MessageClient(senderCache, monitorCache, messageFactory);
            var request = "Hello";
            SetupMirroredResponse<string>(request);

            // Act
            var response = await messageClient.Send<string, string>(request);

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
            var messageClient = new MessageClient(senderCache, monitorCache, messageFactory);
            
            // Act & Assert
            Assert.That(async () => await messageClient.Send<object, object>(null), Throws.ArgumentNullException);
        }


        [Test]
        public void Send_WithExceptionResponse_ThrowsException()
        {
            // Arrange
            var messageClient = new MessageClient(senderCache, monitorCache, messageFactory);
            var request = new object();
            var exception = new IOException();

            mockSender
                .Setup(m => m.SendAndReceive(It.IsAny<Message>(), It.IsAny<TimeSpan>()))
                .ReturnsAsync(new DataMessage<Exception>(new GuidMessageId(), exception));

            // Act
            TestDelegate send = async () => await messageClient.Send<object, string>(request);

            // Assert
            Assert.That(send, Throws.InstanceOf<IOException>());
        }
        #endregion


        #region Start
        [Test]
        public void Start_WithNotStarted_StartsSenderMonitor()
        {
            // Arrange
            var client = new MessageClient(senderCache, monitorCache, messageFactory);

            // Act
            client.Start();

            // Assert
            mockMonitorCache.Verify(m => m.StartAllMonitors(), Times.Once);
        }


        [Test]
        public void Start_WithAlreadyStarted_DoesNothing()
        {
            // Arrange
            var client = new MessageClient(senderCache, monitorCache, messageFactory);
            client.Start();
            mockMonitorCache.ResetCalls();

            // Act
            client.Start();

            // Assert
            mockMonitorCache.Verify(m => m.StartAllMonitors(), Times.Never);
        }
        #endregion


        #region Stop
        [Test]
        public void Stop_WithRunning_StopsSenderMonitor()
        {
            // Arrange
            var client = new MessageClient(senderCache, monitorCache, messageFactory);
            client.Start();

            // Act
            client.Stop();

            // Assert
            mockMonitorCache.Verify(m => m.StopAllMonitors(), Times.Once);
        }


        [Test]
        public void Stop_WithNotRunning_DoesNothing()
        {
            // Arrange
            var client = new MessageClient(senderCache, monitorCache, messageFactory);

            // Act
            client.Stop();

            // Assert
            mockMonitorCache.Verify(m => m.StopAllMonitors(), Times.Never);
        }


        [Test]
        public void Stop_WithStartedAndStopped_DoesNothing()
        {
            // Arrange
            var client = new MessageClient(senderCache, monitorCache, messageFactory);
            client.Start();
            client.Stop();
            mockMonitorCache.ResetCalls();

            // Act
            client.Stop();

            // Assert
            mockMonitorCache.Verify(m => m.StopAllMonitors(), Times.Never);
        }
        #endregion


        #region IsRunning
        [Test]
        public void IsRunning_WhenNotStarted_IsFalse()
        {
            // Arrange
            var client = new MessageClient(senderCache, monitorCache, messageFactory);

            // Act
            var running = client.IsRunning;

            // Assert
            Assert.That(running, Is.False);
        }


        [Test]
        public void IsRunning_WhenStarted_IsTrue()
        {
            // Arrange
            var client = new MessageClient(senderCache, monitorCache, messageFactory);
            client.Start();

            // Act
            var running = client.IsRunning;

            // Assert
            Assert.That(running, Is.True);
        }


        [Test]
        public void IsRunning_WhenStartedAndStopped_IsFalse()
        {
            // Arrange
            var client = new MessageClient(senderCache, monitorCache, messageFactory);
            client.Start();
            client.Stop();

            // Act
            var running = client.IsRunning;

            // Assert
            Assert.That(running, Is.False);
        }
        #endregion
    }
}
