using MessageRouter.Subscribers;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessageRouter.UnitTests.Subscribers
{
    [TestFixture]
    public class SubscriptionTests
    {
        public class Topic { }

        private readonly Mock<ISubscriber> mockSubscriber = new Mock<ISubscriber>();
        private ISubscriber subscriber;


        [SetUp]
        public void Setup()
        {
            subscriber = mockSubscriber.Object;
        }


        [TearDown]
        public void Teardown()
        {
            mockSubscriber.Reset();
        }


        #region Constrctor
        [Test]
        public void Subscription_WithNullSubscriber_ThrowsArgumentNullException()
        {
            // Act
            TestDelegate construct = () => new Subscription(null, typeof(Topic), () => { });

            // Assert
            Assert.That(construct, Throws.ArgumentNullException);
        }


        [Test]
        public void Subscription_WithNullTopicType_ThrowsArgumentNullException()
        {
            // Act
            TestDelegate construct = () => new Subscription(subscriber, null, () => { });

            // Assert
            Assert.That(construct, Throws.ArgumentNullException);
        }


        [Test]
        public void Subscription_WithNullOnSubscribe_ThrowsArgumentNullException()
        {
            // Act
            TestDelegate construct = () => new Subscription(subscriber, typeof(Topic), null);

            // Assert
            Assert.That(construct, Throws.ArgumentNullException);
        }
        #endregion


        #region TopicType
        [Test]
        public void TopicType_WithTopicSupplied_ReturnsSameType()
        {
            // Arrange
            var topicType = typeof(Topic);
            var subscription = new Subscription(subscriber, topicType, () => { });

            // Act
            var topicTypeProperty = subscription.TopicType;

            // Assert
            Assert.That(topicTypeProperty, Is.EqualTo(topicType));
        }
        #endregion


        #region Dispose
        [Test]
        public void Dispose_WhenCalled_InvokesOnUnsubscribeAction()
        {
            // Arrange
            var called = false;
            var subscription = new Subscription(subscriber, typeof(Topic), () => { called = true; });

            // Act
            subscription.Dispose();

            // Assert
            Assert.That(called, Is.True);
        }


        [Test]
        public void Dispose_WhenCalledTwice_InvokesOnUnsubscribeActionOnce()
        {
            // Arrange
            var times = 0;
            var subscription = new Subscription(subscriber, typeof(Topic), () => { times++; });

            // Act
            subscription.Dispose();
            subscription.Dispose();

            // Assert
            Assert.That(times, Is.EqualTo(1));
        }
        #endregion
    }
}
