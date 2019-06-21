using System.Threading.Tasks;

using Moq;

using NUnit.Framework;

using Pigeon.Diagnostics;
using Pigeon.Topics;
using Pigeon.UnitTests.TestFixtures;

namespace Pigeon.UnitTests.Subscripions
{
    [TestFixture]
    public class TopicDispatcherTests
    {
        private readonly Mock<ITopicHandler<Topic>> mockHandler = new Mock<ITopicHandler<Topic>>();
        private ITopicHandler<Topic> handler;
        

        [SetUp]
        public void Setup()
        {
            handler = mockHandler.Object;
        }


        [TearDown]
        public void Teardown()
        {
            mockHandler.Reset();
        }


        #region Register
        [Test]
        public void Register_WithTopicHandler_WithUnserializableTopic_ThrowsUnserializableTypeException()
        {
            // Arrange
            var dispatcher = new TopicDispatcher();
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
            var dispatcher = new TopicDispatcher();
            TopicHandlerDelegate<UnserializableTopic> handler = _ => { };

            // Act
            TestDelegate register = () => dispatcher.Register(handler);

            // Assert
            Assert.That(register, Throws.TypeOf<UnserializableTypeException>());
        }
        #endregion


        #region AsyncRegister
        [Test]
        public void RegisterAsync_WithUnserializableTopic_ThrowsUnserializableTypeException()
        {
            // Arrange
            var dispatcher = new TopicDispatcher();
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
            var dispatcher = new TopicDispatcher();

            // Act
            TestDelegate handle = () => dispatcher.Handle(null);

            // Assert
            Assert.That(handle, Throws.Nothing);
        }


        [Test]
        public void Handle_WithNoHandlerRegistered_DoesNothing()
        {
            // Arrange
            var dispatcher = new TopicDispatcher();
            var message = new Topic();

            // Act
            TestDelegate handle = () => dispatcher.Handle(message);

            // Assert
            Assert.That(handle, Throws.Nothing);
        }


        [Test]
        public void Handle_WithHandlerRegistered_CallsHandler()
        {
            // Arrange
            var dispatcher = new TopicDispatcher();
            dispatcher.Register(handler);
            var message = new Topic();

            // Act
            dispatcher.Handle(message);

            // Assert
            mockHandler.Verify(m => m.Handle(It.IsIn(message)), Times.Once);
        }


        [Test]
        public void Handle_WithHandlerDelegateRegistered_CallsHandler()
        {
            // Arrange
            var handled = false;
            var dispatcher = new TopicDispatcher();
            dispatcher.RegisterAsync<Topic>(e => Task.Run(() => { handled = true; }));
            var message = new Topic();

            // Act
            dispatcher.Handle(message);

            // Assert
            Assert.That(handled, Is.True);
        }


        [Test]
        public void Handle_WithBaseClassHandlerRegistered_DoesNothing()
        {
            // Arrange
            var dispatcher = new TopicDispatcher();
            dispatcher.Register(handler);
            var message = new SubTopic();

            // Act
            dispatcher.Handle(message);

            // Assert
            mockHandler.Verify(m => m.Handle(It.IsAny<Topic>()), Times.Never);
        }
        #endregion
    }
}
