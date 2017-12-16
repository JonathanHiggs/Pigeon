using MessageRouter.NetMQ.Publishers;
using MessageRouter.NetMQ.Receivers;
using MessageRouter.NetMQ.Senders;
using MessageRouter.NetMQ.Subscribers;
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

        private readonly Mock<INetMQPublisher> mockPublisher = new Mock<INetMQPublisher>();
        private INetMQPublisher publisher;

        private readonly Mock<INetMQSubscriber> mockSubscriber = new Mock<INetMQSubscriber>();
        private INetMQSubscriber subscriber;
        
        private readonly Mock<ISocketPollable> mockPollableSocket = new Mock<ISocketPollable>();
        private ISocketPollable pollableSocket;
        

        [SetUp]
        public void Setup()
        {
            poller = mockPoller.Object;
            sender = mockSender.Object;
            receiver = mockReceiver.Object;
            publisher = mockPublisher.Object;
            subscriber = mockSubscriber.Object;
            pollableSocket = mockPollableSocket.Object;

            mockSender
                .SetupGet(m => m.PollableSocket)
                .Returns(pollableSocket);

            mockReceiver
                .SetupGet(m => m.PollableSocket)
                .Returns(pollableSocket);

            mockPublisher
                .SetupGet(m => m.PollableSocket)
                .Returns(pollableSocket);

            mockSubscriber
                .SetupGet(m => m.PollableSocket)
                .Returns(pollableSocket);
        }


        [TearDown]
        public void TearDown()
        {
            mockPoller.Reset();
            mockSender.Reset();
            mockReceiver.Reset();
            mockPublisher.Reset();
            mockSubscriber.Reset();
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


        #region AddSender
        [Test]
        public void AddSender_WithNullReceiver_DoesNotThrow()
        {
            // Arrange
            var monitor = new NetMQMonitor(poller);

            // Act
            TestDelegate add = () => monitor.AddSender(null);

            // Assert
            Assert.That(add, Throws.Nothing);
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
        public void AddSender_AfterStarted_ConnectsSender()
        {
            // Arrange
            var monitor = new NetMQMonitor(poller);
            monitor.StartMonitoring();

            // Act
            monitor.AddSender(sender);

            // Assert
            mockSender.Verify(m => m.ConnectAll(), Times.Once);
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


        #region AddReceiver
        [Test]
        public void AddReceiver_WithNullReceiver_DoesNotThrow()
        {
            // Arrange
            var monitor = new NetMQMonitor(poller);

            // Act
            TestDelegate add = () => monitor.AddReceiver(null);

            // Assert
            Assert.That(add, Throws.Nothing);
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


        #region AddPublisher
        [Test]
        public void AddPublisher_WithNullReceiver_DoesNotThrow()
        {
            // Arrange
            var monitor = new NetMQMonitor(poller);

            // Act
            TestDelegate add = () => monitor.AddPublisher(null);

            // Assert
            Assert.That(add, Throws.Nothing);
        }


        [Test]
        public void AddPublisher_WithReceiver_AddsPollableSocketToPoller()
        {
            // Arrange
            var monitor = new NetMQMonitor(poller);

            // Act
            monitor.AddPublisher(publisher);

            // Assert
            mockPoller.Verify(m => m.Add(It.IsIn(pollableSocket)), Times.Once);
        }


        [Test]
        public void AddPublisher_BeforeRunning_DoesNotBindReceiver()
        {
            // Arrange
            var monitor = new NetMQMonitor(poller);

            // Act
            monitor.AddPublisher(publisher);

            // Assert
            mockPublisher.Verify(m => m.BindAll(), Times.Never);
        }


        [Test]
        public void AddPublisher_AfterStarted_BindsReceiver()
        {
            // Arrange
            var monitor = new NetMQMonitor(poller);
            monitor.StartMonitoring();

            // Act
            monitor.AddPublisher(publisher);

            // Assert
            mockPublisher.Verify(m => m.BindAll(), Times.Once);
        }


        [Test]
        public void AddPublisher_AfterStopped_DoesNotBindReceiver()
        {
            // Arrange
            var monitor = new NetMQMonitor(poller);
            monitor.StartMonitoring();
            monitor.StopMonitoring();

            // Act
            monitor.AddPublisher(publisher);

            // Assert
            mockPublisher.Verify(m => m.BindAll(), Times.Never);
        }
        #endregion


        #region AddSubscriber
        [Test]
        public void AddSubscriber_WithNullReceiver_DoesNotThrow()
        {
            // Arrange
            var monitor = new NetMQMonitor(poller);

            // Act
            TestDelegate add = () => monitor.AddSubscriber(null);

            // Assert
            Assert.That(add, Throws.Nothing);
        }


        [Test]
        public void AddSubscriber_WithReceiver_AddsPollableSocketToPoller()
        {
            // Arrange
            var monitor = new NetMQMonitor(poller);

            // Act
            monitor.AddSubscriber(subscriber);

            // Assert
            mockPoller.Verify(m => m.Add(It.IsIn(pollableSocket)), Times.Once);
        }


        [Test]
        public void AddSubscriber_BeforeRunning_DoesNotBindReceiver()
        {
            // Arrange
            var monitor = new NetMQMonitor(poller);

            // Act
            monitor.AddSubscriber(subscriber);

            // Assert
            mockSubscriber.Verify(m => m.ConnectAll(), Times.Never);
        }


        [Test]
        public void AddSubscriber_AfterStarted_BindsReceiver()
        {
            // Arrange
            var monitor = new NetMQMonitor(poller);
            monitor.StartMonitoring();

            // Act
            monitor.AddSubscriber(subscriber);

            // Assert
            mockSubscriber.Verify(m => m.ConnectAll(), Times.Once);
        }


        [Test]
        public void AddSubscriber_AfterStopped_DoesNotBindReceiver()
        {
            // Arrange
            var monitor = new NetMQMonitor(poller);
            monitor.StartMonitoring();
            monitor.StopMonitoring();

            // Act
            monitor.AddSubscriber(subscriber);

            // Assert
            mockSubscriber.Verify(m => m.ConnectAll(), Times.Never);
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
        public void StartMonitor_WhenRunning_DoesNotRecallStartMethods()
        {
            // Arrange
            var monitor = new NetMQMonitor(poller);
            monitor.StartMonitoring();
            monitor.AddSender(sender);
            monitor.AddReceiver(receiver);
            monitor.AddPublisher(publisher);
            monitor.AddSubscriber(subscriber);

            // Act
            monitor.StartMonitoring();

            // Assert
            mockPoller.Verify(m => m.RunAsync(), Times.Once);
            mockSender.Verify(m => m.ConnectAll(), Times.Once);
            mockReceiver.Verify(m => m.BindAll(), Times.Once);
            mockPublisher.Verify(m => m.BindAll(), Times.Once);
            mockSubscriber.Verify(m => m.ConnectAll(), Times.Once);
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


        [Test]
        public void StartMonitor_WithPublisherAdded_BindsReceiver()
        {
            // Arrange
            var monitor = new NetMQMonitor(poller);
            monitor.AddPublisher(publisher);

            // Act
            monitor.StartMonitoring();

            // Assert
            mockPublisher.Verify(m => m.BindAll(), Times.Once);
        }


        [Test]
        public void StartMonitor_WithSubscriberAdded_ConnectsReceiver()
        {
            // Arrange
            var monitor = new NetMQMonitor(poller);
            monitor.AddSubscriber(subscriber);

            // Act
            monitor.StartMonitoring();

            // Assert
            mockSubscriber.Verify(m => m.ConnectAll(), Times.Once);
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
            monitor.AddPublisher(publisher);
            monitor.AddSubscriber(subscriber);

            // Act
            monitor.StopMonitoring();

            // Assert
            mockPoller.Verify(m => m.StopAsync(), Times.Never);
            mockSender.Verify(m => m.DisconnectAll(), Times.Never);
            mockReceiver.Verify(m => m.UnbindAll(), Times.Never);
            mockPublisher.Verify(m => m.BindAll(), Times.Never);
            mockSubscriber.Verify(m => m.ConnectAll(), Times.Never);
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


        [Test]
        public void StopMonitoring_WithPublisherAdded_UnbindsReceiver()
        {
            // Arrange
            var monitor = new NetMQMonitor(poller);
            monitor.AddPublisher(publisher);
            monitor.StartMonitoring();

            // Act
            monitor.StopMonitoring();

            // Assert
            mockPublisher.Verify(m => m.UnbindAll(), Times.Once);
        }


        [Test]
        public void StopMonitoring_WithSubscriberAdded_UnbindsReceiver()
        {
            // Arrange
            var monitor = new NetMQMonitor(poller);
            monitor.AddSubscriber(subscriber);
            monitor.StartMonitoring();

            // Act
            monitor.StopMonitoring();

            // Assert
            mockSubscriber.Verify(m => m.DisconnectAll(), Times.Once);
        }
        #endregion
    }
}
