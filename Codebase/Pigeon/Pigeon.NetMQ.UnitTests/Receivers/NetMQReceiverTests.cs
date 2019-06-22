using System;
using System.Linq;

using Moq;

using NetMQ.Sockets;

using NUnit.Framework;

using Pigeon.Addresses;
using Pigeon.NetMQ.Receivers;
using Pigeon.Receivers;

namespace Pigeon.NetMQ.UnitTests.Receivers
{
    [TestFixture]
    public class NetMQReceiverTests
    {
        private readonly Mock<INetMQMessageFactory> mockMessageFactory = new Mock<INetMQMessageFactory>();
        private INetMQMessageFactory messageFactory;

        private void Handler(ref RequestTask task)
        { }


        [SetUp]
        public void Setup()
        {
            messageFactory = mockMessageFactory.Object;
        }


        #region Constructor

        [Test]
        public void NetMQReceiver_WithNullRouterSocket_ThrowsArgumentNullException()
        {
            // Act
            TestDelegate test = () => new NetMQReceiver(null, messageFactory, Handler);

            // Assert
            Assert.That(test, Throws.ArgumentNullException);
        }


        [Test]
        public void NetMQReceiver_WithNullSerializer_ThrowsArgumentNullException()
        {
            // Arrange
            var socket = new RouterSocket();

            // Act
            TestDelegate test = () => new NetMQReceiver(socket, null, Handler);

            // Assert
            Assert.That(test, Throws.ArgumentNullException);

            // Cleanup
            socket.Dispose();
        }


        [Test]
        public void NetMQReceiver_WithNullHandler_ThrowsArgumentNullException()
        {
            // Arrange
            var socket = new RouterSocket();

            // Act
            TestDelegate construct = () => new NetMQReceiver(socket, messageFactory, null);

            // Assert
            Assert.That(construct, Throws.ArgumentNullException);

            // Cleanup
            socket.Dispose();
        }
                
        #endregion


        #region IsConnected

        [Test]
        public void IsConnected_BeforeBindIsCalled_IsFalse()
        {
            // Arrange
            var socket = new RouterSocket();
            var receiver = new NetMQReceiver(socket, messageFactory, Handler);

            // Act
            var isConnected = receiver.IsConnected;

            // Assert
            Assert.That(isConnected, Is.False);

            // Cleanup
            receiver.Dispose();
        }


        [Test]
        public void IsConnected_OnceBindIsCalled_IsTrue()
        {
            // Arrange
            var socket = new RouterSocket();
            var receiver = new NetMQReceiver(socket, messageFactory, Handler);
            receiver.InitializeConnection();

            // Act
            var isConnected = receiver.IsConnected;

            // Assert
            Assert.That(isConnected, Is.True);

            // Cleanup
            receiver.Dispose();
        }


        [Test]
        public void IsConnected_OnceUnbindAllIsCalled_IsFalse()
        {
            // Arrange
            var socket = new RouterSocket();
            var receiver = new NetMQReceiver(socket, messageFactory, Handler);
            receiver.InitializeConnection();
            receiver.TerminateConnection();

            // Act
            var isConnected = receiver.IsConnected;

            // Assert
            Assert.That(isConnected, Is.False);

            // Cleanup
            receiver.Dispose();
        }
        
        #endregion


        #region Addresses

        [Test]
        public void Addresses_WhenNoAddressesAdded_IsEmpty()
        {
            // Arrange
            var socket = new RouterSocket();
            var receiver = new NetMQReceiver(socket, messageFactory, Handler);

            // Act
            var any = receiver.Addresses.Any();

            // Assert
            Assert.That(any, Is.False);

            // Cleanup
            receiver.Dispose();
        }


        [Test]
        public void Addresses_WhenDisposed_ThrowsInvalidOperationException()
        {
            // Arrange
            var socket = new RouterSocket();
            var receiver = new NetMQReceiver(socket, messageFactory, Handler);
            var address = TcpAddress.Wildcard(5555);
            receiver.Dispose();

            // Act
            void AddAddress() => receiver.AddAddress(address);

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
            var socket = new RouterSocket();
            var receiver = new NetMQReceiver(socket, messageFactory, Handler);

            // Act
            TestDelegate addAddress = () => receiver.AddAddress(null);

            // Assert
            Assert.That(addAddress, Throws.ArgumentNullException);

            // Cleanup
            receiver.Dispose();
        }


        [Test]
        public void AddAddress_WithNewAddress_AddsToAddresses()
        {
            // Arrange
            var socket = new RouterSocket();
            var receiver = new NetMQReceiver(socket, messageFactory, Handler);
            var address = TcpAddress.Wildcard(5555);

            // Act
            receiver.AddAddress(address);

            // Assert
            CollectionAssert.Contains(receiver.Addresses, address);

            // Cleanup
            receiver.Dispose();
        }


