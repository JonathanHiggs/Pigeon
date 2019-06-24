using System;
using System.Linq;

using Moq;

using NetMQ.Sockets;

using NUnit.Framework;

using Pigeon.Addresses;
using Pigeon.NetMQ.Subscribers;
using Pigeon.Subscribers;
using Pigeon.Topics;

namespace Pigeon.NetMQ.UnitTests.Subscribers
{
    [TestFixture]
    public class NetMQSubscriberTests
    {
        private readonly Mock<ITopicDispatcher> mockTopicDispatcher = new Mock<ITopicDispatcher>();
        private ITopicDispatcher topicDispatcher;

        private readonly Mock<INetMQMessageFactory> mockMessageFactory = new Mock<INetMQMessageFactory>();
        private INetMQMessageFactory messageFactory;

        private readonly IAddress address = TcpAddress.Localhost(5555);

        private readonly TopicEventHandler handler = (sub, task) => { };


        [SetUp]
        public void Setup()
        {
            topicDispatcher = mockTopicDispatcher.Object;
            messageFactory = mockMessageFactory.Object;
        }


        [TearDown]
        public void Teardown()
        {
            mockTopicDispatcher.Reset();
            mockMessageFactory.Reset();
        }


        #region Constructor

        [Test]
        public void NetMQSubscriber_WithNullSubscriberSocket_ThrowsArgumentNullException()
        {
            // Act
            TestDelegate construct = () => new NetMQSubscriber(null, messageFactory, topicDispatcher);

            // Assert
            Assert.That(construct, Throws.ArgumentNullException);
        }


        [Test]
        public void NetMQSubscriber_WithNullSerializer_ThrowsArgumentNullException()
        {
            // Arrange
            var socket = new SubscriberSocket();

            // Act
            TestDelegate construct = () => new NetMQSubscriber(socket, null, topicDispatcher);

            // Assert
            Assert.That(construct, Throws.ArgumentNullException);

            // Cleanup
            socket.Dispose();
        }


        [Test]
        public void NetMQSubscriber_WithNullTopicDispatcher_ThrowsArgumentNullException()
        {
            // Arrange
            var socket = new SubscriberSocket();

            // Act
            TestDelegate construct = () => new NetMQSubscriber(socket, messageFactory, null);

            // Assert
            Assert.That(construct, Throws.ArgumentNullException);

            // Cleanup
            socket.Dispose();
        }

        #endregion


        #region Addresses

        [Test]
        public void Addresses_WithNoAddressesAdded_IsEmpty()
        {
            // Arrange
            var socket = new SubscriberSocket();
            var subscriber = new NetMQSubscriber(socket, messageFactory, topicDispatcher);

            // Act
            var any = subscriber.Addresses.Any();

            // Assert
            Assert.That(any, Is.False);

            // Cleanup
            subscriber.Dispose();
        }


        [Test]
        public void Addresses_WhenDisposed_ThrowsInvalidOperationException()
        {
            // Arrange
            var socket = new SubscriberSocket();
            var subscriber = new NetMQSubscriber(socket, messageFactory, topicDispatcher);
            var address = TcpAddress.Wildcard(5555);
            subscriber.Dispose();

            // Act
            void AddAddress() => subscriber.AddAddress(address);

            // Assert
            Assert.That(AddAddress, Throws.TypeOf<InvalidOperationException>());

            // Cleanup
            subscriber.Dispose();
        }

        #endregion


        #region AddAddress

        [Test]
        public void AddAddress_WithNullAddress_ThrowsArgumentNullException()
        {
            // Arrange
            var socket = new SubscriberSocket();
            var subscriber = new NetMQSubscriber(socket, messageFactory, topicDispatcher);

            // Act
            TestDelegate addAddress = () => subscriber.AddAddress(null);

            // Assert
            Assert.That(addAddress, Throws.ArgumentNullException);

            // Cleanup
            subscriber.Dispose();
        }


        [Test]
        public void AddAddress_WithNewAddress_AddsToAddresses()
        {
            // Arrange
            var socket = new SubscriberSocket();
            var subscriber = new NetMQSubscriber(socket, messageFactory, topicDispatcher);

            // Act
            subscriber.AddAddress(address);

            // Assert
            CollectionAssert.Contains(subscriber.Addresses, address);

            // Cleanup
            subscriber.Dispose();
        }


        [Test]
        public void AddAddress_WithAlreadyAddedAddress_DoesNotAddTwice()
        {
            // Arrange
            var socket = new SubscriberSocket();
            var subscriber = new NetMQSubscriber(socket, messageFactory, topicDispatcher);

            // Act
            subscriber.AddAddress(address);
            subscriber.AddAddress(address);

            // Assert
            Assert.That(subscriber.Addresses, Has.Count.EqualTo(1));

            // Cleanup
            subscriber.Dispose();
        }
        
