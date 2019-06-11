using System.Threading.Tasks;

using Moq;

using NUnit.Framework;

using Pigeon.Diagnostics;
using Pigeon.Topics;
using Pigeon.UnitTests.TestFixtures;

namespace Pigeon.UnitTests.Topics
{
    [TestFixture]
    public class DITopicDispatcherTests
    {
        private readonly Mock<ITopicHandler<Topic>> mockHandler = new Mock<ITopicHandler<Topic>>();
        private ITopicHandler<Topic> handler;

        private readonly Mock<IContainer> mockContainer = new Mock<IContainer>();
        private IContainer container;

        private readonly Topic topic = new Topic();


        [SetUp]
        public void Setup()
        {
            handler = mockHandler.Object;
            container = mockContainer.Object;
        }


        [TearDown]
        public void Teardown()
        {
            mockHandler.Reset();
            mockContainer.Reset();
        }


        [Test]
        public void DITopicDispatcher_WithNullContainer_ThrowsArgumentNullException()
        {
            // Act
            TestDelegate construct = () => new DITopicDispatcher(null);

            // Assert
            Assert.That(construct, Throws.ArgumentNullException);
        }


        #region Register
        [Test]
        public void Register_WithTopicHandler_WithUnserializableTopic_ThrowsUnserializableTypeException()
        {
            // Arrange
            var dispatcher = new DITopicDispatcher(container);
            var handler = new Mock<ITopicHandler<UnserializableTopic>>().Object;

            // Act
            TestDelegate register = () => dispatcher.Register(handler);

            // Assert
            Assert.That(register, Throws.TypeOf<UnserializableTypeException>());
        }


        [Test]
        public void Register_WithTopicHandlerDelegate_WithUnserializableTopic_ThrowsUnserializableTypeException()
        {
            // Arrange
            var dispatcher = new DITopicDispatcher(container);
            TopicHandlerDelegate<UnserializableTopic> handler = _ => { };

            // Act
            TestDelegate register = () => dispatcher.Register(handler);

            // Assert
            Assert.That(register, Throws.TypeOf<UnserializableTypeException>());
        }


        [Test]
        public void Register_WithUnresolvedTopicHandler_WithUnserializableTopic_ThrowsUnserializableTypeException()
        {
            // Arrange
            var dispatcher = new DITopicDispatcher(container);

            // Act
            TestDelegate register = () => dispatcher.Register<UnserializableTopic, ITopicHandler<UnserializableTopic>>();

            // Assert
            Assert.That(register, Throws.TypeOf<UnserializableTypeException>());
        }


        [Test]
        public void Register_WithUnresolvedTopicHandler_WithUnregisteredHandler_ThrowsNotRegisteredException()
        {
            // Arrange
            var dispatcher = new DITopicDispatcher(container);

            // Act
            TestDelegate register = () => dispatcher.Register<Topic, ITopicHandler<Topic>>();

            // Assert
            Assert.That(register, Throws.TypeOf<NotRegisteredException>());
        }


        [Test]
        public void Register_WithUnresolvedTopicHandler_CallsContainerIsRegistered()
        {
            // Arrange
            var dispatcher = new DITopicDispatcher(container);
            mockContainer
                .Setup(m => m.IsRegistered<ITopicHandler<Topic>>())
                .Returns(true);

            // Act
            dispatcher.Register<Topic, ITopicHandler<Topic>>();

            // Assert
            mockContainer.Verify(m => m.IsRegistered<ITopicHandler<Topic>>(), Times.Once);
        }
        #endregion


        #region AsyncRegister
        [Test]
        public void RegisterAsync_WithUnserializableTopic_ThrowsUnserializableTypeException()
        {
            // Arrange
            var dispatcher = new DITopicDispatcher(container);
            AsyncTopicHandlerDelegate<UnserializableTopic> handler = _ => Task.CompletedTask;

            // Act
            TestDelegate register = () => dispatcher.RegisterAsync(handler);

            // Assert
            Assert.That(register, Throws.TypeOf<UnserializableTypeException>());
        }
        #endregion


        #region Handle
        [Test]
        public void Handle_WithNull_DoesNothing()
        {
            // Arrange
            var dispatcher = new DITopicDispatcher(container);

            // Act
            AsyncTestDelegate handle = async () => await dispatcher.Handle(null);

            // Assert
            Assert.That(handle, Throws.Nothing);
        }


        [Test]
        public void Handle_WithNoHandlerRegistered_DoesNothing()
        {
            // Arrange
            var dispatcher = new DITopicDispatcher(container);

            // Act
            AsyncTestDelegate handle = async () => await dispatcher.Handle(topic);

            // Assert
            Assert.That(handle, Throws.Nothing);
        }


        [Test]
        public async Task Handle_WithHandlerRegistered_CallsHandler()
        {
            // Arrange
            var dispatcher = new DITopicDispatcher(container);
            dispatcher.Register(handler);

            // Act
            await dispatcher.Handle(topic);

            // Assert
            mockHandler.Verify(m => m.Handle(It.IsIn(topic)), Times.Once);
        }


        [Test]
        public async Task Handle_WithHandlerDelegateRegistered_CallsHandler()
        {
            // Arrange
            var handled = false;
            var dispatcher = new DITopicDispatcher(container);
            dispatcher.RegisterAsync<Topic>(e => Task.Run(() => { handled = true; }));

            // Act
            await dispatcher.Handle(topic);

            // Assert
            Assert.That(handled, Is.True);
        }


        [Test]
        public async Task Handle_WithBaseClassHandlerRegistered_DoesNothing()
        {
            // Arrange
            var dispatcher = new DITopicDispatcher(container);
            dispatcher.Register(handler);
            var topic = new SubTopic();

            // Act
            await dispatcher.Handle(topic);

            // Assert
            mockHandler.Verify(m => m.Handle(It.IsAny<Topic>()), Times.Never);
        }


        [Test]
        public async Task Handle_WithContainerResolvedHandler_HandlesTopic()
        {
            // Arrange
            var dispatcher = new DITopicDispatcher(container);
            mockContainer.Setup(m => m.IsRegistered<ITopicHandler<Topic>>()).Returns(true);
            mockContainer.Setup(m => m.Resolve<ITopicHandler<Topic>>()).Returns(handler);
            dispatcher.Register<Topic, ITopicHandler<Topic>>();

            // Act
            await dispatcher.Handle(topic);

            // Assert
            mockHandler.Verify(m => m.Handle(It.IsIn(topic)), Times.Once);
        }


        [Test]
        public async Task Handle_WithContainerResolvedHandler_ResolvesFromContainer()
        {
            // Arrange
            var dispatcher = new DITopicDispatcher(container);
            mockContainer.Setup(m => m.IsRegistered<ITopicHandler<Topic>>()).Returns(true);
            mockContainer.Setup(m => m.Resolve<ITopicHandler<Topic>>()).Returns(handler);
            dispatcher.Register<Topic, ITopicHandler<Topic>>();

            // Act
            await dispatcher.Handle(topic);

            // Assert
            mockContainer.Verify(m => m.Resolve<ITopicHandler<Topic>>(), Times.Once);
        }
        #endregion
    }
}
