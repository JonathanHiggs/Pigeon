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
            var dealerSocket = new DealerSocket();

            // Act
            TestDelegate construct = () => new NetMQSender(dealerSocket, null);

            // Assert
            Assert.That(construct, Throws.ArgumentNullException);
        }
        #endregion


        #region AddAddress
        [Test]
        public void AddAddress_WithNewAddress_IsInAddressList()
        {
            // Arrange
            var dealerSocket = new DealerSocket();
            var sender = new NetMQSender(dealerSocket, messageFactory);

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
            var dealerSocket = new DealerSocket();
            var sender = new NetMQSender(dealerSocket, messageFactory);
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
            var dealerSocket = new DealerSocket();
            var sender = new NetMQSender(dealerSocket, messageFactory);
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
            var dealerSocket = new DealerSocket();
            var sender = new NetMQSender(dealerSocket, messageFactory);

            // Act
            TestDelegate remove = () => sender.RemoveAddress(address);

            // Assert
            Assert.That(remove, Throws.Nothing);
        }
        #endregion
    }
}
