using System;

using MessageRouter.Requests;
using Moq;

using NUnit.Framework;

namespace MessageRouter.UnitTests.Requests
{
    [TestFixture]
    public class RequestDispatcherTests
    {
        public class Request { }
        public class SubRequest : Request { }

        private readonly Mock<IRequestHandler<Request, Request>> mockHandler = new Mock<IRequestHandler<Request, Request>>();
        private IRequestHandler<Request, Request> handler;


        [SetUp]
        public void Setup()
        {
            handler = mockHandler.Object;
        }


        [TearDown]
        public void Teaddown()
        {
            mockHandler.Reset();
        }


        #region Handle
        [Test]
        public void Handle_WithNull_ThrowsArgumentNullException()
        {
            // Arrange
            var dispatcher = RequestDispatcher.Create();

            // Act
            TestDelegate handleNull = () => dispatcher.Handle(null);

            // Assert
            Assert.That(handleNull, Throws.ArgumentNullException);
        }


        [Test]
        public void Handle_WithNoRegisteredHandler_ThrowsRequestHandlerNotFoundException()
        {
            // Arrange
            var dispatcher = RequestDispatcher.Create();
            var request = new Request();

            // Act
            TestDelegate handleUnregistered = () => dispatcher.Handle(request);

            // Assert
            Assert.That(handleUnregistered, Throws.TypeOf<RequestHandlerNotFoundException>());
        }


        [Test]
        public void Handle_WithHandlerRegistered_CallsHandler()
        {
            // Arrange
            var dispatcher = RequestDispatcher.Create().Register(handler);
            var request = new Request();

            // Act
            var response = dispatcher.Handle(request);

            // Assert
            mockHandler.Verify(m => m.Handle(It.IsIn(request)), Times.Once);
        }


        [Test]
        public void Handle_WithBaseClassHandlerRegistered_ThrowsRequestHandlerNotFoundException()
        {
            // Arrange
            var dispatcher = RequestDispatcher.Create().Register(handler);
            var request = new SubRequest();

            // Act
            TestDelegate handle = () => dispatcher.Handle(request);

            // Assert
            Assert.That(handle, Throws.TypeOf<RequestHandlerNotFoundException>());
        }


        [Test]
        public void Handle_WithHandlerFunctionRegistered_CallsHandler()
        {
            // Arrange
            var handled = false;
            var dispatcher = RequestDispatcher.Create().Register<Request, Request>(dt => { handled = true; return dt; });
            var request = new Request();

            // Act
            var response = dispatcher.Handle(request);

            // Assert
            Assert.That(handled, Is.True);
        }


        [Test]
        public void Handle_WithBaseClassHandlerFunctionRegistered_ThrowsRequestHandlerNotFoundException()
        {
            // Arrange
            var dispatcher = RequestDispatcher.Create().Register<Request, Request>(r => r);
            var request = new SubRequest();

            // Act
            TestDelegate handle = () => dispatcher.Handle(request);

            // Assert
            Assert.That(handle, Throws.TypeOf<RequestHandlerNotFoundException>());
        }
        #endregion
    }
}
