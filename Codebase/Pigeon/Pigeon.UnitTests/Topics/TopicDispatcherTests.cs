using Pigeon.Topics;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pigeon.UnitTests.Subscripions
{
    [TestFixture]
    public class TopicDispatcherTests
    {
        public class Topic { }
        public class SubTopic : Topic { }

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


        #region Handle
        [Test]
        public void Handle_WithNullMessage_DoesNothing()
        {
            // Arrange
            var dispatcher = new TopicDispatcher();

            // Act
            AsyncTestDelegate handle = async () => await dispatcher.Handle(null);

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
            AsyncTestDelegate handle = async () => await dispatcher.Handle(message);

            // Assert
            Assert.That(handle, Throws.Nothing);
        }


        [Test]
        public async Task Handle_WithHandlerRegistered_CallsHandler()
        {
            // Arrange
            var dispatcher = new TopicDispatcher().Register(handler);
            var message = new Topic();

            // Act
            await dispatcher.Handle(message);

            // Assert
            mockHandler.Verify(m => m.Handle(It.IsIn(message)), Times.Once);
        }


        [Test]
        public async Task Handle_WithHandlerDelegateRegistered_CallsHandler()
        {
            // Arrange
            var handled = false;
            var dispatcher = new TopicDispatcher().RegisterAsync<Topic>(e => Task.Run(() => { handled = true; }));
            var message = new Topic();

            // Act
            await dispatcher.Handle(message);

            // Assert
            Assert.That(handled, Is.True);
        }


        [Test]
        public async Task Handle_WithBaseClassHandlerRegistered_DoesNothing()
        {
            // Arrange
            var dispatcher = new TopicDispatcher().Register(handler);
            var message = new SubTopic();

            // Act
            await dispatcher.Handle(message);

            // Assert
            mockHandler.Verify(m => m.Handle(It.IsAny<Topic>()), Times.Never);
        }
        #endregion
    }
}
