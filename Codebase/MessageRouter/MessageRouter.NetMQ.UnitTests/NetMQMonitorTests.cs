using MessageRouter.NetMQ.Receivers;
using MessageRouter.NetMQ.Senders;
using Moq;
using NetMQ;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessageRouter.NetMQ.UnitTests
{
    [TestFixture]
    public class NetMQMonitorTests
    {
        private readonly Mock<INetMQPoller> mockPoller = new Mock<INetMQPoller>();
        private INetMQPoller poller;

        private readonly Mock<INetMQSender> mockSender = new Mock<INetMQSender>();
        private INetMQSender sender;

        private readonly Mock<INetMQReceiver> mockReceiver = new Mock<INetMQReceiver>();
        private INetMQReceiver receiver;

        private readonly Mock<ISocketPollable> mockPollableSocket = new Mock<ISocketPollable>();
        private ISocketPollable pollableSocket;
        

        [SetUp]
        public void Setup()
        {
            poller = mockPoller.Object;
            sender = mockSender.Object;
            receiver = mockReceiver.Object;
            pollableSocket = mockPollableSocket.Object;

            mockSender
                .SetupGet(m => m.PollableSocket)
                .Returns(pollableSocket);

            mockReceiver
                .SetupGet(m => m.PollableSocket)
                .Returns(pollableSocket);
        }


        [TearDown]
        public void TearDown()
        {
            mockPoller.Reset();
            mockSender.Reset();
            mockReceiver.Reset();
            mockPollableSocket.Reset();
        }


        #region Constructor
        [Test]
        public void NetMQMonitor_WithNullPoller_ThrowsArgumentNullException()
        {
            // Act
            TestDelegate construct = () => new NetMQMonitor(null);

            // Assert
            Assert.That(construct, Throws.ArgumentNullException);
        }
        #endregion


        #region AddReceiver
        [Test]
        public void AddReceiver_WithNullReceiver_ThrowsArgumentNullException()
        {
            // Arrange
            var monitor = new NetMQMonitor(poller);

            // Act
            TestDelegate add = () => monitor.AddReceiver(null);

            // Assert
            Assert.That(add, Throws.ArgumentNullException);
        }


        [Test]
        public void AddReceiver_WithReceiver_AddsPollableSocketToPoller()
        {
            // Arrange
            var monitor = new NetMQMonitor(poller);

            // Act
            monitor.AddReceiver(receiver);

            // Assert
            mockPoller.Verify(m => m.Add(It.IsIn(pollableSocket)), Times.Once);
        }


        [Test]
        public void AddReceiver_BeforeRunning_DoesNotBindReceiver()
        {
            // Arrange
            var monitor = new NetMQMonitor(poller);

            // Act
            monitor.AddReceiver(receiver);

            // Assert
            mockReceiver.Verify(m => m.BindAll(), Times.Never);
        }


        [Test]
        public void AddReceiver_AfterStarted_BindsReceiver()
        {
            // Arrange
            var monitor = new NetMQMonitor(poller);
            monitor.StartMonitoring();

            // Act
            monitor.AddReceiver(receiver);

            // Assert
            mockReceiver.Verify(m => m.BindAll(), Times.Once);
        }


        [Test]
        public void AddReceiver_AfterStopped_DoesNotBindReceiver()
        {
            // Arrange
            var monitor = new NetMQMonitor(poller);
            monitor.StartMonitoring();
            monitor.StopMonitoring();

            // Act
            monitor.AddReceiver(receiver);

            // Assert
            mockReceiver.Verify(m => m.BindAll(), Times.Never);
        }
        #endregion


        #region AddSender
        [Test]
        public void AddSender_WithNullReceiver_ThrowsArgumentNullException()
        {
            // Arrange
            var monitor = new NetMQMonitor(poller);

            // Act
            TestDelegate add = () => monitor.AddSender(null);

            // Assert
            Assert.That(add, Throws.ArgumentNullException);
        }


        [Test]
        public void AddSender_WithReceiver_AddsPollableSocketToPoller()
        {
            // Arrange
            var monitor = new NetMQMonitor(poller);

            // Act
            monitor.AddSender(sender);

            // Assert
            mockPoller.Verify(m => m.Add(It.IsIn(pollableSocket)), Times.Once);
        }


        [Test]
        public void AddSender_BeforeRunning_DoesNotBindReceiver()
        {
            // Arrange
            var monitor = new NetMQMonitor(poller);

            // Act
            monitor.AddSender(sender);

            // Assert
            mockReceiver.Verify(m => m.BindAll(), Times.Never);
        }


        [Test]
        public void AddSender_AfterStarted_BindsReceiver()
        {
            // Arrange
            var monitor = new NetMQMonitor(poller);
            monitor.StartMonitoring();

            // Act
            monitor.AddSender(sender);

            // Assert
            mockReceiver.Verify(m => m.BindAll(), Times.Once);
        }


        [Test]
        public void AddSender_AfterStopped_DoesNotBindReceiver()
        {
            // Arrange
            var monitor = new NetMQMonitor(poller);
            monitor.StartMonitoring();
            monitor.StopMonitoring();

            // Act
            monitor.AddSender(sender);

            // Assert
            mockReceiver.Verify(m => m.BindAll(), Times.Never);
        }
        #endregion


        #region StartMonitoring
        [Test]
        public void StartMonitoring_BeforeStarted_RunsPoller()
        {
            // Arrange
            var monitor = new NetMQMonitor(poller);

            // Act
            monitor.StartMonitoring();

            // Assert
            mockPoller.Verify(m => m.RunAsync(), Times.Once);
        }


        [Test]
        public void StartMonitor_WhenRunning_DoesNothing()
        {
            // Arrange
            var monitor = new NetMQMonitor(poller);
            monitor.StartMonitoring();
            monitor.AddSender(sender);
            monitor.AddReceiver(receiver);

            mockPoller.ResetCalls();
            mockSender.ResetCalls();
            mockReceiver.ResetCalls();

            // Act
            monitor.StartMonitoring();

            // Assert
            mockPoller.Verify(m => m.RunAsync(), Times.Never);
            mockSender.Verify(m => m.ConnectAll(), Times.Never);
            mockReceiver.Verify(m => m.BindAll(), Times.Never);
        }


        [Test]
        public void StartMonitor_WithSenderAdded_ConnectsSender()
        {
            // Arrange
            var monitor = new NetMQMonitor(poller);
            monitor.AddSender(sender);

            // Act
            monitor.StartMonitoring();

            // Assert
            mockSender.Verify(m => m.ConnectAll(), Times.Once);
        }


        [Test]
        public void StartMonitor_WithReceiverAdded_BindsReceiver()
        {
            // Arrange
            var monitor = new NetMQMonitor(poller);
            monitor.AddReceiver(receiver);

            // Act
            monitor.StartMonitoring();

            // Assert
            mockReceiver.Verify(m => m.BindAll(), Times.Once);
        }
        #endregion


        #region StopMonitoring
        [Test]
        public void StopMonitoring_BeforeStarted_DoesNothing()
        {
            // Arrange
            var monitor = new NetMQMonitor(poller);
            monitor.AddSender(sender);
            monitor.AddReceiver(receiver);

            // Act
            monitor.StopMonitoring();

            // Assert
            mockPoller.Verify(m => m.StopAsync(), Times.Never);
            mockSender.Verify(m => m.DisconnectAll(), Times.Never);
            mockReceiver.Verify(m => m.UnbindAll(), Times.Never);
        }


        [Test]
        public void StopMonitoring_WhenRunning_StopsPoller()
        {
            // Arrange
            var monitor = new NetMQMonitor(poller);
            monitor.StartMonitoring();

            // Act
            monitor.StopMonitoring();

            // Assert
            mockPoller.Verify(m => m.StopAsync(), Times.Once);
        }


        [Test]
        public void StopMonitoring_WithSenderAdded_DisconnectsSender()
        {
            // Arrange
            var monitor = new NetMQMonitor(poller);
            monitor.AddSender(sender);
            monitor.StartMonitoring();

            // Act
            monitor.StopMonitoring();

            // Assert
            mockSender.Verify(m => m.DisconnectAll(), Times.Once);
        }


        [Test]
        public void StopMonitoring_WithReceiverAdded_UnbindsReceiver()
        {
            // Arrange
            var monitor = new NetMQMonitor(poller);
            monitor.AddReceiver(receiver);
            monitor.StartMonitoring();

            // Act
            monitor.StopMonitoring();

            // Assert
            mockReceiver.Verify(m => m.UnbindAll(), Times.Once);
        }
        #endregion
    }
}
