using System;
using System.Linq;

using Moq;

using NetMQ.Sockets;

using NUnit.Framework;

using Pigeon.Addresses;
using Pigeon.NetMQ.Publishers;

namespace Pigeon.NetMQ.UnitTests.Publishers
{
    [TestFixture]
    public class NetMQPublisherTests
    {
        private readonly Mock<INetMQMessageFactory> mockMessageFactory = new Mock<INetMQMessageFactory>();
        private INetMQMessageFactory messageFactory;

        private readonly IAddress address = TcpAddress.Wildcard(5555);
        

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
        public void NetMQPublisher_WithNullPublisherSocket_ThrowsArgumentNullException()
        {
            // Act
            TestDelegate construct = () => new NetMQPublisher(null, messageFactory);

            // Assert
            Assert.That(construct, Throws.ArgumentNullException);
        }


        [Test]
        public void NetMQPublisher_WithNullSerializer_ThrowsArgumentNullException()
        {
            // Assert
            var socket = new PublisherSocket();

            // Act
            TestDelegate construct = () => new NetMQPublisher(socket, null);

            // Assert
            Assert.That(construct, Throws.ArgumentNullException);

            // Cleanup
            socket.Dispose();
        }
        
        #endregion


        #region Addresses

        [Test]
        public void Addresses_WhenNoAddressesAdded_IsEmpty()
        {
            // Arrange
            var socket = new PublisherSocket();
            var publisher = new NetMQPublisher(socket, messageFactory);

            // Act
            var any = publisher.Addresses.Any();

            // Assert
            Assert.That(any, Is.False);

            // Cleanup
            publisher.Dispose();
        }


        [Test]
        public void Addresses_WhenDisposed_ThrowsInvalidOperationException()
        {
            // Arrange
            var socket = new PublisherSocket();
            var publisher = new NetMQPublisher(socket, messageFactory);
            var address = TcpAddress.Wildcard(5555);
            publisher.Dispose();

            // Act
            void AddAddress() => publisher.AddAddress(address);

            // Assert
            Assert.That(AddAddress, Throws.TypeOf<InvalidOperationException>());

            // Cleanup
            socket.Dispose();
        }
        
        #endregion


        #region AddAddress

        [Test]
        public void AddAddress_WithNullAddress_ThrowsArgumentNullException()
        {
            // Arrange
            var socket = new PublisherSocket();
            var publisher = new NetMQPublisher(socket, messageFactory);

            // Act
            TestDelegate addAddress = () => publisher.AddAddress(null);

            // Assert
            Assert.That(addAddress, Throws.ArgumentNullException);

            // Cleanup
            publisher.Dispose();
        }


        [Test]
        public void AddAddress_WithNewAddress_AddsToAddresses()
        {
            // Arrange
            var socket = new PublisherSocket();
            var publisher = new NetMQPublisher(socket, messageFactory);

            // Act
            publisher.AddAddress(address);

            // Assert
            CollectionAssert.Contains(publisher.Addresses, address);

            // Cleanup
            publisher.Dispose();
        }


        [Test]
        public void AddAddress_WithAlreadyAddedAddress_DoesNotAddTwice()
        {
            // Arrange
            var socket = new PublisherSocket();
            var publisher = new NetMQPublisher(socket, messageFactory);
            publisher.AddAddress(address);

            // Act
            publisher.AddAddress(address);

            // Assert
            Assert.That(publisher.Addresses, Has.Count.EqualTo(1));

            // Cleanup
            publisher.Dispose();
        }
        
        #endregion


        #region RemoveAddress

        [Test]
        public void RemoveAddress_WithNullAddress_DoesNothing()
        {
            // Arrange
            var socket = new PublisherSocket();
            var publisher = new NetMQPublisher(socket, messageFactory);

            // Act
            TestDelegate removeAddress = () => publisher.RemoveAddress(null);

            // Assert
            Assert.That(removeAddress, Throws.Nothing);

            // Cleanup
            publisher.Dispose();
        }


