using Moq;

using NetMQ;

using NUnit.Framework;

using Pigeon.NetMQ.Publishers;
using Pigeon.NetMQ.Receivers;
using Pigeon.NetMQ.Senders;
using Pigeon.NetMQ.Subscribers;
using Pigeon.Topics;

namespace Pigeon.NetMQ.UnitTests
{
    [TestFixture]
    public class NetMQMonitorTests
    {
        private readonly Mock<INetMQPoller> mockPoller = new Mock<INetMQPoller>();
        private INetMQPoller poller;

        private readonly Mock<ITopicDispatcher> mockTopicDispatcher = new Mock<ITopicDispatcher>();
        private ITopicDispatcher topicDispatcher;
        
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

        private readonly string request = "hello";
        

        #region Setup

        [SetUp]
        public void Setup()
        {
            poller = mockPoller.Object;
            topicDispatcher = mockTopicDispatcher.Object;
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

        #endregion


        #region Constructor

        [Test]
        public void NetMQMonitor_WithNullPoller_ThrowsArgumentNullException()
        {
            // Act
            TestDelegate construct = () => new NetMQMonitor(null, topicDispatcher);

            // Assert
            Assert.That(construct, Throws.ArgumentNullException);
        }


        [Test]
        public void NetMQMonitor_WithNullTopicDispatcher_ThrowsArgumentNullException()
        {
            // Act
            TestDelegate construct = () => new NetMQMonitor(poller, null);

            // Assert
            Assert.That(construct, Throws.ArgumentNullException);
        }
        
        #endregion


        #region AddSender

        [Test]
        public void AddSender_WithNullReceiver_DoesNotThrow()
        {
            // Arrange
            var monitor = new NetMQMonitor(poller, topicDispatcher);

            // Act
            TestDelegate add = () => monitor.AddSender(null);

            // Assert
            Assert.That(add, Throws.Nothing);
        }


        [Test]
        public void AddSender_WithReceiver_AddsPollableSocketToPoller()
        {
            // Arrange
            var monitor = new NetMQMonitor(poller, topicDispatcher);

            // Act
            monitor.AddSender(sender);

            // Assert
            mockPoller.Verify(m => m.Add(It.IsIn(pollableSocket)), Times.Once);
        }


        [Test]
        public void AddSender_BeforeRunning_DoesNotBindReceiver()
        {
            // Arrange
            var monitor = new NetMQMonitor(poller, topicDispatcher);

            // Act
            monitor.AddSender(sender);

            // Assert
            mockReceiver.Verify(m => m.InitializeConnection(), Times.Never);
        }


        [Test]
        public void AddSender_AfterStarted_ConnectsSender()
        {
            // Arrange
            var monitor = new NetMQMonitor(poller, topicDispatcher);
            monitor.StartMonitoring();

            // Act
            monitor.AddSender(sender);

            // Assert
            mockSender.Verify(m => m.InitializeConnection(), Times.Once);
        }


        [Test]
        public void AddSender_AfterStopped_DoesNotBindReceiver()
        {
            // Arrange
            var monitor = new NetMQMonitor(poller, topicDispatcher);
            monitor.StartMonitoring();
            monitor.StopMonitoring();

            // Act
            monitor.AddSender(sender);

            // Assert
            mockReceiver.Verify(m => m.InitializeConnection(), Times.Never);
        }
        
        #endregion


        #region AddReceiver

        [Test]
        public void AddReceiver_WithNullReceiver_DoesNotThrow()
        {
            // Arrange
            var monitor = new NetMQMonitor(poller, topicDispatcher);

            // Act
            TestDelegate add = () => monitor.AddReceiver(null);

            // Assert
            Assert.That(add, Throws.Nothing);
        }


        [Test]
        public void AddReceiver_WithReceiver_AddsPollableSocketToPoller()
        {
            // Arrange
            var monitor = new NetMQMonitor(poller, topicDispatcher);

            // Act
            monitor.AddReceiver(receiver);

            // Assert
            mockPoller.Verify(m => m.Add(It.IsIn(pollableSocket)), Times.Once);
        }


        [Test]
        public void AddReceiver_BeforeRunning_DoesNotBindReceiver()
        {
            // Arrange
            var monitor = new NetMQMonitor(poller, topicDispatcher);

            // Act
            monitor.AddReceiver(receiver);

            // Assert
            mockReceiver.Verify(m => m.InitializeConnection(), Times.Never);
        }


        [Test]
        public void AddReceiver_AfterStarted_BindsReceiver()
        {
            // Arrange
            var monitor = new NetMQMonitor(poller, topicDispatcher);
            monitor.StartMonitoring();

            // Act
            monitor.AddReceiver(receiver);

            // Assert
            mockReceiver.Verify(m => m.InitializeConnection(), Times.Once);
        }


        [Test]
        public void AddReceiver_AfterStopped_DoesNotBindReceiver()
        {
            // Arrange
            var monitor = new NetMQMonitor(poller, topicDispatcher);
            monitor.StartMonitoring();
            monitor.StopMonitoring();

            // Act
            monitor.AddReceiver(receiver);

            // Assert
            mockReceiver.Verify(m => m.InitializeConnection(), Times.Never);
        }
        
        #endregion


        #region AddPublisher

        [Test]
        public void AddPublisher_WithNullReceiver_DoesNotThrow()
        {
            // Arrange
            var monitor = new NetMQMonitor(poller, topicDispatcher);

            // Act
            TestDelegate add = () => monitor.AddPublisher(null);

            // Assert
            Assert.That(add, Throws.Nothing);
        }


        [Test]
        public void AddPublisher_WithReceiver_AddsPollableSocketToPoller()
        {
            // Arrange
            var monitor = new NetMQMonitor(poller, topicDispatcher);

            // Act
            monitor.AddPublisher(publisher);

            // Assert
            mockPoller.Verify(m => m.Add(It.IsIn(pollableSocket)), Times.Once);
        }


        [Test]
        public void AddPublisher_BeforeRunning_DoesNotBindReceiver()
        {
            // Arrange
            var monitor = new NetMQMonitor(poller, topicDispatcher);

            // Act
            monitor.AddPublisher(publisher);

            // Assert
            mockPublisher.Verify(m => m.InitializeConnection(), Times.Never);
        }


        [Test]
        public void AddPublisher_AfterStarted_BindsReceiver()
        {
            // Arrange
            var monitor = new NetMQMonitor(poller, topicDispatcher);
            monitor.StartMonitoring();

            // Act
            monitor.AddPublisher(publisher);

            // Assert
            mockPublisher.Verify(m => m.InitializeConnection(), Times.Once);
        }


        [Test]
        public void AddPublisher_AfterStopped_DoesNotBindReceiver()
        {
            // Arrange
            var monitor = new NetMQMonitor(poller, topicDispatcher);
            monitor.StartMonitoring();
            monitor.StopMonitoring();

            // Act
            monitor.AddPublisher(publisher);

            // Assert
            mockPublisher.Verify(m => m.InitializeConnection(), Times.Never);
        }
        
        #endregion


        #region AddSubscriber

        [Test]
        public void AddSubscriber_WithNullReceiver_DoesNotThrow()
        {
            // Arrange
            var monitor = new NetMQMonitor(poller, topicDispatcher);

            // Act
            TestDelegate add = () => monitor.AddSubscriber(null);

            // Assert
            Assert.That(add, Throws.Nothing);
        }


        [Test]
        public void AddSubscriber_WithReceiver_AddsPollableSocketToPoller()
        {
            // Arrange
            var monitor = new NetMQMonitor(poller, topicDispatcher);

            // Act
            monitor.AddSubscriber(subscriber);

            // Assert
            mockPoller.Verify(m => m.Add(It.IsIn(pollableSocket)), Times.Once);
        }


        [Test]
        public void AddSubscriber_BeforeRunning_DoesNotBindReceiver()
        {
            // Arrange
            var monitor = new NetMQMonitor(poller, topicDispatcher);

            // Act
            monitor.AddSubscriber(subscriber);

            // Assert
            mockSubscriber.Verify(m => m.InitializeConnection(), Times.Never);
        }


        [Test]
        public void AddSubscriber_AfterStarted_BindsReceiver()
        {
            // Arrange
            var monitor = new NetMQMonitor(poller, topicDispatcher);
            monitor.StartMonitoring();

            // Act
            monitor.AddSubscriber(subscriber);

            // Assert
            mockSubscriber.Verify(m => m.InitializeConnection(), Times.Once);
        }


        [Test]
        public void AddSubscriber_AfterStopped_DoesNotBindReceiver()
        {
            // Arrange
            var monitor = new NetMQMonitor(poller, topicDispatcher);
            monitor.StartMonitoring();
            monitor.StopMonitoring();

            // Act
            monitor.AddSubscriber(subscriber);

            // Assert
            mockSubscriber.Verify(m => m.InitializeConnection(), Times.Never);
        }
        
        #endregion


        #region StartMonitoring

        [Test]
        public void StartMonitoring_BeforeStarted_RunsPoller()
        {
            // Arrange
            var monitor = new NetMQMonitor(poller, topicDispatcher);

            // Act
            monitor.StartMonitoring();

            // Assert
            mockPoller.Verify(m => m.RunAsync(), Times.Once);
        }


        [Test]
        public void StartMonitor_WhenRunning_DoesNotRecallStartMethods()
        {
            // Arrange
            var monitor = new NetMQMonitor(poller, topicDispatcher);
            monitor.StartMonitoring();
            monitor.AddSender(sender);
            monitor.AddReceiver(receiver);
            monitor.AddPublisher(publisher);
            monitor.AddSubscriber(subscriber);

            // Act
            monitor.StartMonitoring();

            // Assert
            mockPoller.Verify(m => m.RunAsync(), Times.Once);
            mockSender.Verify(m => m.InitializeConnection(), Times.Once);
            mockReceiver.Verify(m => m.InitializeConnection(), Times.Once);
            mockPublisher.Verify(m => m.InitializeConnection(), Times.Once);
            mockSubscriber.Verify(m => m.InitializeConnection(), Times.Once);
        }


        [Test]
        public void StartMonitor_WithSenderAdded_ConnectsSender()
        {
            // Arrange
            var monitor = new NetMQMonitor(poller, topicDispatcher);
            monitor.AddSender(sender);

            // Act
            monitor.StartMonitoring();

            // Assert
            mockSender.Verify(m => m.InitializeConnection(), Times.Once);
        }


        [Test]
        public void StartMonitor_WithReceiverAdded_BindsReceiver()
        {
            // Arrange
            var monitor = new NetMQMonitor(poller, topicDispatcher);
            monitor.AddReceiver(receiver);

            // Act
            monitor.StartMonitoring();

            // Assert
            mockReceiver.Verify(m => m.InitializeConnection(), Times.Once);
        }


        [Test]
        public void StartMonitor_WithPublisherAdded_BindsReceiver()
        {
            // Arrange
            var monitor = new NetMQMonitor(poller, topicDispatcher);
            monitor.AddPublisher(publisher);

            // Act
            monitor.StartMonitoring();

            // Assert
            mockPublisher.Verify(m => m.InitializeConnection(), Times.Once);
        }


        [Test]
        public void StartMonitor_WithSubscriberAdded_ConnectsReceiver()
        {
            // Arrange
            var monitor = new NetMQMonitor(poller, topicDispatcher);
            monitor.AddSubscriber(subscriber);

            // Act
            monitor.StartMonitoring();

            // Assert
            mockSubscriber.Verify(m => m.InitializeConnection(), Times.Once);
        }
        
        #endregion


        #region StopMonitoring

        [Test]
        public void StopMonitoring_BeforeStarted_DoesNothing()
        {
            // Arrange
            var monitor = new NetMQMonitor(poller, topicDispatcher);
            monitor.AddSender(sender);
            monitor.AddReceiver(receiver);
            monitor.AddPublisher(publisher);
            monitor.AddSubscriber(subscriber);

            // Act
            monitor.StopMonitoring();

            // Assert
            mockPoller.Verify(m => m.StopAsync(), Times.Never);
            mockSender.Verify(m => m.TerminateConnection(), Times.Never);
            mockReceiver.Verify(m => m.TerminateConnection(), Times.Never);
            mockPublisher.Verify(m => m.InitializeConnection(), Times.Never);
            mockSubscriber.Verify(m => m.InitializeConnection(), Times.Never);
        }


        [Test]
        public void StopMonitoring_WhenRunning_StopsPoller()
        {
            // Arrange
            var monitor = new NetMQMonitor(poller, topicDispatcher);
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
            var monitor = new NetMQMonitor(poller, topicDispatcher);
            monitor.AddSender(sender);
            monitor.StartMonitoring();

            // Act
            monitor.StopMonitoring();

            // Assert
            mockSender.Verify(m => m.TerminateConnection(), Times.Once);
        }


        [Test]
        public void StopMonitoring_WithReceiverAdded_UnbindsReceiver()
        {
            // Arrange
            var monitor = new NetMQMonitor(poller, topicDispatcher);
            monitor.AddReceiver(receiver);
            monitor.StartMonitoring();

            // Act
            monitor.StopMonitoring();

            // Assert
            mockReceiver.Verify(m => m.TerminateConnection(), Times.Once);
        }


        [Test]
        public void StopMonitoring_WithPublisherAdded_UnbindsReceiver()
        {
            // Arrange
            var monitor = new NetMQMonitor(poller, topicDispatcher);
            monitor.AddPublisher(publisher);
            monitor.StartMonitoring();

            // Act
            monitor.StopMonitoring();

            // Assert
            mockPublisher.Verify(m => m.TerminateConnection(), Times.Once);
        }


        [Test]
        public void StopMonitoring_WithSubscriberAdded_UnbindsReceiver()
        {
            // Arrange
            var monitor = new NetMQMonitor(poller, topicDispatcher);
            monitor.AddSubscriber(subscriber);
            monitor.StartMonitoring();

            // Act
            monitor.StopMonitoring();

            // Assert
            mockSubscriber.Verify(m => m.TerminateConnection(), Times.Once);
        }
        
        #endregion


        #region TopicHandler

        [Test]
        public void TopicHandler_WithTopicPackage_CallsTopicDispatcher()
        {
            // Arrange
            var monitor = new NetMQMonitor(poller, topicDispatcher);

            // Act
            monitor.TopicHandler(subscriber, request);

            // Assert
            mockTopicDispatcher.Verify(m => m.Handle(It.IsIn(request)), Times.Once);
        }
        
        #endregion
    }
}