        [Test]
        public void AddAddress_WithAlreadyAddedAddress_DoesNotAddTwice()
        {
            // Arrange
            var socket = new RouterSocket();
            var receiver = new NetMQReceiver(socket, messageFactory, Handler);
            var address = TcpAddress.Wildcard(5555);
            receiver.AddAddress(address);

            // Act
            receiver.AddAddress(address);

            // Assert
            Assert.That(receiver.Addresses, Has.Count.EqualTo(1));

            // Cleanup
            receiver.Dispose();
        }
        
        #endregion
        

        #region RemoveAllAddresses

        [Test]
        public void RemoveAllAddresses_WithAddedAddresses_ClearsAddresses()
        {
            // Arrange
            var socket = new RouterSocket();
            var receiver = new NetMQReceiver(socket, messageFactory, Handler);
            receiver.AddAddress(TcpAddress.Wildcard(5555));

            // Act
            receiver.RemoveAllAddresses();

            // Assert
            CollectionAssert.IsEmpty(receiver.Addresses);

            // Cleanup
            receiver.Dispose();
        }

        [Test]
        public void RemoveAllAddresses_WithAddedAddresses_IsConnectedFalse()
        {
            // Arrange
            var socket = new RouterSocket();
            var receiver = new NetMQReceiver(socket, messageFactory, Handler);
            receiver.AddAddress(TcpAddress.Wildcard(5555));

            // Act
            receiver.RemoveAllAddresses();

            // Assert
            Assert.That(receiver.IsConnected, Is.False);

            // Cleanup
            receiver.Dispose();
        }
        
        #endregion


        #region Remove

        [Test]
        public void Remove_WithAddedAddress_RemovesAddress()
        {
            // Arrange
            var socket = new RouterSocket();
            var receiver = new NetMQReceiver(socket, messageFactory, Handler);
            var address = TcpAddress.Wildcard(5555);
            receiver.AddAddress(address);

            // Act
            receiver.RemoveAddress(address);

            // Assert
            CollectionAssert.DoesNotContain(receiver.Addresses, address);

            // Cleanup
            receiver.Dispose();
        }


        [Test]
        public void Remove_WithAddedAddress_IsConnectedFalse()
        {
            // Arrange
            var socket = new RouterSocket();
            var receiver = new NetMQReceiver(socket, messageFactory, Handler);
            var address = TcpAddress.Wildcard(5555);
            receiver.AddAddress(address);

            // Act
            receiver.RemoveAddress(address);

            // Assert
            Assert.That(receiver.IsConnected, Is.False);

            // Cleanup
            receiver.Dispose();
        }


        [Test]
        public void Remove_WithAddedAddress_IsConnectedTrue()
        {
            // Arrange
            var socket = new RouterSocket();
            var receiver = new NetMQReceiver(socket, messageFactory, Handler);
            var address = TcpAddress.Wildcard(5555);
            var address2 = TcpAddress.Wildcard(5556);
            receiver.AddAddress(address);
            receiver.AddAddress(address2);
            receiver.InitializeConnection();

            // Act
            receiver.RemoveAddress(address);

            // Assert
            Assert.That(receiver.IsConnected, Is.True);

            // Cleanup
            receiver.Dispose();
        }


        [Test]
        public void Remove_WithUnaddedAddress_DoesNothing()
        { 
            // Arrange
            var socket = new RouterSocket();
            var receiver = new NetMQReceiver(socket, messageFactory, Handler);

            // Act
            TestDelegate test = () => receiver.RemoveAddress(TcpAddress.Wildcard(5555));

            // Assert
            Assert.That(test, Throws.Nothing);

            // Cleanup
            receiver.Dispose();
        }
        
        #endregion


        #region Bind

        [Test]
        public void Bind_WithNoAddresses_DoesNothing()
        {
            // Arrange
            var socket = new RouterSocket();
            var receiver = new NetMQReceiver(socket, messageFactory, Handler);

            // Act
            TestDelegate test = () => receiver.InitializeConnection();

            // Assert
            Assert.That(test, Throws.Nothing);

            // Cleanup
            receiver.Dispose();
        }


        [Test]
        public void Bind_WhenCalled_SetsIsBound()
        {
            // Arrange
            var socket = new RouterSocket();
            var receiver = new NetMQReceiver(socket, messageFactory, Handler);

            // Act
            receiver.InitializeConnection();

            // Assert
            Assert.That(receiver.IsConnected, Is.True);

            // Cleanup
            receiver.TerminateConnection();
            receiver.Dispose();
        }
        
        #endregion


        #region UnbindAll

        [Test]
        public void UnbindAll_BeforeBindCall_DoesNothing()
        {
            // Arrange
            var socket = new RouterSocket();
            var receiver = new NetMQReceiver(socket, messageFactory, Handler);

            // Act
            TestDelegate test = () => receiver.TerminateConnection();

            // Assert
            Assert.That(test, Throws.Nothing);

            // Cleanup
            receiver.Dispose();
        }
        
        #endregion
    }
}
