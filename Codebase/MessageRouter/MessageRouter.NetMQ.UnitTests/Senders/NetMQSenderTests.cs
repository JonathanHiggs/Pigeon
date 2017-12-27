using MessageRouter.Addresses;
using MessageRouter.Packages;
using MessageRouter.NetMQ.Senders;
using MessageRouter.Serialization;
using Moq;
using NetMQ;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessageRouter.NetMQ.UnitTests.Senders
{
    [TestFixture]
    public class NetMQSenderTests
    {
        private readonly Mock<IAsyncSocket> mockAsyncSocket = new Mock<IAsyncSocket>();
        private IAsyncSocket asyncSocket;

        private readonly Mock<IMessageFactory> mockMessageFactory = new Mock<IMessageFactory>();
        private IMessageFactory messageFactory;
        
        private IAddress address = TcpAddress.Wildcard(5555);
        

        [SetUp]
        public void Setup()
        {
            asyncSocket = mockAsyncSocket.Object;
            messageFactory = mockMessageFactory.Object;
        }


        [TearDown]
        public void TearDown()
        {
            mockAsyncSocket.Reset();
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
            // Act
            TestDelegate construct = () => new NetMQSender(asyncSocket, null);

            // Assert
            Assert.That(construct, Throws.ArgumentNullException);
        }
        #endregion


        #region AddAddress
        [Test]
        public void AddAddress_WithNewAddress_IsInAddressList()
        {
            // Arrange
            var sender = new NetMQSender(asyncSocket, messageFactory);

            // Act
            sender.AddAddress(address);

            // Assert
            Assert.That(sender.Addresses, Has.Count.EqualTo(1));
            Assert.That(sender.Addresses, Has.Exactly(1).EqualTo(address));
        }


        [Test]
        public void AddAddress_WithAlreadyAddedAddress_DoesNothing()
        {
            // Arrange
            var sender = new NetMQSender(asyncSocket, messageFactory);
            sender.AddAddress(address);

            // Act
            sender.AddAddress(address);

            // Assert
            Assert.That(sender.Addresses, Has.Count.EqualTo(1));
            Assert.That(sender.Addresses, Has.Exactly(1).EqualTo(address));
        }
        #endregion


        #region RemoveAddress
        [Test]
        public void RemoveAddress_WithAddedAddress_RemovesFromAddressList()
        {
            // Arrange
            var sender = new NetMQSender(asyncSocket, messageFactory);
            sender.AddAddress(address);

            // Act
            sender.RemoveAddress(address);

            // Assert
            CollectionAssert.IsEmpty(sender.Addresses);
        }


        [Test]
        public void RemoveAddress_WithNoAddresses_DoesNothing()
        {
            // Arrange
            var sender = new NetMQSender(asyncSocket, messageFactory);

            // Act
            TestDelegate remove = () => sender.RemoveAddress(address);

            // Assert
            Assert.That(remove, Throws.Nothing);
        }
        #endregion


        #region ConnectAll
        [Test]
        public void ConnectAll_WithNoAddresses_DoesNothing()
        {
            // Arrange
            var sender = new NetMQSender(asyncSocket, messageFactory);

            // Act
            sender.InitializeConnection();

            // Assert
            mockAsyncSocket.Verify(m => m.Connect(It.IsAny<string>()), Times.Never);
        }


        [Test]
        public void ConnectAll_WithAddedAddress_CallsSocketConnect()
        {
            // Arrange
            var sender = new NetMQSender(asyncSocket, messageFactory);
            sender.AddAddress(address);

            // Act
            sender.InitializeConnection();

            // Assert
            mockAsyncSocket.Verify(m => m.Connect(It.IsIn(address.ToString())), Times.Once);
            mockAsyncSocket.Verify(m => m.Connect(It.IsNotIn(address.ToString())), Times.Never);
        }
        #endregion


        #region DisconnectAll
        [Test]
        public void DisconnectAll_WithNoAddresses_DoesNothing()
        {
            // Arrange
            var sender = new NetMQSender(asyncSocket, messageFactory);

            // Act
            sender.TerminateConnection();

            // Assert
            mockAsyncSocket.Verify(m => m.Disconnect(It.IsAny<string>()), Times.Never);
        }
        #endregion
    }
}
