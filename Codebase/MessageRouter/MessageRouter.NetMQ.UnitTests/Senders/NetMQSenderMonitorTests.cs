using MessageRouter.NetMQ.Senders;
using Moq;
using NetMQ;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessageRouter.NetMQ.UnitTests.Senders
{
    [TestFixture]
    public class NetMQSenderMonitorTests
    {
        private readonly Mock<INetMQPoller> mockPoller = new Mock<INetMQPoller>();
        private INetMQPoller poller;

        private readonly Mock<INetMQSender> mockSender = new Mock<INetMQSender>();
        private INetMQSender sender;

        private readonly Mock<ISocketPollable> mockSocketPollable = new Mock<ISocketPollable>();
        private ISocketPollable socketPollable;

        
        [SetUp]
        public void Setup()
        {
            poller = mockPoller.Object;
            sender = mockSender.Object;
            socketPollable = mockSocketPollable.Object;

            mockSender
                .SetupGet(m => m.PollableSocket)
                .Returns(socketPollable);
        }


        [TearDown]
        public void TearDown()
        {
            mockPoller.Reset();
            mockSender.Reset();
            mockSocketPollable.Reset();
        }


        #region Constructor
        [Test]
        public void NetMQSenderMonitor_WithNullPoller_ThrowsArgumentNullException()
        {
            // Act
            TestDelegate construct = () => new NetMQSenderMonitor(null);

            // Assert
            Assert.That(construct, Throws.ArgumentNullException);
        }
        #endregion


        #region AddSender
        [Test]
        public void AddSender_BeforeMonitorStarted_AddsToPoller()
        {
            // Arrange
            var monitor = new NetMQSenderMonitor(poller);

            // Act
            monitor.AddSender(sender);

            // Assert
            mockPoller.Verify(m => m.Add(It.IsIn(socketPollable)), Times.Once);
        }


        [Test]
        public void AddSender_BeforeMonitorStarted_SenderConnectNotCalled()
        {
            // Arrange
            var monitor = new NetMQSenderMonitor(poller);

            // Act
            monitor.AddSender(sender);

            // Assert
            mockSender.Verify(m => m.ConnectAll(), Times.Never);
        }


        [Test]
        public void AddSender_WithMonitorStarted_ConnectsSender()
        {
            // Arrange
            var monitor = new NetMQSenderMonitor(poller);
            monitor.StartSenders();

            // Act
            monitor.AddSender(sender);

            // Assert
            mockSender.Verify(m => m.ConnectAll(), Times.Once);
        }


        [Test]
        public void AddSender_AfterMonitorStopped_ConnectNotCalled()
        {
            // Arrange
            var monitor = new NetMQSenderMonitor(poller);
            monitor.StartSenders();
            monitor.StopSenders();

            // Act
            monitor.AddSender(sender);

            // Assert
            mockSender.Verify(m => m.ConnectAll(), Times.Never);
        }


        [Test]
        public void AddSender_WithNullSender_ThrowsArgumentNullException()
        {
            // Arrange
            var monitor = new NetMQSenderMonitor(poller);

            // Act
            TestDelegate addSender = () =>  monitor.AddSender(null);

            // Assert
            Assert.That(addSender, Throws.ArgumentNullException);
        }
        #endregion


        #region StartSenders
        [Test]
        public void StartSenders_WithNoSenders_CallsPoller()
        {
            // Arrange
            var monitor = new NetMQSenderMonitor(poller);

            // Act
            monitor.StartSenders();

            // Assert
            mockPoller.Verify(m => m.RunAsync(), Times.Once);
        }


        [Test]
        public void StartSenders_WhenAlreadyStarted_DoesNothing()
        {
            // Arrange
            var monitor = new NetMQSenderMonitor(poller);
            monitor.StartSenders();
            mockPoller.ResetCalls();

            // Act
            monitor.StartSenders();

            // Assert
            mockPoller.Verify(m => m.RunAsync(), Times.Never);
        }


        [Test]
        public void StartSenders_WithSenderAdded_CallsSenderConnectAll()
        {
            // Arrange
            var monitor = new NetMQSenderMonitor(poller);
            monitor.AddSender(sender);

            // Act
            monitor.StartSenders();

            // Assert
            mockSender.Verify(m => m.ConnectAll(), Times.Once);
        }
        #endregion


        #region StopSenders
        [Test]
        public void StopSenders_BeforeStarted_DoesNothing()
        {
            // Arrange
            var monitor = new NetMQSenderMonitor(poller);
            monitor.AddSender(sender);

            // Act
            monitor.StopSenders();

            // Assert
            mockPoller.Verify(m => m.StopAsync(), Times.Never);
            mockSender.Verify(m => m.DisconnectAll(), Times.Never);
        }


        [Test]
        public void StopSenders_WithMonitorStarted_CallsPollerStop()
        {
            // Arrange
            var monitor = new NetMQSenderMonitor(poller);
            monitor.StartSenders();

            // Act
            monitor.StopSenders();

            // Assert
            mockPoller.Verify(m => m.StopAsync(), Times.Once);
        }


        [Test]
        public void StopSenders_WithSenderAdded_CallsSenderDisconnect()
        {
            // Arrange
            var monitor = new NetMQSenderMonitor(poller);
            monitor.AddSender(sender);
            monitor.StartSenders();

            // Act
            monitor.StopSenders();

            // Assert
            mockSender.Verify(m => m.DisconnectAll(), Times.Once);
        }
        #endregion
    }
}
