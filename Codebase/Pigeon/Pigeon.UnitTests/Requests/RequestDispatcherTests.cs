using System.Threading.Tasks;

using Moq;

using NUnit.Framework;

using Pigeon.Diagnostics;
using Pigeon.Receivers;
using Pigeon.Requests;
using Pigeon.UnitTests.TestFixtures;

namespace Pigeon.UnitTests.Requests
{
    [TestFixture]
    public class RequestDispatcherTests
    {
        private readonly Mock<IReceiver> mockReceiver = new Mock<IReceiver>();
        private readonly Mock<IRequestHandler<Request, Response>> mockHandler = new Mock<IRequestHandler<Request, Response>>();
        private readonly Mock<IAsyncRequestHandler<Request, Response>> mockAsyncHandler = new Mock<IAsyncRequestHandler<Request, Response>>();

        private IReceiver receiver;
        private IRequestHandler<Request, Response> handler;
        private IAsyncRequestHandler<Request, Response> asyncHandler;

        private readonly Request request = new Request();
        private readonly Response response = new Response();


        [SetUp]
        public void Setup()
        {
            receiver = mockReceiver.Object;
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
            mockReceiver.Reset();
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
        public void Handle_WithNoRegisteredHandler_ThrowsRequestHandlerNotFoundException()
        {
            // Arrange
            var dispatcher = new RequestDispatcher();
            var requestTask = new RequestTask(request, _ => { });

            // Act
            TestDelegate handleUnregistered = () => dispatcher.Handle(receiver, ref requestTask);

            // Assert
            Assert.That(handleUnregistered, Throws.TypeOf<RequestHandlerNotFoundException>());
        }


        [Test]
        public void Handle_WithHandlerRegistered_CallsHandler()
        {
            // Arrange
            var dispatcher = new RequestDispatcher();
            var requestTask = new RequestTask(request, _ => { });
            dispatcher.Register(handler);

            // Act
            dispatcher.Handle(receiver, ref requestTask);

            // Assert
            mockHandler.Verify(m => m.Handle(It.IsIn(request)), Times.Once);
        }


        [Test]
        public void Handler_WithAsyncHandlerRegistered_CallsHandler()
        {
            // Arrange
            var dispatcher = new RequestDispatcher();
            var requestTask = new RequestTask(request, _ => { });
            dispatcher.Register(asyncHandler);

            // Act
            dispatcher.Handle(receiver, ref requestTask);

            // Assert
            mockAsyncHandler.Verify(m => m.Handle(It.IsIn(request)), Times.Once);
        }


        [Test]
        public void Handle_WithBaseClassHandlerRegistered_ThrowsRequestHandlerNotFoundException()
        {
            // Arrange
            var dispatcher = new RequestDispatcher();
            var request = new RequestTask(new SubRequest(), _ => { });
            dispatcher.Register(handler);

            // Act
            TestDelegate handle = () => dispatcher.Handle(receiver, ref request);

            // Assert
            Assert.That(handle, Throws.TypeOf<RequestHandlerNotFoundException>());
        }


        [Test]
        public void Handle_WithHandlerFunctionRegistered_CallsHandler()
        {
            // Arrange
            var handled = false;
            var dispatcher = new RequestDispatcher();
            var requestTask = new RequestTask(request, _ => { });
            dispatcher.Register<Request, Response>(dt => { handled = true; return response; });

            // Act
            dispatcher.Handle(receiver, ref requestTask);

            // Assert
            Assert.That(handled, Is.True);
        }


        [Test]
        public void Handle_WithBaseClassHandlerFunctionRegistered_ThrowsRequestHandlerNotFoundException()
        {
            // Arrange
            var dispatcher = new RequestDispatcher();
            var request = new RequestTask(new SubRequest(), _ => { });
            dispatcher.Register<Request, Response>(r => response);

            // Act
            TestDelegate handle = () => dispatcher.Handle(receiver, ref request);

            // Assert
            Assert.That(handle, Throws.TypeOf<RequestHandlerNotFoundException>());
        }


        [Test]
        public void Handle_WithHandlerRegistered_ReturnsResponse()
        {
            // Arrange
            Response ret = null;
            var dispatcher = new RequestDispatcher();
            var requestTask = new RequestTask(request, r => ret = (Response)r);
            dispatcher.Register<Request, Response>(r => response);

            // Act
            dispatcher.Handle(receiver, ref requestTask);

            // Assert
            Assert.That(ret is Response, Is.True);
            Assert.That(ret, Is.SameAs(response));
        }


        [Test]
        public void Handle_WithAsyncHandlerRegistered_ReturnsResponse()
        {
            // Arrange
            Response ret = null;
            var dispatcher = new RequestDispatcher();
            var requestTask = new RequestTask(request, r => ret = (Response)r);
            AsyncRequestHandlerDelegate<Request, Response> handler = request => Task.FromResult(response);
            dispatcher.RegisterAsync(handler);

            // Act
            dispatcher.Handle(receiver, ref requestTask);

            // Assert
            Assert.That(ret is Response, Is.True);
            Assert.That(ret, Is.SameAs(response));
        }


        [Test]
        public void Handle_WithHandlerClassRegistered_ReturnsResponse()
        {
            // Arrange
            Response ret = null;
            var dispatcher = new RequestDispatcher();
            var requestTask = new RequestTask(request, r => ret = (Response)r);
            dispatcher.Register(handler);

            // Act
            dispatcher.Handle(receiver, ref requestTask);

            // Assert
            Assert.That(ret is Response, Is.True);
            Assert.That(ret, Is.SameAs(response));
        }


        [Test]
        public void Handle_WithAsyncHandlerClassRegistered_ReturnsResponse()
        {
            // Arrange
            Response ret = null;
            var dispatcher = new RequestDispatcher();
            var requestTask = new RequestTask(request, r => ret = (Response)r);
            dispatcher.Register(asyncHandler);

            // Act
            dispatcher.Handle(receiver, ref requestTask);

            // Assert
            Assert.That(ret is Response, Is.True);
            Assert.That(ret, Is.SameAs(response));
        }

        #endregion
    }
}
