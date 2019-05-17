using System;
using System.Threading.Tasks;

using Moq;

using NUnit.Framework;
using Pigeon.Diagnostics;
using Pigeon.Monitors;
using Pigeon.Publishers;
using Pigeon.Receivers;
using Pigeon.Senders;
using Pigeon.Subscribers;
using Pigeon.UnitTests.TestFixtures;

namespace Pigeon.UnitTests
{
    [TestFixture]
    public class RouterTests
    {
        private readonly Mock<ISenderCache> mockSenderCache = new Mock<ISenderCache>();
        private ISenderCache senderCache;

        private readonly Mock<IMonitorCache> mockMonitorCache = new Mock<IMonitorCache>();
        private IMonitorCache monitorCache;

        private readonly Mock<IReceiverCache> mockReceiverCache = new Mock<IReceiverCache>();
        private IReceiverCache receiverCache;

        private readonly Mock<IPublisherCache> mockPublisherCache = new Mock<IPublisherCache>();
        private IPublisherCache publisherCache;

        private readonly Mock<ISubscriberCache> mockSubscriberCache = new Mock<ISubscriberCache>();
        private ISubscriberCache subscriberCache;
        
        private readonly string name = "some name";
        private readonly Request request = new Request();
        private readonly Topic topic = new Topic();


        [SetUp]
        public void Setup()
        {
            senderCache = mockSenderCache.Object;
            monitorCache = mockMonitorCache.Object;
            receiverCache = mockReceiverCache.Object;
            publisherCache = mockPublisherCache.Object;
            subscriberCache = mockSubscriberCache.Object;
        }


        [TearDown]
        public void TearDown()
        {
            mockSenderCache.Reset();
            mockMonitorCache.Reset();
            mockReceiverCache.Reset();
            mockPublisherCache.Reset();
            mockSubscriberCache.Reset();
        }


        #region Constructor

        [Test]
        public void Router_WithNullName_ThrowsArgumentNullException()
        {
            // Act
            TestDelegate construct = () => new Router(null, senderCache, monitorCache, receiverCache, publisherCache, subscriberCache);

            // Assert
            Assert.That(construct, Throws.ArgumentNullException);
        }


        [Test]
        public void Router_WitNullSenderCache_ThrowsArgumentNullException()
        {
            // Act
            TestDelegate construct = () => new Router(name, null, monitorCache, receiverCache, publisherCache, subscriberCache);

            // Assert
            Assert.That(construct, Throws.ArgumentNullException);
        }


        [Test]
        public void Router_WithNullMonitorCache_ThrowsArgumentNullException()
        {
            // Act
            TestDelegate construct = () => new Router(name, senderCache, null, receiverCache, publisherCache, subscriberCache);

            // Assert
            Assert.That(construct, Throws.ArgumentNullException);
        }


        [Test]
        public void Router_WithNullReceiverCache_ThrowsArgumentNullException()
        {
            // Act
            TestDelegate construct = () => new Router(name, senderCache, monitorCache, null, publisherCache, subscriberCache);

            // Assert
            Assert.That(construct, Throws.ArgumentNullException);
        }


        [Test]
        public void Router_WithNullPublisherCache_ThrowsArgumentNullException()
        {
            // Act
            TestDelegate construct = () => new Router(name, senderCache, monitorCache, receiverCache, null, subscriberCache);

            // Assert
            Assert.That(construct, Throws.ArgumentNullException);
        }


        [Test]
        public void Router_WithNullSubscriberCache_ThrowsArgumentNullException()
        {
            // Act
            TestDelegate construct = () => new Router(name, senderCache, monitorCache, receiverCache, publisherCache, null);

            // Assert
            Assert.That(construct, Throws.ArgumentNullException);
        }
        
        #endregion


        #region Info

        [Test]
        public void Info_BeforeStart_HasNoStartTimestamp()
        {
            // Arrange
            var router = new Router(name, senderCache, monitorCache, receiverCache, publisherCache, subscriberCache);

            // Act
            var startTimestamp = router.Info.StartedTimestamp.HasValue;

            // Assert
            Assert.That(startTimestamp, Is.False);
        }


        [Test]
        public void Info_BeforeStart_HasNoStopTimestamp()
        {
            // Arrange
            var router = new Router(name, senderCache, monitorCache, receiverCache, publisherCache, subscriberCache);

            // Act
            var stopTimestamp = router.Info.StoppedTimestamp.HasValue;

            // Assert
            Assert.That(stopTimestamp, Is.False);
        }


