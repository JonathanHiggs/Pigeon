using MessageRouter.NetMQ.Receivers;
using MessageRouter.Receivers;
using Moq;
using NetMQ;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessageRouter.NetMQ.UnitTests.Receivers
{
    [TestFixture]
    public class NetMQReceiverMonitorTests
    {
        private readonly Mock<INetMQReceiver> mockReceiver = new Mock<INetMQReceiver>();
        private INetMQReceiver receiver;

        private readonly Mock<INetMQPoller> mockPoller = new Mock<INetMQPoller>();
        private INetMQPoller poller;


        [SetUp]
        public void Setup()
        {
            receiver = mockReceiver.Object;
            poller = mockPoller.Object;
        }


        [TearDown]
        public void TearDown()
        {
            mockReceiver.Reset();
            mockPoller.Reset();
        }


        #region Constructor
        [Test]
        public void NetMQReceiverMonitor_WithNullReceiver_ThrowsArgumentNullException()
        {
            // Act
            TestDelegate test = () => new NetMQReceiverMonitor(null, poller);

            // Assert
            Assert.That(test, Throws.ArgumentNullException);
        }


        [Test]
        public void NetMQReceiverMonitor_WithNullPoller_ThrowsArgumentNullException()
        {
            // Act
            TestDelegate test = () => new NetMQReceiverMonitor(receiver, null);

            // Assert
            Assert.That(test, Throws.ArgumentNullException);
        }


        [Test]
        public void NetMQReceiverMonitor_WithDependencies_AddsReceiverToPoller()
        {
            // Act
            var receiverMonitor = new NetMQReceiverMonitor(receiver, poller);

            // Assert
            mockPoller.Verify(m => m.Add(It.IsAny<ISocketPollable>()), Times.Once);
        }
        #endregion


        #region Start
        [Test]
        public void Start_WhenNotRunning_RunsPoller()
        {
            // Arrange
            var receiverMonitor = new NetMQReceiverMonitor(receiver, poller);

            // Act
            receiverMonitor.StartReceivers();

            // Assert
            mockPoller.Verify(m => m.RunAsync(), Times.Once);
        }


        [Test]
        public void Sart_WhenNotRunning_BindsReceiver()
        {
            // Arrange
            var receiverMonitor = new NetMQReceiverMonitor(receiver, poller);

            // Act
            receiverMonitor.StartReceivers();

            // Assert
            mockReceiver.Verify(m => m.Bind(), Times.Once);
        }
        #endregion


        #region Stop
        [Test]
        public void Stop_WhenRunning_StopsPoller()
        {
            // Arrange
            var receiverMonitor = new NetMQReceiverMonitor(receiver, poller);
            receiverMonitor.StartReceivers();

            // Act
            receiverMonitor.StopReceivers();

            // Assert
            mockPoller.Verify(m => m.StopAsync(), Times.Once);
        }


        [Test]
        public void Stop_WhenRunning_UnbindsReceiver()
        {
            // Arrange
            var receiverMonitor = new NetMQReceiverMonitor(receiver, poller);
            receiverMonitor.StartReceivers();

            // Act
            receiverMonitor.StopReceivers();

            // Assert
            mockReceiver.Verify(m => m.UnbindAll(), Times.Once);
        }
        #endregion


        #region RequestReceived
        [Test]
        public void RequestReceived_WhenReceiverRaises_IsRaised()
        {
            // Arrange
            var receiverMonitor = new NetMQReceiverMonitor(receiver, poller);
            var called = false;
            receiverMonitor.RequestReceived += (sender, task) => called = true;

            // Act
            mockReceiver.Raise(r => r.RequestReceived += null, receiver, new RequestTask());

            // Assert
            Assert.That(called, Is.True);
        }
        #endregion
    }
}
