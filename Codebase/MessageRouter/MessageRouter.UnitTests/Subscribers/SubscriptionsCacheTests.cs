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
    public class SubscriptionsCacheTests
    {
        public class Topic { }

        private readonly Mock<ISubscriber> mockSubscriber = new Mock<ISubscriber>();
        private ISubscriber subscriber;

        private readonly Mock<ISubscriber> mockSubscriber2 = new Mock<ISubscriber>();
        private ISubscriber subscriber2;
        

        [SetUp]
        public void Setup()
        {
            subscriber = mockSubscriber.Object;
            subscriber2 = mockSubscriber2.Object;
        }


        [TearDown]
        public void Teardown()
        {
            mockSubscriber.Reset();
            mockSubscriber2.Reset();
        }


        #region Add
        [Test]
        public void Add_WithNullSubscriber_ThrowsArgumentNullException()
        {
            // Arrange
            var cache = new SubscriptionsCache();

            // Act
            TestDelegate add = () => cache.Add<Topic>(null);

            // Assert
            Assert.That(add, Throws.ArgumentNullException);
        }


        [Test]
        public void Add_WithSubscriber_ReturnsSubscription()
        {
            // Arrange
            var cache = new SubscriptionsCache();

            // Act
            var subscription = cache.Add<Topic>(subscriber);

            // Assert
            Assert.That(subscription, Is.Not.Null);
        }


        [Test]
        public void Add_WithSubscriber_ReturnsSubscriptionWithSameTopicType()
        {
            // Arrange
            var cache = new SubscriptionsCache();

            // Act
            var subscription = cache.Add<Topic>(subscriber);

            // Assert
            Assert.That(subscription.TopicType, Is.EqualTo(typeof(Topic)));
        }


        [Test]
        public void Add_CalledTwiceWithSameSubscriber_ReturnsSameSubscriptionInstance()
        {
            // Arrange
            var cache = new SubscriptionsCache();

            // Act
            var sub1 = cache.Add<Topic>(subscriber);
            var sub2 = cache.Add<Topic>(subscriber);

            // Assert
            Assert.That(sub1, Is.SameAs(sub2));
        }


        [Test]
        public void Add_CalledTwiceWithDifferentSubscribers_ReturnsDifferentSubscriptionInstance()
        {
            // Arrange
            var cache = new SubscriptionsCache();

            // Act
            var sub1 = cache.Add<Topic>(subscriber);
            var sub2 = cache.Add<Topic>(subscriber2);

            // Assert
            Assert.That(sub1, Is.Not.SameAs(sub2));
        }


        [Test]
        public void Add_WhenSubscriptionDisposeCalled_CallsSubscriberUnsubscribe()
        {
            // Arrange
            var cache = new SubscriptionsCache();

            // Act
            cache.Add<Topic>(subscriber).Dispose();

            // Assert
            mockSubscriber.Verify(m => m.Unsubscribe<Topic>(), Times.Once);
        }
        #endregion


        #region Remove
        [Test]
        public void Remove_WithNoSubscriptionAdded_DoesNothing()
        {
            // Arrange
            var cache = new SubscriptionsCache();

            // Act
            TestDelegate remove = () => cache.Remove<Topic>(subscriber);

            // Assert
            Assert.That(remove, Throws.Nothing);
        }


        [Test]
        public void Remove_WithSubscriptionAdded_CallsSubscriberUnsubscribe()
        {
            // Arrange
            var cache = new SubscriptionsCache();
            cache.Add<Topic>(subscriber);

            // Act
            cache.Remove<Topic>(subscriber);

            // Assert
            mockSubscriber.Verify(m => m.Unsubscribe<Topic>(), Times.Once);
        }


        [Test]
        public void Remove_WithTwoSubscriptionsAdded_CallsCorrectUnsubscribe()
        {
            // Arrange
            var cache = new SubscriptionsCache();
            cache.Add<Topic>(subscriber);
            cache.Add<Topic>(subscriber2);

            // Act
            cache.Remove<Topic>(subscriber);

            // Assert
            mockSubscriber.Verify(m => m.Unsubscribe<Topic>(), Times.Once);
        }


        [Test]
        public void Remove_WithTwoSubscriptionsAdded_CallsCorrectUnsubscribe_2()
        {
            // Arrange
            var cache = new SubscriptionsCache();
            cache.Add<Topic>(subscriber);
            cache.Add<Topic>(subscriber2);

            // Act
            cache.Remove<Topic>(subscriber2);

            // Assert
            mockSubscriber2.Verify(m => m.Unsubscribe<Topic>(), Times.Once);
        }


        [Test]
        public void Remove_WhenCalledForSameSubscriberTwice_UnsubscribesOnce()
        {
            // Arrange
            var cache = new SubscriptionsCache();
            cache.Add<Topic>(subscriber);

            // Act
            cache.Remove<Topic>(subscriber);
            cache.Remove<Topic>(subscriber);

            // Assert
            mockSubscriber.Verify(m => m.Unsubscribe<Topic>(), Times.Once);
        }
        #endregion
    }
}