        [Test]
        public void Info_BeforeStart_IsNotRunning()
        {
            // Arrange
            var router = new Router(name, senderCache, monitorCache, receiverCache, publisherCache, subscriberCache);

            // Act
            var running = router.Info.Running;

            // Assert
            Assert.That(running, Is.False);
        }
        
        #endregion


        #region Publish

        [Test]
        public void Publish_CallsPublisherCache()
        {
            // Arrange
            var router = new Router(name, senderCache, monitorCache, receiverCache, publisherCache, subscriberCache);
            var message = new Topic();

            // Act
            router.Publish(message);

            // Assert
            mockPublisherCache.Verify(m => m.Publish(It.IsIn(message)), Times.Once);
        }
        

        [Test]
        public void Publish_WithUnannotatedTopic_ThrowsMissingAttributeException()
        {
            // Arrange
            var router = new Router(name, senderCache, monitorCache, receiverCache, publisherCache, subscriberCache);
            var message = new UnannotatedTopic();

            // Act
            void Publish() => router.Publish(message);

            // Assert
            Assert.That(Publish, Throws.TypeOf<MissingAttributeException>());
        }

        #endregion


        #region Send

        [Test]
        public async Task Send_WithNoTimeout_CallsSenderCache()
        {
            // Arrange
            var router = new Router(name, senderCache, monitorCache, receiverCache, publisherCache, subscriberCache);
            mockSenderCache
                .Setup(m => m.Send<Request, Response>(It.IsAny<Request>()))
                .ReturnsAsync(new Response());

            // Act
            var response = await router.Send<Request, Response>(request);

            // Assert
            mockSenderCache
                .Verify(
                    m => m.Send<Request, Response>(It.IsIn(request)),
                    Times.Once);
        }


        [Test]
        public async Task Send_WithTimeout_CallsSenderCache()
        {
            // Arrange
            var router = new Router(name, senderCache, monitorCache, receiverCache, publisherCache, subscriberCache);
            mockSenderCache
                .Setup(m => m.Send<Request, Response>(It.IsAny<Request>()))
                .ReturnsAsync(new Response());

            // Act
            var response = await router.Send<Request, Response>(request, TimeSpan.FromMilliseconds(10));

            // Assert
            mockSenderCache
                .Verify(
                    m => m.Send<Request, Response>(It.IsIn(request), It.IsAny<TimeSpan>()), 
                    Times.Once);
        }
        

        [Test]
        public void Send_WithUnannotatedRequest_ThrowsMissingAttributeException()
        {
            // Arrange
            var router = new Router(name, senderCache, monitorCache, receiverCache, publisherCache, subscriberCache);
            var request = new UnannotatedRequest();

            // Act
            async Task Send() => await router.Send<UnannotatedRequest, Response>(request);

            // Assert
            Assert.ThrowsAsync<MissingAttributeException>(async () => await Send());
        }
        

        [Test]
        public void Send_WithUnannotatedRequest_AndTimeout_ThrowsMissingAttributeException()
        {
            // Arrange
            var router = new Router(name, senderCache, monitorCache, receiverCache, publisherCache, subscriberCache);
            var request = new UnannotatedRequest();

            // Act
            async Task Send() => await router.Send<UnannotatedRequest, Response>(request, TimeSpan.FromMilliseconds(10));

            // Assert
            Assert.ThrowsAsync<MissingAttributeException>(async () => await Send());
        }


        [Test]
        public void Send_WithMismatchingResponse_ThrowsMismatchingResponseException()
        {
            // Arrange
            var router = new Router(name, senderCache, monitorCache, receiverCache, publisherCache, subscriberCache);

            // Act
            async Task Send() => await router.Send<Request, string>(request);

            // Assert
            Assert.ThrowsAsync<MismatchingResponseTypeException>(async () => await Send());
        }


        [Test]
        public void Send_WithMismatchingResponse_AndTimeout_ThrowsMismatchingResponseException()
        {
            // Arrange
            var router = new Router(name, senderCache, monitorCache, receiverCache, publisherCache, subscriberCache);

            // Act
            async Task Send() => await router.Send<Request, string>(request, TimeSpan.FromMilliseconds(10));

            // Assert
            Assert.ThrowsAsync<MismatchingResponseTypeException>(async () => await Send());
        }

        #endregion


        #region Start

        [Test]
        public void Start_BeforeStarted_StartsMonitors()
        {
            // Arrange
            var router = new Router(name, senderCache, monitorCache, receiverCache, publisherCache, subscriberCache);

            // Act
            router.Start();

            // Assert
            mockMonitorCache.Verify(m => m.StartAllMonitors(), Times.Once);
        }


