﻿using MessageRouter.Addresses;
using MessageRouter.Messages;
using MessageRouter.Serialization;
using NetMQ.Sockets;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessageRouter.NetMQ.Tests
{
    [TestFixture]
    public class SenderReceiverTest
    {
        [Test]
        public void SendAndReceive()
        {
            // Arrange
            var binarySerializer = new BinarySerializer();
            var routerSocket = new RouterSocket();
            var receiver = new NetMQReceiver(routerSocket, binarySerializer);
            var dealerSocket = new DealerSocket();
            var asyncSocket = new AsyncSocket(dealerSocket);
            var sender = new NetMQSender(asyncSocket, binarySerializer);
            
            receiver.Add(TcpAddress.Wildcard(5555));
            receiver.Bind();
            sender.Connect(TcpAddress.Localhost(5555));

            Task.Factory.StartNew(() =>
            {
                var r = receiver.Receive();
                var ret = new DataMessage<string>(new GuidMessageId(), "Hello");
                r.ResponseHandler(ret);
            });

            var request = new DataMessage<string>(new GuidMessageId(), "Hi?");

            // Act
            var response = sender.SendAndReceive(request).Body.ToString();

            // Assert
            Assert.That(response, Is.EqualTo("Hello"));
        }
    }
}
