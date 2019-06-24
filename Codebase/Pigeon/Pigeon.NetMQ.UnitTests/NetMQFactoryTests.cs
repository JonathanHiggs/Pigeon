using Moq;

using NUnit.Framework;

using Pigeon.Addresses;
using Pigeon.NetMQ.Publishers;
using Pigeon.NetMQ.Receivers;
using Pigeon.NetMQ.Senders;
using Pigeon.NetMQ.Subscribers;
using Pigeon.Receivers;
using Pigeon.Requests;
using Pigeon.Subscribers;
using Pigeon.Topics;

namespace Pigeon.NetMQ.UnitTests
{
    [TestFixture]
    public class NetMQFactoryTests
    {
        private readonly Mock<IRequestDispatcher> mockRequestDispatcher = new Mock<IRequestDispatcher>();
        private IRequestDispatcher requestDispatcher;

        private readonly Mock<ITopicDispatcher> mockTopicDispatcher = new Mock<ITopicDispatcher>();
        private ITopicDispatcher topicDispatcher;

        private readonly Mock<INetMQMonitor> mockMonitor = new Mock<INetMQMonitor>();
        private INetMQMonitor monitor;

        private readonly Mock<INetMQMessageFactory> mockMessageFactory = new Mock<INetMQMessageFactory>();
        private INetMQMessageFactory messageFactory;


        private void RequestHandler(IReceiver rec, ref RequestTask task)
        { }


        [SetUp]
        public void Setup()
        {
            requestDispatcher = mockRequestDispatcher.Object;
            topicDispatcher = mockTopicDispatcher.Object;
            monitor = mockMonitor.Object;
            messageFactory = mockMessageFactory.Object;
        }


        [TearDown]
        public void TearDown()
        {
            mockRequestDispatcher.Reset();
            mockTopicDispatcher.Reset();
            mockMonitor.Reset();
            mockMessageFactory.Reset();
        }


        #region Constructor

        [Test]
        public void NetMQFactory_WithNullRequestDispatcher_ThrowsArgumentNullException()
        {
            // Act
            TestDelegate construct = () => new NetMQFactory(null, topicDispatcher, monitor, messageFactory);

            // Assert
            Assert.That(construct, Throws.ArgumentNullException);
        }


        [Test]
        public void NetMQFactory_WithNullTopicDispatcher_ThrowsArgumentNullException()
        {
            // Act
            TestDelegate construct = () => new NetMQFactory(requestDispatcher, null, monitor, messageFactory);

            // Assert
            Assert.That(construct, Throws.ArgumentNullException);
        }

        
        [Test]
        public void NetMQFactory_WithNullMonitor_ThrowsArgumentNullException()
        {
            // Act
            TestDelegate construct = () => new NetMQFactory(requestDispatcher, topicDispatcher, null, messageFactory);

            // Assert
            Assert.That(construct, Throws.ArgumentNullException);
        }


        [Test]
        public void NetMQFactory_WithNullSerializer_ThrowsArgumentNullException()
        {
            // Act
            TestDelegate construct = () => new NetMQFactory(requestDispatcher, topicDispatcher, monitor, null);

            // Assert
            Assert.That(construct, Throws.ArgumentNullException);
        }
        
        #endregion


        [Test]
        public void SenderMonitor_ReturnsSuppliedMonitor()
        {
            // Arrange
            var factory = new NetMQFactory(requestDispatcher, topicDispatcher, monitor, messageFactory);

            // Act
            var senderMonitor = factory.SenderMonitor;

            // Assert
            Assert.AreSame(monitor, senderMonitor);
        }


        [Test]
        public void ReceiverMonitor_ReturnsSuppliedMonitor()
        {
            // Arrange
            var factory = new NetMQFactory(requestDispatcher, topicDispatcher, monitor, messageFactory);

            // Act
            var receiverMonitor = factory.ReceiverMonitor;

            // Assert
            Assert.AreSame(monitor, receiverMonitor);
        }


        [Test]
        public void PublisherMonitor_ReturnsSuppliedMonitor()
        {
            // Arrange
            var factory = new NetMQFactory(requestDispatcher, topicDispatcher, monitor, messageFactory);

            // Act
            var publisherMonitor = factory.PublisherMonitor;

            // Assert
            Assert.AreSame(monitor, publisherMonitor);
        }


        [Test]
        public void SubscriberMonitor_ReturnsSuppliedMonitor()
        {
            // Arrange
            var factory = new NetMQFactory(requestDispatcher, topicDispatcher, monitor, messageFactory);

            // Act
            var subscriberMonitor = factory.SubscriberMonitor;

            // Assert
            Assert.AreSame(monitor, subscriberMonitor);
        }


        [Test]
        public void SenderType_ReturnsINetMQSender()
        {
            // Arrange
            var factory = new NetMQFactory(requestDispatcher, topicDispatcher, monitor, messageFactory);

            // Act
            var senderType = factory.SenderType;

            // Assert
            Assert.AreSame(senderType, typeof(INetMQSender));
        }


        [Test]
        public void ReceiverType_ReturnsINetMQReceiver()
        {
            // Arrange
            var factory = new NetMQFactory(requestDispatcher, topicDispatcher, monitor, messageFactory);

            // Act
            var receiverType = factory.ReceiverType;

            // Assert
            Assert.AreSame(receiverType, typeof(INetMQReceiver));
        }


        [Test]
        public void PublisherType_ReturnsINetMQPublisher()
        {
            // Arrange
            var factory = new NetMQFactory(requestDispatcher, topicDispatcher, monitor, messageFactory);

            // Act
            var publisherType = factory.PublisherType;

            // Assert
            Assert.AreSame(publisherType, typeof(INetMQPublisher));
        }


        [Test]
        public void SubscriberType_ReturnsINetMQSubscriber()
        {
            // Arrange
            var factory = new NetMQFactory(requestDispatcher, topicDispatcher, monitor, messageFactory);

            // Act
            var subscriberType = factory.SubscriberType;

            // Assert
            Assert.AreSame(subscriberType, typeof(INetMQSubscriber));
        }


        [Test]
        public void CreateNewSender_ReturnsSender()
        {
            // Arrange
            var factory = new NetMQFactory(requestDispatcher, topicDispatcher, monitor, messageFactory);

            // Act
            var sender = factory.CreateSender(TcpAddress.Localhost(5555));

            // Assert
            Assert.That(sender, Is.Not.Null);
        }


        [Test]
        public void CreateNewReceiver_ReturnsReceiver()
        {
            // Arrange
            var factory = new NetMQFactory(requestDispatcher, topicDispatcher, monitor, messageFactory);

            // Act
            var receiver = factory.CreateReceiver(TcpAddress.Wildcard(5555));

            // Assert
            Assert.That(receiver, Is.Not.Null);
        }


        [Test]
        public void CreateNewPublisher_ReturnsPublisher()
        {
            // Arrange
            var factory = new NetMQFactory(requestDispatcher, topicDispatcher, monitor, messageFactory);

            // Act
            var publisher = factory.CreatePublisher(TcpAddress.Wildcard(5555));

            // Assert
            Assert.That(publisher, Is.Not.Null);
        }


        [Test]
        public void CreateNewSubscriber_ReturnsSubscriber()
        {
            // Arrange
            var factory = new NetMQFactory(requestDispatcher, topicDispatcher, monitor, messageFactory);

            // Act
            var subscriber = factory.CreateSubscriber(TcpAddress.Localhost(5555));

            // Assert
            Assert.That(subscriber, Is.Not.Null);
        }
    }
}
