using System;
using NUnit.Framework;
using MessageRouter.Messages;
using System.Collections.Generic;

namespace MessageRouter.UnitTests.Messages
{
    [TestFixture]
    public class MessageFactoryTests
    {
        private readonly MessageFactory factory = new MessageFactory();


        #region CreateRequest
        [Test]
        public void CreateRequest_WithObject_ReturnsMessage()
        {
            // Arrange
            var request = new object();

            // Act
            var requestMessage = factory.CreateRequest<object>(request);

            // Assert
            Assert.IsNotNull(requestMessage);
        }


        [Test]
        public void CreateRequest_WithMessage_ReturnsSameMessage()
        {
            // Arrange
            var message = new DataMessage<object>(new GuidMessageId(), new object());

            // Act
            var requestMessage = factory.CreateRequest<Message>(message);

            // Assert
            Assert.AreSame(message, requestMessage);
        }


        [Test]
        public void CreateRequest_WithMessageDerivative_ReturnsSameMessage()
        {
            // Arrange
            var message = new DataMessage<object>(new GuidMessageId(), new object());

            // Act
            var requestMessage = factory.CreateRequest<DataMessage<object>>(message);

            // Assert
            Assert.AreSame(message, requestMessage);
        }
        #endregion


        #region CreateResponse
        [Test]
        public void CreateResponse_WithObject_ReturnsMessage()
        {
            // Arrange
            var response = new object();

            // Act
            var responseMessage = factory.CreateResponse<object>(response);

            // Assert
            Assert.IsNotNull(responseMessage);
        }


        [Test]
        public void CreateResponse_WithMessage_ReturnsSameMessage()
        {
            // Arrange
            var message = new DataMessage<object>(new GuidMessageId(), new object());

            // Act
            var responseMessage = factory.CreateResponse<Message>(message);

            // Assert
            Assert.AreSame(message, responseMessage);
        }


        [Test]
        public void CreateResponse_WithMessageDerivative_ReturnsSameMessage()
        {
            // Arrange
            var message = new DataMessage<object>(new GuidMessageId(), new object());

            // Act
            var responseMessage = factory.CreateResponse<DataMessage<object>>(message);

            // Assert
            Assert.AreSame(message, responseMessage);
        }
        #endregion


        #region ExtractRequest
        [Test]
        public void ExtractRequest_WithWrappedObject_ReturnsObject()
        {
            // Arrange
            var request = new object();
            var message = new DataMessage<object>(new GuidMessageId(), request);

            // Act
            var extractedRequest = factory.ExtractRequest(message);

            // Assert
            Assert.AreSame(request, extractedRequest);
        }
        #endregion


        #region ExtractResponse
        [Test]
        public void ExtractResponse_WithWrappedObject_ReturnsObject()
        {
            // Arrange
            var response = new object();
            var message = new DataMessage<object>(new GuidMessageId(), response);

            // Act
            var extractedResponse = factory.ExtractResponse<object>(message);

            // Assert
            Assert.AreSame(response, extractedResponse);
        }


        [Test]
        public void ExtractResponse_WithMessage_ReturnsMessage()
        {
            // Arrange
            var message = new DataMessage<object>(new GuidMessageId(), new object());

            // Act
            var extractedResponse = factory.ExtractResponse<Message>(message);

            // Assert
            Assert.AreSame(message, extractedResponse);
        }


        [Test]
        public void ExtractResponse_WithDifferentWrappedType_ThrowsInvalidCastException()
        {
            // Arrange
            var message = new DataMessage<String>(new GuidMessageId(), String.Empty);

            // Act
            TestDelegate extract = () => factory.ExtractResponse<List<String>>(message);

            // Assert
            Assert.That(extract, Throws.InstanceOf<InvalidCastException>());
        }


        [Test]
        public void ExtractResponse_WithExceptionMessage_RethrowsException()
        {
            // Arrange
            var message = new ExceptionMessage(new GuidMessageId(), new Exception());

            // Act
            TestDelegate extract = () => factory.ExtractResponse<List<String>>(message);

            // Assert
            Assert.That(extract, Throws.Exception);
        }
        #endregion
    }
}
