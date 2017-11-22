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
        private readonly Mock<ISenderManager> mockSenderManager = new Mock<ISenderManager>();
        private readonly Mock<IMessageFactory> mockMessageFactory = new Mock<IMessageFactory>();
        private readonly Mock<IAsyncSender> mockSender = new Mock<IAsyncSender>();

        private ISenderManager senderManager;
        private IMessageFactory messageFactory;
        private IAsyncSender sender;

        [SetUp]
        public void Setup()
        {
            senderManager = mockSenderManager.Object;
            messageFactory = mockMessageFactory.Object;
            sender = mockSender.Object;
        }


        [TearDown]
        public void TearDown()
        {
            mockSenderManager.Reset();
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

            mockSenderManager
                .Setup(m => m.SenderFor<T>())
                .Returns(sender);

            mockSenderManager
                .Setup(m => m.AsyncSenderFor<T>())
                .Returns(sender);

            mockSender
                .Setup(m => m.SendAndReceive(It.IsAny<Message>()))
                .Returns<Message>(m => m);
            
            mockSender
                .Setup(m => m.SendAndReceiveAsync(It.IsAny<Message>(), It.IsAny<TimeSpan>()))
                .ReturnsAsync(new DataMessage<T>(new GuidMessageId(), val));
        }


        #region Constructor
        [Test]
        public void MessageClient_WithDependencies_Initializes()
        {
            // Act
            var messageClient = new MessageClient(senderManager, messageFactory);

            // Assert
            Assert.That(messageClient, Is.Not.Null);
        }


        [Test]
        public void MessageClient_WithMissingSenderManager_ThrowsArugmentNullException()
        {
            // Act
            TestDelegate test = () => new MessageClient(null, messageFactory);

            // Assert
            Assert.That(test, Throws.ArgumentNullException);
        }


        [Test]
        public void MessageClient_WithMissingMessageFactory_ThrowsArugmentNullException()
        {
            // Act
            TestDelegate test = () => new MessageClient(senderManager, null);

            // Assert
            Assert.That(test, Throws.ArgumentNullException);
        }
        #endregion


        #region Send
        [Test]
        public void Send_WithSenderReturnRequestObject_ReceivesSameObject()
        {
            // Arrange
            var messageClient = new MessageClient(senderManager, messageFactory);
            var request = "Hello";
            SetupMirroredResponse<string>(request);

            // Act
            var response = messageClient.Send<string, string>(request);

            // Assert
            Assert.That(response, Is.SameAs(request));
        }


        [Test]
        public void Send_WithRequest_CreatesRequestMessage()
        {
            // Arrange
            var messageClient = new MessageClient(senderManager, messageFactory);
            var request = "Hello";
            SetupMirroredResponse<string>(request);

            // Act
            var response = messageClient.Send<string, string>(request);

            // Assert
            mockMessageFactory
                .Verify(
                    m => m.CreateRequest<object>(It.IsAny<object>()),
                    Times.Once);
        }


        [Test]
        public void Send_WithRequest_ResolvesSender()
        {
            // Arrange
            var messageClient = new MessageClient(senderManager, messageFactory);
            var request = "Hello";
            SetupMirroredResponse<string>(request);

            // Act
            var response = messageClient.Send<string, string>(request);

            // Assert
            mockSenderManager
                .Verify(
                    m => m.SenderFor<object>(),
                    Times.Once);
        }


        [Test]
        public void Send_WithRequest_SendsMessage()
        {
            // Arrange
            var messageClient = new MessageClient(senderManager, messageFactory);
            var request = "Hello";
            SetupMirroredResponse<string>(request);

            // Act
            var response = messageClient.Send<string, string>(request);

            // Assert
            mockSender
                .Verify(
                    m => m.SendAndReceive(It.IsAny<Message>()),
                    Times.Once);
        }


        [Test]
        public void Send_WithRequest_ExtractsResponse()
        {
            // Arrange
            var messageClient = new MessageClient(senderManager, messageFactory);
            var request = "Hello";
            SetupMirroredResponse<string>(request);

            // Act
            var response = messageClient.Send<string, string>(request);

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
            var messageClient = new MessageClient(senderManager, messageFactory);

            // Act
            TestDelegate test = () => messageClient.Send<object, object>(null);

            // Assert
            Assert.That(test, Throws.ArgumentNullException);
        }


        [Test]
        public void Send_WithExceptionResponse_ThrowsException()
        {
            // Arrange
            var messageClient = new MessageClient(senderManager, messageFactory);
            var request = new object();
            var exception = new IOException();

            mockSender
                .Setup(m => m.SendAndReceive(It.IsAny<Message>()))
                .Returns(new DataMessage<Exception>(new GuidMessageId(), exception));

            // Act
            TestDelegate test = () => messageClient.Send<object, string>(request);

            // Assert
            Assert.That(test, Throws.InstanceOf<IOException>());
        }
        #endregion


        #region SendAsync
        [Test]
        public async Task SendAsync_WithSenderReturnRequestObject_ReceivesSameObject()
        {
            // Arrange
            var messageClient = new MessageClient(senderManager, messageFactory);
            var request = "Hello";
            SetupMirroredResponse<string>(request);

            // Act
            var response = await messageClient.SendAsync<string, string>(request);

            // Assert
            Assert.That(response, Is.SameAs(request));
        }


        [Test]
        public async Task SendAsync_WithRequest_CreatesRequestMessage()
        {
            // Arrange
            var messageClient = new MessageClient(senderManager, messageFactory);
            var request = "Hello";
            SetupMirroredResponse<string>(request);

            // Act
            var response = await messageClient.SendAsync<string, string>(request);

            // Assert
            mockMessageFactory
                .Verify(
                    m => m.CreateRequest<object>(It.IsAny<object>()),
                    Times.Once);
        }


        [Test]
        public async Task SendAsync_WithRequest_ResolvesSender()
        {
            // Arrange
            var messageClient = new MessageClient(senderManager, messageFactory);
            var request = "Hello";
            SetupMirroredResponse<string>(request);

            // Act
            var response = await messageClient.SendAsync<string, string>(request);

            // Assert
            mockSenderManager
                .Verify(
                    m => m.AsyncSenderFor<object>(),
                    Times.Once);
        }


        [Test]
        public async Task SendAsync_WithRequest_SendsMessage()
        {
            // Arrange
            var messageClient = new MessageClient(senderManager, messageFactory);
            var request = "Hello";
            SetupMirroredResponse<string>(request);

            // Act
            var response = await messageClient.SendAsync<string, string>(request);

            // Assert
            mockSender
                .Verify(
                    m => m.SendAndReceiveAsync(It.IsAny<Message>(), It.IsAny<TimeSpan>()),
                    Times.Once);
        }


        [Test]
        public async Task SendAsync_WithRequest_ExtractsResponse()
        {
            // Arrange
            var messageClient = new MessageClient(senderManager, messageFactory);
            var request = "Hello";
            SetupMirroredResponse<string>(request);

            // Act
            var response = await messageClient.SendAsync<string, string>(request);

            // Assert
            mockMessageFactory
                .Verify(
                    m => m.ExtractResponse<string>(It.IsAny<Message>()),
                    Times.Once);
        }


        [Test]
        public void SendAsync_WithNullRequestObject_ThrowsArgumentException()
        {
            // Arrange
            var messageClient = new MessageClient(senderManager, messageFactory);
            
            // Act & Assert
            Assert.That(async () => await messageClient.SendAsync<object, object>(null), Throws.ArgumentNullException);
        }


        [Test]
        public void SendAsync_WithExceptionResponse_ThrowsException()
        {
            // Arrange
            var messageClient = new MessageClient(senderManager, messageFactory);
            var request = new object();
            var exception = new IOException();

            mockSender
                .Setup(m => m.SendAndReceive(It.IsAny<Message>()))
                .Returns(new DataMessage<Exception>(new GuidMessageId(), exception));

            // Act
            TestDelegate test = async () => await messageClient.SendAsync<object, string>(request);

            // Assert
            Assert.That(test, Throws.InstanceOf<IOException>());
        }
        #endregion


        #region Start
        [Test]
        public void Start_WithNotStarted_StartsSenderManager()
        {
            // Arrange
            var client = new MessageClient(senderManager, messageFactory);

            // Act
            client.Start();

            // Assert
            mockSenderManager.Verify(m => m.Start(), Times.Once);
        }


        [Test]
        public void Start_WithAlreadyStarted_ThrowsInvalidOperationException()
        {
            // Arrange
            var client = new MessageClient(senderManager, messageFactory);
            client.Start();

            // Act
            TestDelegate test = () => client.Start();

            // Assert
            Assert.That(test, Throws.InvalidOperationException);
        }
        #endregion


        #region Stop
        [Test]
        public void Stop_WithRunning_StopsSenderManager()
        {
            // Arrange
            var client = new MessageClient(senderManager, messageFactory);
            client.Start();

            // Act
            client.Stop();

            // Assert
            mockSenderManager.Verify(m => m.Stop(), Times.Once);
        }


        [Test]
        public void Stop_WithNotRunning_ThrowsInvalidOperationException()
        {
            // Arrange
            var client = new MessageClient(senderManager, messageFactory);

            // Act
            TestDelegate test = () => client.Stop();

            // Assert
            Assert.That(test, Throws.InvalidOperationException);
        }
        #endregion


        #region IsRunning
        [Test]
        public void IsRunning_WhenNotStarted_IsFalse()
        {
            // Arrange
            var client = new MessageClient(senderManager, messageFactory);

            // Act
            var running = client.IsRunning;

            // Assert
            Assert.That(running, Is.False);
        }


        [Test]
        public void IsRunning_WhenStarted_IsTrue()
        {
            // Arrange
            var client = new MessageClient(senderManager, messageFactory);
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
            var client = new MessageClient(senderManager, messageFactory);
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
