using MessageRouter.Addresses;
using MessageRouter.Messages;
using MessageRouter.Serialization;
using NetMQ;
using NetMQ.Sockets;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessageRouter.NetMQ.IntegrationTests
{
    [TestFixture]
    public class AsyncSenderReceiverTest
    {
        [Test]
        public async Task AsyncSendAndReceive()
        {
            // Arrange
            var binarySerializer = new BinarySerializer();

            var routerSocket = new RouterSocket();
            var receiver = new NetMQReceiver(routerSocket, binarySerializer);
            var poller = new NetMQPoller();
            var receiverManager = new NetMQReceiverManager(receiver, poller);

            var dealerSocket = new DealerSocket();
            var asyncSocket = new AsyncSocket(dealerSocket);
            var sender = new NetMQSender(asyncSocket, binarySerializer);

            receiverManager.RequestReceived += (s, task) => {
                var reply = new DataMessage<string>(new GuidMessageId(), "Hi");
                task.ResponseHandler(reply);
            };

            var request = new DataMessage<string>(new GuidMessageId(), "Hello");

            // Act
            sender.Connect(TcpAddress.Localhost(5555));
            receiverManager.Start();

            var response = await sender.SendAndReceiveAsync(request);

            receiverManager.Stop();

            // Assert
            Assert.That(response.Body.ToString(), Is.EqualTo("Hi"));
        }
    }
}
