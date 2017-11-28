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

namespace MessageRouter.UnitTests.Server
{
    [TestFixture]
    public class MessageServerTests
    {
        private readonly Mock<IMessageFactory> mockMessageFactory = new Mock<IMessageFactory>();
        private readonly Mock<IReceiverMonitor> mockReceiverMonitor = new Mock<IReceiverMonitor>();
        private readonly Mock<IRequestDispatcher> mockRequestDispatcher = new Mock<IRequestDispatcher>();

        private IMessageFactory messageFactory;
        private IReceiverMonitor receiverMonitor;
        private IRequestDispatcher requestDispatcher;
        private string name = "something";


        [SetUp]
        public void Setup()
        {
            messageFactory = mockMessageFactory.Object;
            receiverMonitor = mockReceiverMonitor.Object;
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
            mockReceiverMonitor.Reset();
            mockRequestDispatcher.Reset();
        }


        [Test]
        public void MessageServer_WithAllDependencies_Initializes()
        {
            // Act
            var messageServer = new MessageServer(messageFactory, receiverMonitor, requestDispatcher, name);

            // Assert
            Assert.That(messageServer, Is.Not.Null);
        }


        #region Constructor
        [Test]
        public void MessageServer_WithMissingMessageFactory_ThrowsArgumentNullException()
        {
            // Act
            TestDelegate test = () => new MessageServer(null, receiverMonitor, requestDispatcher, name);

            // Assert
            Assert.That(test, Throws.ArgumentNullException);
        }


        [Test]
        public void MessageServer_WithMissingReceiverMonitor_ThrowsArgumentNullException()
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
            TestDelegate test = () => new MessageServer(messageFactory, receiverMonitor, null, name);

            // Assert
            Assert.That(test, Throws.ArgumentNullException);
        }
        #endregion


        #region CreateResponse
        [Test]
        public void CreateResponse_WithValidResponse_CallsMessageFactory()
        {
            // Arrange
            var messageServer = new MessageServer(messageFactory, receiverMonitor, requestDispatcher, name);
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
            var messageServer = new MessageServer(messageFactory, receiverMonitor, requestDispatcher, name);
            string responseObject = null;

            // Act
            TestDelegate test = () => messageServer.CreateResponse(responseObject);

            // Assert
            Assert.That(test, Throws.ArgumentNullException);
        }
        #endregion
    }
}
