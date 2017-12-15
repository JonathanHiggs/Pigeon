﻿using MessageRouter.Addresses;
using MessageRouter.NetMQ.Receivers;
using MessageRouter.Serialization;
using Moq;
using NetMQ.Sockets;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessageRouter.NetMQ.UnitTests.Receivers
{
    [TestFixture]
    public class NetMQReceiverTests
    {
        private readonly Mock<ISerializer<byte[]>> mockSerializer = new Mock<ISerializer<byte[]>>();
        private ISerializer<byte[]> serializer;


        [SetUp]
        public void Setup()
        {
            serializer = mockSerializer.Object;
        }


        #region Constructor
        [Test]
        public void NetMQReceiver_WithNullRouterSocket_ThrowsArgumentNullException()
        {
            // Act
            TestDelegate test = () => new NetMQReceiver(null, serializer);

            // Assert
            Assert.That(test, Throws.ArgumentNullException);
        }


        [Test]
        public void NetMQReceiver_WithNullSerializer_ThrowsArgumentNullException()
        {
            // Act
            var routerSocket = new RouterSocket();
            TestDelegate test = () => new NetMQReceiver(routerSocket, null);

            // Assert
            Assert.That(test, Throws.ArgumentNullException);
        }
        #endregion


        #region IsBound
        [Test]
        public void IsBound_BeforeBindIsCalled_IsFalse()
        {
            // Arrange
            var routerSocket = new RouterSocket();
            var receiver = new NetMQReceiver(routerSocket, serializer);

            // Act
            var isBound = receiver.IsBound;

            // Assert
            Assert.That(isBound, Is.False);
        }


        [Test]
        public void IsBound_OnceBindIsCalled_IsTrue()
        {
            // Arrange
            var routerSocket = new RouterSocket();
            var receiver = new NetMQReceiver(routerSocket, serializer);
            receiver.BindAll();

            // Act
            var isBound = receiver.IsBound;

            // Assert
            Assert.That(isBound, Is.True);
        }


        [Test]
        public void IsBound_OnceUnbindAllIsCalled_IsFalse()
        {
            // Arrange
            var routerSocket = new RouterSocket();
            var receiver = new NetMQReceiver(routerSocket, serializer);
            receiver.BindAll();
            receiver.UnbindAll();

            // Act
            var isBound = receiver.IsBound;

            // Assert
            Assert.That(isBound, Is.False);
        }
        #endregion


        #region Addresses
        [Test]
        public void Addresses_WhenNoAddressesAdded_IsEmpty()
        {
            // Arrange
            var routerSocket = new RouterSocket();
            var receiver = new NetMQReceiver(routerSocket, serializer);

            // Act
            var any = receiver.Addresses.Any();

            // Assert
            Assert.That(any, Is.False);
        }
        #endregion


        #region AddAddress
        [Test]
        public void AddAddress_WithNullAddress_ThrowsArgumentNullException()
        {
            // Arrange
            var routerSocket = new RouterSocket();
            var receiver = new NetMQReceiver(routerSocket, serializer);

            // Act
            TestDelegate addAddress = () => receiver.AddAddress(null);

            // Assert
            Assert.That(addAddress, Throws.ArgumentNullException);
        }


        [Test]
        public void AddAddress_WithNewAddress_AddsToAddresses()
        {
            // Arrange
            var routerSocket = new RouterSocket();
            var receiver = new NetMQReceiver(routerSocket, serializer);
            var address = TcpAddress.Wildcard(5555);

            // Act
            receiver.AddAddress(address);

            // Assert
            CollectionAssert.Contains(receiver.Addresses, address);
        }


        [Test]
        public void AddAddress_WithAlreadyAddedAddress_DoesNotAddTwice()
        {
            // Arrange
            var routerSocket = new RouterSocket();
            var receiver = new NetMQReceiver(routerSocket, serializer);
            var address = TcpAddress.Wildcard(5555);
            receiver.AddAddress(address);

            // Act
            receiver.AddAddress(address);

            // Assert
            Assert.That(receiver.Addresses, Has.Count.EqualTo(1));
        }
        #endregion


        #region RemoveAddress

        #endregion


        #region RemoveAll
        [Test]
        public void RemoveAll_WithAddedAddresses_ClearsAddresses()
        {
            // Arrange
            var routerSocket = new RouterSocket();
            var receiver = new NetMQReceiver(routerSocket, serializer);
            receiver.AddAddress(TcpAddress.Wildcard(5555));

            // Act
            receiver.RemoveAllAddresses();

            // Assert
            CollectionAssert.IsEmpty(receiver.Addresses);
        }


        [Test]
        public void RemoveAll_WhenBound_Unbinds()
        {
            // Arrange
            var routerSocket = new RouterSocket();
            var receiver = new NetMQReceiver(routerSocket, serializer);
            receiver.BindAll();

            // Act
            receiver.RemoveAllAddresses();

            // Assert
            Assert.That(receiver.IsBound, Is.False);
        }
        #endregion


        #region Remove
        [Test]
        public void Remove_WithAddedAddress_RemovesAddress()
        {
            // Arrange
            var routerSocket = new RouterSocket();
            var receiver = new NetMQReceiver(routerSocket, serializer);
            var address = TcpAddress.Wildcard(5555);
            receiver.AddAddress(address);

            // Act
            receiver.RemoveAddress(address);

            // Assert
            CollectionAssert.DoesNotContain(receiver.Addresses, address);
        }


        [Test]
        public void Remove_WithUnaddedAddress_DoesNothing()
        {
            // Arrange
            var routerSocket = new RouterSocket();
            var receiver = new NetMQReceiver(routerSocket, serializer);

            // Act
            TestDelegate test = () => receiver.RemoveAddress(TcpAddress.Wildcard(5555));

            // Assert
            Assert.That(test, Throws.Nothing);
        }
        #endregion


        #region Bind
        [Test]
        public void Bind_WithNoAddresses_DoesNothing()
        {
            // Arrange
            var routerSocket = new RouterSocket();
            var receiver = new NetMQReceiver(routerSocket, serializer);

            // Act
            TestDelegate test = () => receiver.BindAll();

            // Assert
            Assert.That(test, Throws.Nothing);
        }


        [Test]
        public void Bind_WhenCalled_SetsIsBound()
        {
            // Arrange
            var routerSocket = new RouterSocket();
            var receiver = new NetMQReceiver(routerSocket, serializer);

            // Act
            receiver.BindAll();

            // Assert
            Assert.That(receiver.IsBound, Is.True);
        }
        #endregion


        #region UnbindAll
        [Test]
        public void UnbindAll_BeforeBindCall_DoesNothing()
        {
            // Arrange
            var routerSocket = new RouterSocket();
            var receiver = new NetMQReceiver(routerSocket, serializer);

            // Act
            TestDelegate test = () => receiver.UnbindAll();

            // Assert
            Assert.That(test, Throws.Nothing);
        }
        #endregion
    }
}
