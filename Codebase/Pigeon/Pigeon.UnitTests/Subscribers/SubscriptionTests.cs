using Moq;

using NUnit.Framework;

using Pigeon.Subscribers;
using Pigeon.UnitTests.TestFixtures;

namespace Pigeon.UnitTests.Subscribers
{
    [TestFixture]
    public class SubscriptionTests
    {
        private readonly Mock<ISubscriber> mockSubscriber = new Mock<ISubscriber>();
        private ISubscriber subscriber;

        private string subject = "1";


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
            TestDelegate construct = () => new Subscription(null, typeof(Topic), subject, () => { });

            // Assert
            Assert.That(construct, Throws.ArgumentNullException);
        }


        [Test]
        public void Subscription_WithNullTopicType_ThrowsArgumentNullException()
        {
            // Act
            TestDelegate construct = () => new Subscription(subscriber, null, subject, () => { });

            // Assert
            Assert.That(construct, Throws.ArgumentNullException);
        }


        [Test]
        public void Subscription_WithNullOnSubscribe_ThrowsArgumentNullException()
        {
            // Act
            TestDelegate construct = () => new Subscription(subscriber, typeof(Topic), subject, null);

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
            var subscription = new Subscription(subscriber, topicType, subject, () => { });

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
            var subscription = new Subscription(subscriber, typeof(Topic), subject, () => { called = true; });

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
            var subscription = new Subscription(subscriber, typeof(Topic), subject, () => { times++; });

            // Act
            subscription.Dispose();
            subscription.Dispose();

            // Assert
            Assert.That(times, Is.EqualTo(1));
        }

        #endregion
    }
}
