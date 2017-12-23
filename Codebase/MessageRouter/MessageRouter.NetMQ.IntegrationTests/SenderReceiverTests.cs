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
            var packageFactory = new PackageFactory();

            RequestTaskHandler handler = (rec, task) =>
            {
                var requestMessage = packageFactory.Unpack(task.Request);
                var responseMessage = $"{requestMessage}, World!";
                var responsePackage = packageFactory.Pack(responseMessage);
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

            var package = new DataPackage<string>(new GuidPackageId(), "Hello");

            // Act
            var response = await sender.SendAndReceive(package, TimeSpan.FromSeconds(5));

            // Cleanup
            poller.StopAsync();
            sender.TerminateConnection();
            receiver.TerminateConnection();

            // Assert
            Assert.That(response.Body, Is.EqualTo("Hello, World!"));
        }
    }
}
