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
        private readonly Mock<ISender> mockSender = new Mock<ISender>();

        private ISenderManager senderManager;
        private IMessageFactory messageFactory;
        private ISender sender;

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


        public void SetupMirroredResponse<T>() where T : class
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

            mockSender
                .Setup(m => m.SendAndReceive(It.IsAny<Message>()))
                .Returns<Message>(m => m);
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
            SetupMirroredResponse<string>();

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
            SetupMirroredResponse<string>();

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
            SetupMirroredResponse<string>();

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
            SetupMirroredResponse<string>();

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
            SetupMirroredResponse<string>();

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
            TestDelegate test = () => messageClient.Send<string, object>(request);

            // Assert
            Assert.That(test, Throws.InstanceOf<IOException>());
        }
        #endregion
    }
}
