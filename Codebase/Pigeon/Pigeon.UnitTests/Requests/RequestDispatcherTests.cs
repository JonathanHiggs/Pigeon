using System.Threading.Tasks;

using Moq;

using NUnit.Framework;

using Pigeon.Diagnostics;
using Pigeon.Requests;
using Pigeon.UnitTests.TestFixtures;

namespace Pigeon.UnitTests.Requests
{
    [TestFixture]
    public class RequestDispatcherTests
    {
        private readonly Mock<IRequestHandler<Request, Response>> mockHandler = new Mock<IRequestHandler<Request, Response>>();
        private readonly Mock<IAsyncRequestHandler<Request, Response>> mockAsyncHandler = new Mock<IAsyncRequestHandler<Request, Response>>();

        private IRequestHandler<Request, Response> handler;
        private IAsyncRequestHandler<Request, Response> asyncHandler;

        private readonly Request request = new Request();
        private readonly Response response = new Response();


        [SetUp]
        public void Setup()
        {
            handler = mockHandler.Object;
            asyncHandler = mockAsyncHandler.Object;

            mockHandler
                .Setup(m => m.Handle(It.IsAny<Request>()))
                .Returns(response);

            mockAsyncHandler
                .Setup(m => m.Handle(It.IsAny<Request>()))
                .Returns(Task.FromResult(response));
        }


        [TearDown]
        public void Teaddown()
        {
            mockHandler.Reset();
            mockAsyncHandler.Reset();
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
            RequestHandlerDelegate<UnserializableRequest, Response> handler = request => response;

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
            AsyncRequestHandlerDelegate<UnserializableRequest, Response> handler = request => Task.FromResult(response);

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
            var dispatcher = new RequestDispatcher();

            // Act
            TestDelegate handleNull = () => dispatcher.Handle(null);

            // Assert
            Assert.That(handleNull, Throws.ArgumentNullException);
        }


        [Test]
        public void Handle_WithNoRegisteredHandler_ThrowsRequestHandlerNotFoundException()
        {
            // Arrange
            var dispatcher = new RequestDispatcher();

            // Act
            TestDelegate handleUnregistered = () => dispatcher.Handle(request);

            // Assert
            Assert.That(handleUnregistered, Throws.TypeOf<RequestHandlerNotFoundException>());
        }


        [Test]
        public void Handle_WithHandlerRegistered_CallsHandler()
        {
            // Arrange
            var dispatcher = new RequestDispatcher();
            dispatcher.Register(handler);

            // Act
            var response = dispatcher.Handle(request);

            // Assert
            mockHandler.Verify(m => m.Handle(It.IsIn(request)), Times.Once);
        }


        [Test]
        public void Handler_WithAsyncHandlerRegistered_CallsHandler()
        {
            // Arrange
            var dispatcher = new RequestDispatcher();
            dispatcher.Register(asyncHandler);

            // Act
            var response = dispatcher.Handle(request);

            // Assert
            mockAsyncHandler.Verify(m => m.Handle(It.IsIn(request)), Times.Once);
        }


        [Test]
        public void Handle_WithBaseClassHandlerRegistered_ThrowsRequestHandlerNotFoundException()
        {
            // Arrange
            var dispatcher = new RequestDispatcher();
            dispatcher.Register(handler);
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
            var dispatcher = new RequestDispatcher();
            dispatcher.Register<Request, Request>(dt => { handled = true; return dt; });

            // Act
            var response = dispatcher.Handle(request);

            // Assert
            Assert.That(handled, Is.True);
        }


        [Test]
        public void Handle_WithBaseClassHandlerFunctionRegistered_ThrowsRequestHandlerNotFoundException()
        {
            // Arrange
            var dispatcher = new RequestDispatcher();
            dispatcher.Register<Request, Request>(r => r);
            var request = new SubRequest();

            // Act
            TestDelegate handle = () => dispatcher.Handle(request);

            // Assert
            Assert.That(handle, Throws.TypeOf<RequestHandlerNotFoundException>());
        }


        [Test]
        public void Handle_WithHandlerRegistered_ReturnsResponse()
        {
            // Arrange
            var dispatcher = new RequestDispatcher();
            dispatcher.Register<Request, Response>(r => response);

            // Act
            var ret = dispatcher.Handle(request);

            // Assert
            Assert.That(ret is Response, Is.True);
            Assert.That(ret, Is.SameAs(response));
        }


        [Test]
        public void Handle_WithAsyncHandlerRegistered_ReturnsResponse()
        {
            // Arrange
            var dispatcher = new RequestDispatcher();
            AsyncRequestHandlerDelegate<Request, Response> handler = request => Task.FromResult(response);
            dispatcher.RegisterAsync(handler);

            // Act
            var ret = dispatcher.Handle(request);

            // Assert
            Assert.That(ret is Response, Is.True);
            Assert.That(ret, Is.SameAs(response));
        }


        [Test]
        public void Handle_WithHandlerClassRegistered_ReturnsResponse()
        {
            // Arrange
            var dispatcher = new RequestDispatcher();
            dispatcher.Register(handler);

            // Act
            var ret = dispatcher.Handle(request);

            // Assert
            Assert.That(ret is Response, Is.True);
            Assert.That(ret, Is.SameAs(response));
        }


        [Test]
        public void Handle_WithAsyncHandlerClassRegistered_ReturnsResponse()
        {
            // Arrange
            var dispatcher = new RequestDispatcher();
            dispatcher.Register(asyncHandler);

            // Act
            var ret = dispatcher.Handle(request);

            // Assert
            Assert.That(ret is Response, Is.True);
            Assert.That(ret, Is.SameAs(response));
        }

        #endregion
    }
}
