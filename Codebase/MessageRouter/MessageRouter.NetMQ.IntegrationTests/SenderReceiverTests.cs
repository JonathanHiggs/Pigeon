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
using MessageRouter.Receivers;

namespace MessageRouter.NetMQ.IntegrationTests
{
    [TestFixture]
    public class SenderReceiverTests
    {
        [Test]
        public async Task SenderReceiver_WhenBoundAndConnected_PassesMessage()
        {
            // Arrange
            var responseStr = "Hello, World!";
            var requestStr = "Hello";
            var receivedRequest = String.Empty;
            var called = false;

            RequestTaskHandler handler = (rec, task) =>
            {
                called = true;
                receivedRequest = (string)task.Request.Body;
                var responsePackage = new DataPackage<string>(new GuidPackageId(), responseStr);
                task.ResponseHandler(responsePackage);
            };

            var serializer = new DotNetSerializer();
            var sender = new NetMQSender(new AsyncSocket(new DealerSocket()), serializer);
            var receiver = new NetMQReceiver(new RouterSocket(), serializer, handler);
            var poller = new NetMQPoller();

            sender.AddAddress(TcpAddress.Localhost(5555));
            sender.InitializeConnection();
            receiver.AddAddress(TcpAddress.Wildcard(5555));
            receiver.InitializeConnection();
                       
            poller.Add(sender.PollableSocket);
            poller.Add(receiver.PollableSocket);
            poller.RunAsync();

            var package = new DataPackage<string>(new GuidPackageId(), requestStr);

            // Act
            var response = await sender.SendAndReceive(package, TimeSpan.FromSeconds(5));

            // Cleanup
            poller.StopAsync();
            sender.TerminateConnection();
            receiver.TerminateConnection();

            // Assert
            Assert.That(called, Is.True);
            Assert.That(receivedRequest, Is.EqualTo(requestStr));
            Assert.That((string)response.Body, Is.EqualTo(responseStr));
        }
    }
}
