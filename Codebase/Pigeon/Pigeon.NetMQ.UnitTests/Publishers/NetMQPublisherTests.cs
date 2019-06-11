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
        }
        #endregion
    }
}
