using System;
using Moq;
using NUnit.Framework;

using Pigeon.Packages;
using Pigeon.Receivers;
using Pigeon.UnitTests.TestFixtures;

namespace Pigeon.UnitTests.Receivers
{
    [TestFixture]
    public class RequestTaskTests
    {
        private readonly Mock<IReceiver> mockReceiver = new Mock<IReceiver>();
        private IReceiver receiver;


        [SetUp]
        public void Setup()
        {
            receiver = mockReceiver.Object;
        }


        [TearDown]
        public void Teardown()
        {
            mockReceiver.Reset();
        }


        private void SendResponse(object message) { }


        [Test]
        public void RequestTask_WithAllRequiredFields_InitializesObject()
        {
            // Arrange
            var request = new Request();

            // Act
            var requestTask = new RequestTask(receiver, request, SendResponse, SendResponse);

            // Assert
            Assert.That(request, Is.EqualTo(requestTask.Request));
        }


        [Test]
        public void RequestTask_WithNullRequest_ThrowsArgumentNullException()
        {
            // Arrange
            Action<object> handler = _ => { };

            // Act
            TestDelegate test = () => new RequestTask(receiver, null, SendResponse, SendResponse);

            // Assert
            Assert.That(test, Throws.ArgumentNullException);
        }


        [Test]
        public void RequestTask_WithNullRequestHandler_ThrowsArgumentNullException()
        {
            // Arrange
            var request = new Request();

            // Act
            TestDelegate test = () => new RequestTask(receiver, request, null, SendResponse);

            // Assert
            Assert.That(test, Throws.ArgumentNullException);
        }
    }
}