        [Test]
        public void Start_WhenAlreadyStarted_DoesNothing()
        {
            // Arrange
            var router = new Router(name, senderCache, monitorCache, receiverCache, publisherCache, subscriberCache);
            router.Start();
            mockMonitorCache.Invocations.Clear();

            // Act
            router.Start();

            // Assert
            mockMonitorCache.Verify(m => m.StartAllMonitors(), Times.Never);
        }


        [Test]
        public void Start_BeforeStarted_SetsStartedTimestamp()
        {
            // Arrange
            var router = new Router(name, senderCache, monitorCache, receiverCache, publisherCache, subscriberCache);

            // Act
            router.Start();

            // Assert
            Assert.That(router.Info.StartedTimestamp.HasValue, Is.True);
        }


        [Test]
        public void Start_WhenAlreadyStarted_DoesNotChangeStartedTimestamp()
        {
            // Arrange
            var router = new Router(name, senderCache, monitorCache, receiverCache, publisherCache, subscriberCache);
            router.Start();
            var startedTimestamp = router.Info.StartedTimestamp.Value;

            // Act
            router.Start();

            // Assert
            Assert.That(router.Info.StartedTimestamp.Value, Is.EqualTo(startedTimestamp));
        }
        
        #endregion


        #region Stop

        [Test]
        public void Stop_BeforeStarted_DoesNothing()
        {
            // Arrange
            var router = new Router(name, senderCache, monitorCache, receiverCache, publisherCache, subscriberCache);

            // Act
            router.Stop();

            // Assert
            mockMonitorCache.Verify(m => m.StopAllMonitors(), Times.Never);
        }


        [Test]
        public void Stop_WhenRunning_StopsMonitors()
        {
            // Arrange
            var router = new Router(name, senderCache, monitorCache, receiverCache, publisherCache, subscriberCache);
            router.Start();

            // Act
            router.Stop();

            // Assert
            mockMonitorCache.Verify(m => m.StopAllMonitors(), Times.Once);
        }


        [Test]
        public void Stop_AfterAlreadyStopped_DoesNothing()
        {
            // Arrange
            var router = new Router(name, senderCache, monitorCache, receiverCache, publisherCache, subscriberCache);
            router.Start();
            router.Stop();
            mockMonitorCache.Invocations.Clear();

            // Act
            router.Stop();

            // Assert
            mockMonitorCache.Verify(m => m.StopAllMonitors(), Times.Never);
        }


        [Test]
        public void Stop_BeforeStarted_DoesNotSetStopTimestamp()
        {
            // Arrange
            var router = new Router(name, senderCache, monitorCache, receiverCache, publisherCache, subscriberCache);

            // Act
            router.Stop();

            // Assert
            Assert.That(router.Info.StoppedTimestamp.HasValue, Is.False);
        }


        [Test]
        public void Stop_AfterRunning_SetsStoppedTimestamp()
        {
            // Arrange
            var router = new Router(name, senderCache, monitorCache, receiverCache, publisherCache, subscriberCache);
            router.Start();

            // Act
            router.Stop();

            // Assert
            Assert.That(router.Info.StoppedTimestamp.HasValue, Is.True);
        }

        #endregion


        #region Subscribe

        [Test]
        public void Subscribe_CallsSubscriberCache()
        {
            // Arrange
            var router = new Router(name, senderCache, monitorCache, receiverCache, publisherCache, subscriberCache);

            // Act
            var subscription = router.Subscribe<Topic>();

            // Assert
            mockSubscriberCache
                .Verify(
                    m => m.Subscribe<Topic>(),
                    Times.Once);
        }


        [Test]
        public void Subscribe_WithUnannotatedTopic_ThrowsMissingAttributeException()
        {
            // Arrange
            var router = new Router(name, senderCache, monitorCache, receiverCache, publisherCache, subscriberCache);

            // Act
            void Subscribe() => router.Subscribe<UnannotatedTopic>();

            // Assert
            Assert.That(Subscribe, Throws.TypeOf<MissingAttributeException>());
        }

        #endregion


        #region Unsubscribe

        [Test]
        public void Unsubscribe_CallsSubscriberCache()
        {
            // Arrange
            var router = new Router(name, senderCache, monitorCache, receiverCache, publisherCache, subscriberCache);

            // Act
            router.Unsubscribe<Topic>();

            // Assert
            mockSubscriberCache
                .Verify(
                    m => m.Unsubscribe<Topic>(),
                    Times.Once);
        }

        #endregion
    }
}
