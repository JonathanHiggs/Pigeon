using System;

using Pigeon.Requests;
using Moq;

using NUnit.Framework;
using System.Threading.Tasks;
using Pigeon.UnitTests.TestFixtures;
using Pigeon.Diagnostics;

namespace Pigeon.UnitTests.Requests
{
    [TestFixture]
    public class RequestDispatcherTests
    {
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


        #region Register
        [Test]
        public void Register_WithRequestHandler_WithUnserializableRequestType_ThrowsUnserializableTypeException()
        {
            // Arrange
            var dispatcher = new RequestDispatcher();
            var handler = new Mock<IRequestHandler<UnserializableRequest, Response>>().Object;

            // Act
            TestDelegate register = () => dispatcher.Register(handler);

            // Assert
            Assert.That(register, Throws.TypeOf<UnserializableTypeException>());
        }


        [Test]
        public void Register_WithRequestHandler_WithUnserializableResponseType_ThrowsUnserializableTypeException()
        {
            // Arrange
            var dispatcher = new RequestDispatcher();
            var handler = new Mock<IRequestHandler<Request, UnserializableResponse>>().Object;

            // Act
            TestDelegate register = () => dispatcher.Register(handler);

            // Assert
            Assert.That(register, Throws.TypeOf<UnserializableTypeException>());
        }
        

        [Test]
        public void Register_WithRequestHandlerDelegate_WithUnserializableRequestType_ThrowsUnserializableTypeException()
        {
            // Arrange
            var dispatcher = new RequestDispatcher();
            RequestHandlerDelegate<UnserializableRequest, Response> handler = request => default(Response);

            // Act
            TestDelegate register = () => dispatcher.Register(handler);

            // Assert
            Assert.That(register, Throws.TypeOf<UnserializableTypeException>());
        }


        [Test]
        public void Register_WithRequestHandlerDelegate_WithUnserializableResponseType_ThrowsUnserializableTypeException()
        {
            // Arrange
            var dispatcher = new RequestDispatcher();
            RequestHandlerDelegate<Request, UnserializableResponse> handler = request => default(UnserializableResponse);

            // Act
            TestDelegate register = () => dispatcher.Register(handler);

            // Assert
            Assert.That(register, Throws.TypeOf<UnserializableTypeException>());
        }
        #endregion


        #region RegisterAsync
        [Test]
        public void RegisterAsync_WithUnserializableRequestType_ThrowsUnserializableTypeException()
        {
            // Arrange
            var dispatcher = new RequestDispatcher();
            AsyncRequestHandlerDelegate<UnserializableRequest, Response> handler = request => Task.FromResult(default(Response));

            // Act
            TestDelegate register = () => dispatcher.RegisterAsync(handler);

            // Assert
            Assert.That(register, Throws.TypeOf<UnserializableTypeException>());
        }


        [Test]
        public void RegisterAsync_WithUnserializableResponseType_ThrowsUnserializableTypeException()
        {
            // Arrange
            var dispatcher = new RequestDispatcher();
            AsyncRequestHandlerDelegate<Request, UnserializableResponse> handler = request => Task.FromResult(default(UnserializableResponse));

            // Act
            TestDelegate register = () => dispatcher.RegisterAsync(handler);

            // Assert
            Assert.That(register, Throws.TypeOf<UnserializableTypeException>());
        }
        #endregion


        #region Handle
        [Test]
        public void Handle_WithNull_ThrowsArgumentNullException()
        {
            // Arrange
            var dispatcher = RequestDispatcher.Create();

            // Act
            AsyncTestDelegate handleNull = async () => await dispatcher.Handle(null);

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
            AsyncTestDelegate handleUnregistered = async () => await dispatcher.Handle(request);

            // Assert
            Assert.That(handleUnregistered, Throws.TypeOf<RequestHandlerNotFoundException>());
        }


        [Test]
        public async Task Handle_WithHandlerRegistered_CallsHandler()
        {
            // Arrange
            var dispatcher = RequestDispatcher.Create();
            dispatcher.Register(handler);
            var request = new Request();

            // Act
            var response = await dispatcher.Handle(request);

            // Assert
            mockHandler.Verify(m => m.Handle(It.IsIn(request)), Times.Once);
        }


        [Test]
        public void Handle_WithBaseClassHandlerRegistered_ThrowsRequestHandlerNotFoundException()
        {
            // Arrange
            var dispatcher = RequestDispatcher.Create();
            dispatcher.Register(handler);
            var request = new SubRequest();

            // Act
            AsyncTestDelegate handle = async () => await dispatcher.Handle(request);

            // Assert
            Assert.That(handle, Throws.TypeOf<RequestHandlerNotFoundException>());
        }


        [Test]
        public async Task Handle_WithHandlerFunctionRegistered_CallsHandler()
        {
            // Arrange
            var handled = false;
            var dispatcher = RequestDispatcher.Create();
            dispatcher.Register<Request, Request>(dt => { handled = true; return dt; });
            var request = new Request();

            // Act
            var response = await dispatcher.Handle(request);

            // Assert
            Assert.That(handled, Is.True);
        }


        [Test]
        public void Handle_WithBaseClassHandlerFunctionRegistered_ThrowsRequestHandlerNotFoundException()
        {
            // Arrange
            var dispatcher = RequestDispatcher.Create();
            dispatcher.Register<Request, Request>(r => r);
            var request = new SubRequest();

            // Act
            AsyncTestDelegate handle = async () => await dispatcher.Handle(request);

            // Assert
            Assert.That(handle, Throws.TypeOf<RequestHandlerNotFoundException>());
        }
        #endregion
    }
}
