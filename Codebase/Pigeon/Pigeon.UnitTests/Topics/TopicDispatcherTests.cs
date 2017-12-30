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
    public class SubscriptionEventDispatcherTests
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
            var dispatcher = TopicDispatcher.Create();

            // Act
            TestDelegate handle = () => dispatcher.Handle(null);

            // Assert
            Assert.That(handle, Throws.Nothing);
        }


        [Test]
        public void Handle_WithNoHandlerRegistered_DoesNothing()
        {
            // Arrange
            var dispatcher = TopicDispatcher.Create();
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
            var dispatcher = TopicDispatcher.Create().Register(handler);
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
            var dispatcher = TopicDispatcher.Create().Register<Topic>(e => { handled = true; });
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
            var dispatcher = TopicDispatcher.Create().Register(handler);
            var message = new SubTopic();

            // Act
            TestDelegate handle = () => dispatcher.Handle(message);

            // Assert
            Assert.That(handle, Throws.Nothing);
            mockHandler.Verify(m => m.Handle(It.IsAny<Topic>()), Times.Never);
        }
        #endregion
    }
}