        #endregion


        #region RemoveAddress

        [Test]
        public void RemoveAddress_WithNullAddress_DoesNothing()
        {
            // Arrange
            var socket = new SubscriberSocket();
            var subscriber = new NetMQSubscriber(socket, messageFactory, topicDispatcher);

            // Act
            TestDelegate removeAddress = () => subscriber.RemoveAddress(null);

            // Assert
            Assert.That(removeAddress, Throws.Nothing);

            // Cleanup
            subscriber.Dispose();
        }


        [Test]
        public void RemoveAddress_WithUnaddedAddress_DoesNothing()
        {
            // Arrange
            var socket = new SubscriberSocket();
            var subscriber = new NetMQSubscriber(socket, messageFactory, topicDispatcher);

            // Act
            TestDelegate removeAddress = () => subscriber.RemoveAddress(address);

            // Assert
            Assert.That(removeAddress, Throws.Nothing);

            // Cleanup
            subscriber.Dispose();
        }


        [Test]
        public void RemoveAddress_WithAddedAddress_RemovesFromAddresses()
        {
            // Arrange
            var socket = new SubscriberSocket();
            var subscriber = new NetMQSubscriber(socket, messageFactory, topicDispatcher);
            subscriber.AddAddress(address);

            // Act
            subscriber.RemoveAddress(address);

            // Assert
            CollectionAssert.DoesNotContain(subscriber.Addresses, address);

            // Cleanup
            subscriber.Dispose();
        }


        [Test]
        public void Remove_WithAddedAddress_IsConnectedFalse()
        {
            // Arrange
            var socket = new SubscriberSocket();
            var subscriber = new NetMQSubscriber(socket, messageFactory, topicDispatcher);
            var address = TcpAddress.Wildcard(5555);
            subscriber.AddAddress(address);

            // Act
            subscriber.RemoveAddress(address);

            // Assert
            Assert.That(subscriber.IsConnected, Is.False);

            // Cleanup
            subscriber.Dispose();
        }


        [Test]
        public void Remove_WithAddedAddress_IsConnectedTrue()
        {
            // Arrange
            var socket = new SubscriberSocket();
            var subscriber = new NetMQSubscriber(socket, messageFactory, topicDispatcher);
            var address = TcpAddress.Wildcard(5555);
            var address2 = TcpAddress.Wildcard(5556);
            subscriber.AddAddress(address);
            subscriber.AddAddress(address2);
            subscriber.InitializeConnection();

            // Act
            subscriber.RemoveAddress(address);

            // Assert
            Assert.That(subscriber.IsConnected, Is.True);

            // Cleanup
            subscriber.TerminateConnection();
            subscriber.Dispose();
        }

        #endregion


        #region RemoveAllAddresses

        [Test]
        public void RemoveAllAddresses_WithAddedAddresses_ClearsAddresses()
        {
            // Arrange
            var socket = new SubscriberSocket();
            var subscriber = new NetMQSubscriber(socket, messageFactory, topicDispatcher);
            subscriber.AddAddress(TcpAddress.Wildcard(5555));

            // Act
            subscriber.RemoveAllAddresses();

            // Assert
            CollectionAssert.IsEmpty(subscriber.Addresses);

            // Cleanup
            subscriber.Dispose();
        }


        [Test]
        public void RemoveAllAddresses_WithAddedAddresses_IsConnectedFalse()
        {
            // Arrange
            var socket = new SubscriberSocket();
            var subscriber = new NetMQSubscriber(socket, messageFactory, topicDispatcher);
            subscriber.AddAddress(TcpAddress.Wildcard(5555));

            // Act
            subscriber.RemoveAllAddresses();

            // Assert
            Assert.That(subscriber.IsConnected, Is.False);

            // Cleanup
            subscriber.Dispose();
        }

        #endregion


        #region ConnectAll

        [Test]
        public void ConnectAll_WithNoAddresses_DoesNothing()
        {
            // Arrange
            var socket = new SubscriberSocket();
            var subscriber = new NetMQSubscriber(socket, messageFactory, topicDispatcher);

            // Act
            TestDelegate connectAll = () => subscriber.InitializeConnection();

            // Assert
            Assert.That(connectAll, Throws.Nothing);

            // Cleanup
            subscriber.Dispose();
        }


        [Test]
        public void ConnectAll_WhenAlreadyConnected_DoesNothing()
        {
            // Arrange
            var socket = new SubscriberSocket();
            var subscriber = new NetMQSubscriber(socket, messageFactory, topicDispatcher);
            subscriber.InitializeConnection();

            // Act
            TestDelegate connectAll = () => subscriber.InitializeConnection();

            // Assert
            Assert.That(connectAll, Throws.Nothing);

            // Cleanup
            subscriber.TerminateConnection();
            subscriber.Dispose();
        }
        
        #endregion
    }
}
