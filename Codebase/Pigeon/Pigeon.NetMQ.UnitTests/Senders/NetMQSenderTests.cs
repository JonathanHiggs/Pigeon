using System;

using Moq;

using NetMQ.Sockets;

using NUnit.Framework;

using Pigeon.Addresses;
using Pigeon.NetMQ.Senders;

namespace Pigeon.NetMQ.UnitTests.Senders
{
    [TestFixture]
    public class NetMQSenderTests
    {
        private readonly Mock<INetMQMessageFactory> mockMessageFactory = new Mock<INetMQMessageFactory>();
        private INetMQMessageFactory messageFactory;
        
        private IAddress address = TcpAddress.Wildcard(5555);
        

        [SetUp]
        public void Setup()
        {
            messageFactory = mockMessageFactory.Object;
        }


        [TearDown]
        public void TearDown()
        {
            mockMessageFactory.Reset();
        }


        #region Constructor

        [Test]
        public void NetMQSender_WithMissingSocket_ThrowsArugmentNullException()
        {
            // Act
            TestDelegate construct = () => new NetMQSender(null, messageFactory);

            // Assert
            Assert.That(construct, Throws.ArgumentNullException);
        }


        [Test]
        public void NetMQSender_WithMissingSerializer_ThrowsArgumentNullException()
        {
            // Arrange
            var socket = new DealerSocket();

            // Act
            TestDelegate construct = () => new NetMQSender(socket, null);

            // Assert
            Assert.That(construct, Throws.ArgumentNullException);

            // Cleanup
            socket.Dispose();
        }
        
        #endregion


        #region AddAddress

        [Test]
        public void AddAddress_WithNewAddress_IsInAddressList()
        {
            // Arrange
            var socket = new DealerSocket();
            var sender = new NetMQSender(socket, messageFactory);

            // Act
            sender.AddAddress(address);

            // Assert
            Assert.That(sender.Addresses, Has.Count.EqualTo(1));
            Assert.That(sender.Addresses, Has.Exactly(1).EqualTo(address));

            // Cleanup
            sender.Dispose();
        }


        [Test]
        public void AddAddress_WithAlreadyAddedAddress_DoesNothing()
        {
            // Arrange
            var socket = new DealerSocket();
            var sender = new NetMQSender(socket, messageFactory);
            sender.AddAddress(address);

            // Act
            sender.AddAddress(address);

            // Assert
            Assert.That(sender.Addresses, Has.Count.EqualTo(1));
            Assert.That(sender.Addresses, Has.Exactly(1).EqualTo(address));

            // Cleanup
            sender.Dispose();
        }


        [Test]
        public void Addresses_WhenDisposed_ThrowsInvalidOperationException()
        {
            // Arrange
            var socket = new DealerSocket();
            var sender = new NetMQSender(socket, messageFactory);
            var address = TcpAddress.Wildcard(5555);
            sender.Dispose();

            // Act
            void AddAddress() => sender.AddAddress(address);

            // Assert
            Assert.That(AddAddress, Throws.TypeOf<InvalidOperationException>());

            // Cleanup
            sender.Dispose();
        }

        #endregion


        #region RemoveAddress

        [Test]
        public void RemoveAddress_WithAddedAddress_RemovesFromAddressList()
        {
            // Arrange
            var socket = new DealerSocket();
            var sender = new NetMQSender(socket, messageFactory);
            sender.AddAddress(address);

            // Act
            sender.RemoveAddress(address);

            // Assert
            CollectionAssert.IsEmpty(sender.Addresses);

            // Cleanup
            sender.Dispose();
        }


        [Test]
        public void RemoveAddress_WithNoAddresses_DoesNothing()
        {
            // Arrange
            var socket = new DealerSocket();
            var sender = new NetMQSender(socket, messageFactory);

            // Act
            TestDelegate remove = () => sender.RemoveAddress(address);

            // Assert
            Assert.That(remove, Throws.Nothing);

            // Cleanup
            sender.Dispose();
        }
        

        [Test]
        public void Remove_WithAddedAddress_IsConnectedFalse()
        {
            // Arrange
            var socket = new DealerSocket();
            var sender = new NetMQSender(socket, messageFactory);
            var address = TcpAddress.Wildcard(5555);
            sender.AddAddress(address);

            // Act
            sender.RemoveAddress(address);

            // Assert
            Assert.That(sender.IsConnected, Is.False);

            // Cleanup
            sender.Dispose();
        }


        [Test]
        public void Remove_WithAddedAddress_IsConnectedTrue()
        {
            // Arrange
            var socket = new DealerSocket();
            var sender = new NetMQSender(socket, messageFactory);
            var address = TcpAddress.Wildcard(5555);
            var address2 = TcpAddress.Wildcard(5556);
            sender.AddAddress(address);
            sender.AddAddress(address2);
            sender.InitializeConnection();

            // Act
            sender.RemoveAddress(address);

            // Assert
            Assert.That(sender.IsConnected, Is.True);

            // Cleanup
            sender.TerminateConnection();
            sender.Dispose();
        }

        #endregion
    }
}
