using MessageRouter.Addresses;
using MessageRouter.Messages;
using MessageRouter.NetMQ.Receivers;
using MessageRouter.NetMQ.Senders;
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
    public class SenderReceiverTests
    {
        [Test]
        public async Task SenderReceiver_WhenBoundAndConnected_PassesMessage()
        {
            // Arrange
            var serializer = new BinarySerializer();
            var sender = new NetMQSender(new AsyncSocket(new DealerSocket()), serializer);
            var receiver = new NetMQReceiver(new RouterSocket(), serializer);
            var poller = new NetMQPoller();
            var messageFactory = new MessageFactory();

            sender.AddAddress(TcpAddress.Localhost(5555));
            sender.ConnectAll();
            receiver.AddAddress(TcpAddress.Wildcard(5555));
            receiver.BindAll();
                        
            receiver.RequestReceived += (s, e) =>
            {
                var requestMessage = messageFactory.ExtractMessage(e.Request);
                var responseMessage = $"{requestMessage}, World!";
                var responsePackage = messageFactory.CreateMessage(responseMessage);
                e.ResponseHandler(responsePackage);
            };

            poller.Add(sender.PollableSocket);
            poller.Add(receiver.PollableSocket);
            poller.RunAsync();

            var request = new DataMessage<string>(new GuidMessageId(), "Hello");

            // Act
            var response = await sender.SendAndReceive(request);

            // Cleanup
            poller.StopAsync();
            sender.DisconnectAll();
            receiver.UnbindAll();

            // Assert
            Assert.That(response.Body, Is.EqualTo("Hello, World!"));
        }
    }
}
