using MessageRouter.Messages;
using MessageRouter.Receivers;
using MessageRouter.Server;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessageRouter.Tests.Server
{
    [TestFixture]
    public class MessageServerTests
    {
        private readonly Mock<IMessageFactory> mockMessageFactory = new Mock<IMessageFactory>();
        private readonly Mock<IReceiverManager> mockReceiverManager = new Mock<IReceiverManager>();
        private readonly Mock<IRequestDispatcher> mockRequestDispatcher = new Mock<IRequestDispatcher>();

        private IMessageFactory messageFactory;
        private IReceiverManager receiverManager;
        private IRequestDispatcher requestDispatcher;
        private string name = "something";


        [SetUp]
        public void Setup()
        {
            messageFactory = mockMessageFactory.Object;
            receiverManager = mockReceiverManager.Object;
            requestDispatcher = mockRequestDispatcher.Object;

            mockMessageFactory
                .Setup(m => m.CreateResponse<string>(It.IsAny<string>()))
                .Returns<string>(d => new DataMessage<string>(new GuidMessageId(), d));

            mockMessageFactory
                .Setup(m => m.ExtractRequest(It.IsAny<Message>()))
                .Returns<Message>(m => m.Body);

            mockRequestDispatcher
                .Setup(m => m.Handle(It.IsAny<object>()))
                .Returns<object>(d => d);
        }


        [TearDown]
        public void TearDown()
        {
            mockMessageFactory.Reset();
            mockReceiverManager.Reset();
            mockRequestDispatcher.Reset();
        }


        [Test]
        public void MessageServer_WithAllDependencies_Initializes()
        {
            // Act
            var messageServer = new MessageServer(messageFactory, receiverManager, requestDispatcher, name);

            // Assert
            Assert.That(messageServer, Is.Not.Null);
        }


        #region Constructor
        [Test]
        public void MessageServer_WithMissingMessageFactory_ThrowsArgumentNullException()
        {
            // Act
            TestDelegate test = () => new MessageServer(null, receiverManager, requestDispatcher, name);

            // Assert
            Assert.That(test, Throws.ArgumentNullException);
        }


        [Test]
        public void MessageServer_WithMissingReceiverManager_ThrowsArgumentNullException()
        {
            // Act
            TestDelegate test = () => new MessageServer(messageFactory, null, requestDispatcher, name);

            // Assert
            Assert.That(test, Throws.ArgumentNullException);
        }


        [Test]
        public void MessageServer_WithMissingRequestDispatcher_ThrowsArgumentNullException()
        {
            // Act
            TestDelegate test = () => new MessageServer(messageFactory, receiverManager, null, name);

            // Assert
            Assert.That(test, Throws.ArgumentNullException);
        }
        #endregion


        #region CreateResponse
        [Test]
        public void CreateResponse_WithValidResponse_CallsMessageFactory()
        {
            // Arrange
            var messageServer = new MessageServer(messageFactory, receiverManager, requestDispatcher, name);
            var responseObject = "response";

            // Act
            var responseMessage = messageServer.CreateResponse(responseObject);

            // Assert
            mockMessageFactory
                .Verify(
                    m => m.CreateResponse<string>(It.IsAny<string>()),
                    Times.Once);
        }


        [Test]
        public void CreateResponse_WithNullResponse_ThrowsArgumentException()
        {
            // Arrange
            var messageServer = new MessageServer(messageFactory, receiverManager, requestDispatcher, name);
            string responseObject = null;

            // Act
            TestDelegate test = () => messageServer.CreateResponse(responseObject);

            // Assert
            Assert.That(test, Throws.ArgumentNullException);
        }
        #endregion


        #region HandleAndRespond
        [Test]
        public void HandleAndRespond_WithTask_ExtractsRequest()
        {
            // Arrange
            var messageServer = new MessageServer(messageFactory, receiverManager, requestDispatcher, name);
            var requestObject = "something";
            var message = new DataMessage<string>(new GuidMessageId(), requestObject);
            var responded = false;
            var task = new RequestTask(message, _ => responded = true);

            // Act
            messageServer.HandleAndRespond(task);

            // Assert
            mockMessageFactory
                .Verify(
                    m => m.ExtractRequest(It.IsAny<Message>()),
                    Times.Once);
        }


        [Test]
        public void HandleAndRespond_WithTask_DispatchesRequest()
        {
            // Arrange
            var messageServer = new MessageServer(messageFactory, receiverManager, requestDispatcher, name);
            var requestObject = "something";
            var message = new DataMessage<string>(new GuidMessageId(), requestObject);
            var responded = false;
            var task = new RequestTask(message, _ => responded = true);

            // Act
            messageServer.HandleAndRespond(task);

            // Assert
            mockRequestDispatcher
                .Verify(
                    m => m.Handle(It.IsAny<object>()),
                    Times.Once);
        }


        [Test]
        public void HandleAndRespond_WithTask_CallsReponseAction()
        {
            // Arrange
            var messageServer = new MessageServer(messageFactory, receiverManager, requestDispatcher, name);
            var requestObject = "something";
            var message = new DataMessage<string>(new GuidMessageId(), requestObject);
            var responded = false;
            var task = new RequestTask(message, _ => responded = true);

            // Act
            messageServer.HandleAndRespond(task);

            // Assert
            Assert.That(responded, Is.True);
        }
        #endregion
    }
}
