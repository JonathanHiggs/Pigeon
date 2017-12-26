using MessageRouter.Addresses;
using MessageRouter.NetMQ.Publishers;
using MessageRouter.NetMQ.Receivers;
using MessageRouter.NetMQ.Senders;
using MessageRouter.NetMQ.Subscribers;
using MessageRouter.Receivers;
using MessageRouter.Senders;
using MessageRouter.Serialization;
using MessageRouter.Subscribers;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessageRouter.NetMQ.UnitTests
{
    [TestFixture]
    public class NetMQFactoryTests
    {
        private readonly Mock<INetMQMonitor> mockMonitor = new Mock<INetMQMonitor>();
        private INetMQMonitor monitor;
        
        private readonly Mock<ISerializer> mockSerializer = new Mock<ISerializer>();
        private ISerializer serializer;
        
        private readonly RequestTaskHandler requestHandler = (rec, task) => { };
        private readonly TopicEventHandler topicHandler = (sub, topic) => { };


        [SetUp]
        public void Setup()
        {
            monitor = mockMonitor.Object;
            serializer = mockSerializer.Object;

            mockMonitor
                .SetupGet(m => m.RequestHandler)
                .Returns(requestHandler);

            mockMonitor
                .SetupGet(m => m.TopicHandler)
                .Returns(topicHandler);
        }


        [TearDown]
        public void TearDown()
        {
            mockMonitor.Reset();
            mockSerializer.Reset();
        }


        #region Constructor
        [Test]
        public void NetMQFactory_WithNullMonitor_ThrowsArgumentNullException()
        {
            // Act
            TestDelegate construct = () => new NetMQFactory(null, serializer);

            // Assert
            Assert.That(construct, Throws.ArgumentNullException);
        }


        [Test]
        public void NetMQFactory_WithNullSerializer_ThrowsArgumentNullException()
        {
            // Act
            TestDelegate construct = () => new NetMQFactory(monitor, null);

            // Assert
            Assert.That(construct, Throws.ArgumentNullException);
        }
        #endregion


        [Test]
        public void SenderMonitor_ReturnsSuppliedMonitor()
        {
            // Arrange
            var factory = new NetMQFactory(monitor, serializer);

            // Act
            var senderMonitor = factory.SenderMonitor;

            // Assert
            Assert.AreSame(monitor, senderMonitor);
        }


        [Test]
        public void ReceiverMonitor_ReturnsSuppliedMonitor()
        {
            // Arrange
            var factory = new NetMQFactory(monitor, serializer);

            // Act
            var receiverMonitor = factory.ReceiverMonitor;

            // Assert
            Assert.AreSame(monitor, receiverMonitor);
        }


        [Test]
        public void PublisherMonitor_ReturnsSuppliedMonitor()
        {
            // Arrange
            var factory = new NetMQFactory(monitor, serializer);

            // Act
            var publisherMonitor = factory.PublisherMonitor;

            // Assert
            Assert.AreSame(monitor, publisherMonitor);
        }


        [Test]
        public void SubscriberMonitor_ReturnsSuppliedMonitor()
        {
            // Arrange
            var factory = new NetMQFactory(monitor, serializer);

            // Act
            var subscriberMonitor = factory.SubscriberMonitor;

            // Assert
            Assert.AreSame(monitor, subscriberMonitor);
        }


        [Test]
        public void SenderType_ReturnsINetMQSender()
        {
            // Arrange
            var factory = new NetMQFactory(monitor, serializer);

            // Act
            var senderType = factory.SenderType;

            // Assert
            Assert.AreSame(senderType, typeof(INetMQSender));
        }


        [Test]
        public void ReceiverType_ReturnsINetMQReceiver()
        {
            // Arrange
            var factory = new NetMQFactory(monitor, serializer);

            // Act
            var receiverType = factory.ReceiverType;

            // Assert
            Assert.AreSame(receiverType, typeof(INetMQReceiver));
        }


        [Test]
        public void PublisherType_ReturnsINetMQPublisher()
        {
            // Arrange
            var factory = new NetMQFactory(monitor, serializer);

            // Act
            var publisherType = factory.PublisherType;

            // Assert
            Assert.AreSame(publisherType, typeof(INetMQPublisher));
        }


        [Test]
        public void SubscriberType_ReturnsINetMQSubscriber()
        {
            // Arrange
            var factory = new NetMQFactory(monitor, serializer);

            // Act
            var subscriberType = factory.SubscriberType;

            // Assert
            Assert.AreSame(subscriberType, typeof(INetMQSubscriber));
        }


        [Test]
        public void CreateNewSender_ReturnsSender()
        {
            // Arrange
            var factory = new NetMQFactory(monitor, serializer);

            // Act
            var sender = factory.CreateSender(TcpAddress.Localhost(5555));

            // Assert
            Assert.That(sender, Is.Not.Null);
        }


        [Test]
        public void CreateNewReceiver_ReturnsReceiver()
        {
            // Arrange
            var factory = new NetMQFactory(monitor, serializer);

            // Act
            var receiver = factory.CreateReceiver(TcpAddress.Wildcard(5555));

            // Assert
            Assert.That(receiver, Is.Not.Null);
        }


        [Test]
        public void CreateNewReceiver_WithHandler_ReceiverHandlerIsSame()
        {
            // Arrange
            var factory = new NetMQFactory(monitor, serializer);

            // Act
            var receiver = factory.CreateReceiver(TcpAddress.Wildcard(5555));

            // Assert
            Assert.That(receiver.Handler, Is.SameAs(requestHandler));
        }


        [Test]
        public void CreateNewPublisher_ReturnsPublisher()
        {
            // Arrange
            var factory = new NetMQFactory(monitor, serializer);

            // Act
            var publisher = factory.CreatePublisher(TcpAddress.Wildcard(5555));

            // Assert
            Assert.That(publisher, Is.Not.Null);
        }


        [Test]
        public void CreateNewSubscriber_ReturnsSubscriber()
        {
            // Arrange
            var factory = new NetMQFactory(monitor, serializer);

            // Act
            var subscriber = factory.CreateSubscriber(TcpAddress.Localhost(5555));

            // Assert
            Assert.That(subscriber, Is.Not.Null);
        }


        [Test]
        public void CreateNewSubscriber_WithHandler_SusbcriberHandlerIsSame()
        {
            // Arrange
            var factory = new NetMQFactory(monitor, serializer);

            // Act
            var subscriber = factory.CreateSubscriber(TcpAddress.Localhost(5555));

            // Assert
            Assert.That(subscriber.Handler, Is.SameAs(topicHandler));
        }
    }
}
