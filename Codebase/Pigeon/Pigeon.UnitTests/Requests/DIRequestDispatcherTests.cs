
using System.Threading.Tasks;

using Moq;

using NUnit.Framework;

using Pigeon.Diagnostics;
using Pigeon.Requests;
using Pigeon.UnitTests.TestFixtures;

namespace Pigeon.UnitTests.Requests
{
    [TestFixture]
    public class DIRequestDispatcherTests
    {
        private readonly Mock<IRequestHandler<Request, Response>> mockHandler = new Mock<IRequestHandler<Request, Response>>();
        private IRequestHandler<Request, Response> handler;

        private readonly Mock<IContainer> mockContainer = new Mock<IContainer>();
        private IContainer container;

        private readonly Request request = new Request();
        private readonly Response response = new Response();
        

        [SetUp]
        public void Setup()
        {
            handler = mockHandler.Object;
            container = mockContainer.Object;

            mockHandler
                .Setup(m => m.Handle(It.IsAny<Request>()))
                .Returns(response);
        }


        [TearDown]
        public void Teaddown()
        {
            mockHandler.Reset();
            mockContainer.Reset();
        }


        [Test]
        public void DIRequestDispatcher_WithNullContainer_ThrowsArgumentNullException()
        {
            // Act
            TestDelegate construct = () => new DIRequestDispatcher(null);

            // Assert
            Assert.That(construct, Throws.ArgumentNullException);
        }


        #region Register
        [Test]
        public void Register_WithRequestHandler_WithUnserializableRequestType_ThrowsUnserializableTypeException()
        {
            // Arrange
            var dispatcher = new DIRequestDispatcher(container);
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
            var dispatcher = new DIRequestDispatcher(container);
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
            var dispatcher = new DIRequestDispatcher(container);
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
            var dispatcher = new DIRequestDispatcher(container);
            RequestHandlerDelegate<Request, UnserializableResponse> handler = request => default(UnserializableResponse);

            // Act
            TestDelegate register = () => dispatcher.Register(handler);

            // Assert
            Assert.That(register, Throws.TypeOf<UnserializableTypeException>());
        }


        [Test]
        public void Register_WithRequestHandlerType_WithUnserializableRequestType_ThrowsUnserializableTypeException()
        {
            // Arrange
            var dispatcher = new DIRequestDispatcher(container);

            // Act
            TestDelegate register = () => dispatcher.Register<UnserializableRequest, Response, IRequestHandler<UnserializableRequest, Response>>();

            // Assert
            Assert.That(register, Throws.TypeOf<UnserializableTypeException>());
        }


        [Test]
        public void Register_WithRequestHandlerType_WithUnserializableResponseType_ThrowsUnserializableTypeException()
        {
            // Arrange
            var dispatcher = new DIRequestDispatcher(container);

            // Act
            TestDelegate register = () => dispatcher.Register<Request, UnserializableResponse, IRequestHandler<Request, UnserializableResponse>>();

            // Assert
            Assert.That(register, Throws.TypeOf<UnserializableTypeException>());
        }


        [Test]
        public void Register_WithRequestHandlerType_WithUnregisteredHandler_ThrowsNotRegisteredException()
        {
            // Arrange
            var dispatcher = new DIRequestDispatcher(container);

            // Act
            TestDelegate register = () => dispatcher.Register<Request, Response, IRequestHandler<Request, Response>>();

            // Assert
            Assert.That(register, Throws.TypeOf<NotRegisteredException>());
        }


        [Test]
        public void Register_WithRequestHandlerType_CallsContainerIsRegistered()
        {
            // Arrange
            var dispatcher = new DIRequestDispatcher(container);
            mockContainer
                .Setup(m => m.IsRegistered<IRequestHandler<Request, Response>>())
                .Returns(true);

            // Act
            dispatcher.Register<Request, Response, IRequestHandler<Request, Response>>();

            // Assert
            mockContainer.Verify(m => m.IsRegistered<IRequestHandler<Request, Response>>(), Times.Once);
        }
        #endregion


