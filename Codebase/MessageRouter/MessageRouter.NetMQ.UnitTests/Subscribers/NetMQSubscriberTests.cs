using MessageRouter.Addresses;
using MessageRouter.NetMQ.Subscribers;
using MessageRouter.Serialization;
using MessageRouter.Subscribers;
using Moq;
using NetMQ.Sockets;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessageRouter.NetMQ.UnitTests.Subscribers
{
    [TestFixture]
    public class NetMQSubscriberTests
    {
        private readonly Mock<IMessageFactory> mockMessageFactory = new Mock<IMessageFactory>();
        private IMessageFactory messageFactory;

        private readonly IAddress address = TcpAddress.Localhost(5555);

        private readonly TopicEventHandler handler = (sub, task) => { };


        [SetUp]
        public void Setup()
        {
            messageFactory = mockMessageFactory.Object;
        }


        [TearDown]
        public void Teardown()
        {
            mockMessageFactory.Reset();
        }


        #region Constructor
        [Test]
        public void NetMQSubscriber_WithNullSubscriberSocket_ThrowsArgumentNullException()
        {
            // Act
            TestDelegate construct = () => new NetMQSubscriber(null, messageFactory, handler);

            // Assert
            Assert.That(construct, Throws.ArgumentNullException);
        }


        [Test]
        public void NetMQSubscriber_WithNullSerializer_ThrowsArgumentNullException()
        {
            // Arrange
            var socket = new SubscriberSocket();

            // Act
            TestDelegate construct = () => new NetMQSubscriber(socket, null, handler);

            // Assert
            Assert.That(construct, Throws.ArgumentNullException);
        }


        [Test]
        public void NetMQSubscriber_WithNullHandler_ThrowsArgumentNullException()
        {
            // Arrange
            var socket = new SubscriberSocket();

            // Act
            TestDelegate construct = () => new NetMQSubscriber(socket, messageFactory, null);

            // Assert
            Assert.That(construct, Throws.ArgumentNullException);
        }


        [Test]
        public void NetMQSubscriber_WithHandler_SubscriberHandlerPropertyReturnsSameHandler()
        {
            // Arrange
            var socket = new SubscriberSocket();

            // Act
            var subscriber = new NetMQSubscriber(socket, messageFactory, handler);

            // Assert
            Assert.That(subscriber.Handler, Is.SameAs(handler));
        }
        #endregion


        #region Addresses
        [Test]
        public void Addresses_WithNoAddressesAdded_IsEmpty()
        {
            // Arrange
            var socket = new SubscriberSocket();
            var subscriber = new NetMQSubscriber(socket, messageFactory, handler);

            // Act
            var any = subscriber.Addresses.Any();

            // Assert
            Assert.That(any, Is.False);
        }
        #endregion


        #region AddAddress
        [Test]
        public void AddAddress_WithNullAddress_ThrowsArgumentNullException()
        {
            // Arrange
            var socket = new SubscriberSocket();
            var subscriber = new NetMQSubscriber(socket, messageFactory, handler);

            // Act
            TestDelegate addAddress = () => subscriber.AddAddress(null);

            // Assert
            Assert.That(addAddress, Throws.ArgumentNullException);
        }


        [Test]
        public void AddAddress_WithNewAddress_AddsToAddresses()
        {
            // Arrange
            var socket = new SubscriberSocket();
            var subscriber = new NetMQSubscriber(socket, messageFactory, handler);

            // Act
            subscriber.AddAddress(address);

            // Assert
            CollectionAssert.Contains(subscriber.Addresses, address);
        }


        [Test]
        public void AddAddress_WithAlreadyAddedAddress_DoesNotAddTwice()
        {
            // Arrange
            var socket = new SubscriberSocket();
            var subscriber = new NetMQSubscriber(socket, messageFactory, handler);

            // Act
            subscriber.AddAddress(address);
            subscriber.AddAddress(address);

            // Assert
            Assert.That(subscriber.Addresses, Has.Count.EqualTo(1));
        }
        #endregion


        #region RemoveAddress
        [Test]
        public void RemoveAddress_WithNullAddress_DoesNothing()
        {
            // Arrange
            var socket = new SubscriberSocket();
            var subscriber = new NetMQSubscriber(socket, messageFactory, handler);

            // Act
            TestDelegate removeAddress = () => subscriber.RemoveAddress(null);

            // Assert
            Assert.That(removeAddress, Throws.Nothing);
        }


        [Test]
        public void RemoveAddress_WithUnaddedAddress_DoesNothing()
        {
            // Arrange
            var socket = new SubscriberSocket();
            var subscriber = new NetMQSubscriber(socket, messageFactory, handler);

            // Act
            TestDelegate removeAddress = () => subscriber.RemoveAddress(address);

            // Assert
            Assert.That(removeAddress, Throws.Nothing);
        }


        [Test]
        public void RemoveAddress_WithAddedAddress_RemovesFromAddresses()
        {
            // Arrange
            var socket = new SubscriberSocket();
            var subscriber = new NetMQSubscriber(socket, messageFactory, handler);
            subscriber.AddAddress(address);

            // Act
            subscriber.RemoveAddress(address);

            // Assert
            CollectionAssert.DoesNotContain(subscriber.Addresses, address);
        }
        #endregion


        #region ConnectAll
        [Test]
        public void ConnectAll_WithNoAddresses_DoesNothing()
        {
            // Arrange
            var socket = new SubscriberSocket();
            var subscriber = new NetMQSubscriber(socket, messageFactory, handler);

            // Act
            TestDelegate connectAll = () => subscriber.InitializeConnection();

            // Assert
            Assert.That(connectAll, Throws.Nothing);
        }


        [Test]
        public void ConnectAll_WhenAlreadyConnected_DoesNothing()
        {
            // Arrange
            var socket = new SubscriberSocket();
            var subscriber = new NetMQSubscriber(socket, messageFactory, handler);
            subscriber.InitializeConnection();

            // Act
            TestDelegate connectAll = () => subscriber.InitializeConnection();

            // Assert
            Assert.That(connectAll, Throws.Nothing);
        }
        #endregion
    }
}
