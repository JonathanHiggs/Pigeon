﻿using System;
using System.Threading.Tasks;
using Moq;
using NetMQ;
using NetMQ.Sockets;

using NUnit.Framework;

using Pigeon.Addresses;
using Pigeon.NetMQ.Receivers;
using Pigeon.NetMQ.Senders;
using Pigeon.Packages;
using Pigeon.Receivers;
using Pigeon.Requests;
using Pigeon.Serialization;

namespace Pigeon.NetMQ.IntegrationTests
{
    delegate void Handler(ref RequestTask task);


    [TestFixture]
    public class SenderReceiverTests
    {
        private readonly Mock<IRequestDispatcher> mockRequestDispatcher = new Mock<IRequestDispatcher>();

        private IRequestDispatcher requestDispatcher;


        [SetUp]
        public void Setup()
        {
            requestDispatcher = mockRequestDispatcher.Object;
        }


        [TearDown]
        public void Teardown()
        {
            mockRequestDispatcher.Reset();
        }


        [Test]
        public async Task SenderReceiver_WhenBoundAndConnected_PassesMessage()
        {
            // Arrange
            var responseStr = "Hello, World!";
            var requestStr = "Hello";
            var receivedRequest = string.Empty;
            ushort port = 6555;
            var called = false;

            mockRequestDispatcher
                .Setup(m => m.Handle(ref It.Ref<RequestTask>.IsAny))
                .Callback(new Handler((ref RequestTask task) => {
                    called = true;
                    receivedRequest = (string)task.Request;
                    task.ResponseSender(responseStr);
                }));

            var packageFactory = new PackageFactory();
            var serializer = new DotNetSerializer();
            var serializerCache = new SerializerCache();
            var messageFactory = new NetMQMessageFactory(serializerCache, packageFactory);
            var sender = new NetMQSender(new DealerSocket(), messageFactory);
            var receiver = new NetMQReceiver(new RouterSocket(), messageFactory, requestDispatcher);
            var poller = new NetMQPoller();

            serializerCache.AddSerializer(serializer);

            sender.AddAddress(TcpAddress.Localhost(port));
            sender.InitializeConnection();

            receiver.AddAddress(TcpAddress.Wildcard(port));
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
            Assert.That((string)response, Is.EqualTo(responseStr));
        }
    }
}
