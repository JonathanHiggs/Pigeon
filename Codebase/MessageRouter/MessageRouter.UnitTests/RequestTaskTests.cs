using System;
using NUnit.Framework;
using MessageRouter.Messages;

namespace MessageRouter.UnitTests
{
    [TestFixture]
    public class RequestTaskTests
    {
        [Test]
        public void RequestTask_WithAllRequiredFields_InitializesObject()
        {
            // Arrange
            var request = new DataMessage<object>(new GuidMessageId(), new object());
            Action<Message> handler = _ => { };

            // Act
            var requestTask = new RequestTask(request, handler);

            // Assert
            Assert.AreSame(request, requestTask.Request);
            Assert.AreSame(handler, requestTask.ResponseHandler);
        }


        [Test]
        public void RequestTask_WithNullRequest_ThrowsArgumentNullException()
        {
            // Arrange
            Action<Message> handler = _ => { };

            // Act
            TestDelegate test = () => new RequestTask(null, handler);

            // Assert
            Assert.That(test, Throws.ArgumentNullException);
        }


        [Test]
        public void RequestTask_WithNullRequestHandler_ThrowsArgumentNullException()
        {
            // Arrange
            var request = new DataMessage<object>(new GuidMessageId(), new object());

            // Act
            TestDelegate test = () => new RequestTask(request, null);

            // Assert
            Assert.That(test, Throws.ArgumentNullException);
        }
    }
}