        #region RegisterAsync
        [Test]
        public void RegisterAsync_WithUnserializableRequestType_ThrowsUnserializableTypeException()
        {
            // Arrange
            var dispatcher = new DIRequestDispatcher(container);
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
            var dispatcher = new DIRequestDispatcher(container);
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
            var dispatcher = new DIRequestDispatcher(container);

            // Act
            TestDelegate handleNull = () => dispatcher.Handle(null);

            // Assert
            Assert.That(handleNull, Throws.ArgumentNullException);
        }


        [Test]
        public void Handle_WithNoRegisteredHandler_ThrowsRequestHandlerNotFoundException()
        {
            // Arrange
            var dispatcher = new DIRequestDispatcher(container);

            // Act
            TestDelegate handleUnregistered = () => dispatcher.Handle(request);

            // Assert
            Assert.That(handleUnregistered, Throws.TypeOf<RequestHandlerNotFoundException>());
        }


        [Test]
        public void Handle_WithHandlerRegistered_CallsHandler()
        {
            // Arrange
            var dispatcher = new DIRequestDispatcher(container);
            dispatcher.Register(handler);

            // Act
            var response = dispatcher.Handle(request);

            // Assert
            mockHandler.Verify(m => m.Handle(It.IsIn(request)), Times.Once);
        }


        [Test]
        public void Handle_WithBaseClassHandlerRegistered_ThrowsRequestHandlerNotFoundException()
        {
            // Arrange
            var dispatcher = new DIRequestDispatcher(container);
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
            var dispatcher = new DIRequestDispatcher(container);
            dispatcher.Register<Request, Response>(dt => { handled = true; return response; });

            // Act
            var ret = dispatcher.Handle(request);

            // Assert
            Assert.That(handled, Is.True);
        }


        [Test]
        public void Handle_WithBaseClassHandlerFunctionRegistered_ThrowsRequestHandlerNotFoundException()
        {
            // Arrange
            var dispatcher = new DIRequestDispatcher(container);
            dispatcher.Register<Request, Response>(r => response);
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
            var dispatcher = new DIRequestDispatcher(container);
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
            var dispatcher = new DIRequestDispatcher(container);
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
            var dispatcher = new DIRequestDispatcher(container);
            dispatcher.Register(handler);

            // Act
            var ret = dispatcher.Handle(request);

            // Assert
            Assert.That(ret is Response, Is.True);
            Assert.That(ret, Is.SameAs(response));
        }


        [Test]
        public void Handle_WithResolvableHandlerClassRegistered_ReturnsResponse()
        {
            // Arrange
            var dispatcher = new DIRequestDispatcher(container);
            mockContainer.Setup(m => m.IsRegistered<IRequestHandler<Request, Response>>()).Returns(true);
            mockContainer.Setup(m => m.Resolve<IRequestHandler<Request, Response>>()).Returns(handler);
            dispatcher.Register<Request, Response, IRequestHandler<Request, Response>>();

            // Act
            var ret = dispatcher.Handle(request);

            // Assert
            Assert.That(ret is Response, Is.True);
            Assert.That(ret, Is.SameAs(response));
        }


        [Test]
        public void Handle_WithResolvableHandlerClassRegistered_ResolvesFromContainer()
        {
            // Arrange
            var dispatcher = new DIRequestDispatcher(container);
            mockContainer.Setup(m => m.IsRegistered<IRequestHandler<Request, Response>>()).Returns(true);
            mockContainer.Setup(m => m.Resolve<IRequestHandler<Request, Response>>()).Returns(handler);
            dispatcher.Register<Request, Response, IRequestHandler<Request, Response>>();

            // Act
            var ret = dispatcher.Handle(request);

            // Assert
            mockContainer.Verify(m => m.Resolve<IRequestHandler<Request, Response>>(), Times.Once);
        }
        #endregion
    }
}