        [Test]
        public void RemoveAddress_WithUnaddedAddress_DoesNothing()
        {
            // Arrange
            var socket = new PublisherSocket();
            var publisher = new NetMQPublisher(socket, messageFactory);

            // Act
            TestDelegate removeAddress = () => publisher.RemoveAddress(address);

            // Assert
            Assert.That(removeAddress, Throws.Nothing);

            // Cleanup
            publisher.Dispose();
        }


        [Test]
        public void RemoveAddress_WithAddedAddress_RemovesFromAddresses()
        {
            // Arrange
            var socket = new PublisherSocket();
            var publisher = new NetMQPublisher(socket, messageFactory);
            publisher.AddAddress(address);

            // Act
            publisher.RemoveAddress(address);

            // Assert
            CollectionAssert.DoesNotContain(publisher.Addresses, address);

            // Cleanup
            publisher.Dispose();
        }


        [Test]
        public void Remove_WithAddedAddress_IsConnectedFalse()
        {
            // Arrange
            var socket = new PublisherSocket();
            var publisher = new NetMQPublisher(socket, messageFactory);
            var address = TcpAddress.Wildcard(5555);
            publisher.AddAddress(address);

            // Act
            publisher.RemoveAddress(address);

            // Assert
            Assert.That(publisher.IsConnected, Is.False);

            // Cleanup
            publisher.Dispose();
        }


        [Test]
        public void Remove_WithAddedAddress_IsConnectedTrue()
        {
            // Arrange
            var socket = new PublisherSocket();
            var publisher = new NetMQPublisher(socket, messageFactory);
            var address = TcpAddress.Wildcard(5555);
            var address2 = TcpAddress.Wildcard(5556);
            publisher.AddAddress(address);
            publisher.AddAddress(address2);
            publisher.InitializeConnection();

            // Act
            publisher.RemoveAddress(address);

            // Assert
            Assert.That(publisher.IsConnected, Is.True);

            // Cleanup
            publisher.TerminateConnection();
            publisher.Dispose();
        }

        #endregion


        #region RemoveAllAddresses

        [Test]
        public void RemoveAllAddresses_WithAddedAddresses_ClearsAddresses()
        {
            // Arrange
            var socket = new PublisherSocket();
            var publisher = new NetMQPublisher(socket, messageFactory);
            publisher.AddAddress(TcpAddress.Wildcard(5555));

            // Act
            publisher.RemoveAllAddresses();

            // Assert
            CollectionAssert.IsEmpty(publisher.Addresses);

            // Cleanup
            publisher.Dispose();
        }


        [Test]
        public void RemoveAllAddresses_WithAddedAddresses_IsConnectedFalse()
        {
            // Arrange
            var socket = new PublisherSocket();
            var publisher = new NetMQPublisher(socket, messageFactory);
            publisher.AddAddress(TcpAddress.Wildcard(5555));

            // Act
            publisher.RemoveAllAddresses();

            // Assert
            Assert.That(publisher.IsConnected, Is.False);

            // Cleanup
            publisher.Dispose();
        }

        #endregion


        #region BindAll

        [Test]
        public void BindAll_WithNoAddresses_DoesNothing()
        {
            // Arrange
            var socket = new PublisherSocket();
            var publisher = new NetMQPublisher(socket, messageFactory);

            // Act
            TestDelegate bindAll = () => publisher.InitializeConnection();

            // Assert
            Assert.That(bindAll, Throws.Nothing);

            // Cleanup
            publisher.Dispose();
        }


        [Test]
        public void BindAll_WhenAlreadyBound_DoesNothing()
        {
            // Arrange
            var socket = new PublisherSocket();
            var publisher = new NetMQPublisher(socket, messageFactory);
            publisher.InitializeConnection();

            // Act
            TestDelegate bindAll = () => publisher.InitializeConnection();

            // Assert
            Assert.That(bindAll, Throws.Nothing);

            // Cleanup
            publisher.TerminateConnection();
            publisher.Dispose();
        }
        
        #endregion
    }
}
