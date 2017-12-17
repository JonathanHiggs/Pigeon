using MessageRouter.Addresses;
using MessageRouter.Packages;
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
            var packageFactory = new PackageFactory();

            sender.AddAddress(TcpAddress.Localhost(5555));
            sender.InitializeConnection();
            receiver.AddAddress(TcpAddress.Wildcard(5555));
            receiver.InitializeConnection();
                        
            receiver.RequestReceived += (s, e) =>
            {
                var requestMessage = packageFactory.Unpack(e.Request);
                var responseMessage = $"{requestMessage}, World!";
                var responsePackage = packageFactory.Pack(responseMessage);
                e.ResponseHandler(responsePackage);
            };

            poller.Add(sender.PollableSocket);
            poller.Add(receiver.PollableSocket);
            poller.RunAsync();

            var package = new DataPackage<string>(new GuidPackageId(), "Hello");

            // Act
            var response = await sender.SendAndReceive(package);

            // Cleanup
            poller.StopAsync();
            sender.TerminateConnection();
            receiver.TerminateConnection();

            // Assert
            Assert.That(response.Body, Is.EqualTo("Hello, World!"));
        }
    }
}
