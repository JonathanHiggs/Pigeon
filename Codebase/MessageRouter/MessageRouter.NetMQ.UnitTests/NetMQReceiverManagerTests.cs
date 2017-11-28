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
    public class NetMQReceiverManagerTests
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
        public void NetMQReceiverManager_WithNullReceiver_ThrowsArgumentNullException()
        {
            // Act
            TestDelegate test = () => new NetMQReceiverManager(null, poller);

            // Assert
            Assert.That(test, Throws.ArgumentNullException);
        }


        [Test]
        public void NetMQReceiverManager_WithNullPoller_ThrowsArgumentNullException()
        {
            // Act
            TestDelegate test = () => new NetMQReceiverManager(receiver, null);

            // Assert
            Assert.That(test, Throws.ArgumentNullException);
        }


        [Test]
        public void NetMQReceiverManager_WithDependencies_AddsReceiverToPoller()
        {
            // Act
            var receiverManager = new NetMQReceiverManager(receiver, poller);

            // Assert
            mockPoller.Verify(m => m.Add(It.IsAny<ISocketPollable>()), Times.Once);
        }
        #endregion


        #region Start
        [Test]
        public void Start_WhenNotRunning_RunsPoller()
        {
            // Arrange
            var receiverManager = new NetMQReceiverManager(receiver, poller);

            // Act
            receiverManager.Start();

            // Assert
            mockPoller.Verify(m => m.RunAsync(), Times.Once);
        }


        [Test]
        public void Sart_WhenNotRunning_BindsReceiver()
        {
            // Arrange
            var receiverManager = new NetMQReceiverManager(receiver, poller);

            // Act
            receiverManager.Start();

            // Assert
            mockReceiver.Verify(m => m.Bind(), Times.Once);
        }
        #endregion


        #region Stop
        [Test]
        public void Stop_WhenRunning_StopsPoller()
        {
            // Arrange
            var receiverManager = new NetMQReceiverManager(receiver, poller);
            receiverManager.Start();

            // Act
            receiverManager.Stop();

            // Assert
            mockPoller.Verify(m => m.StopAsync(), Times.Once);
        }


        [Test]
        public void Stop_WhenRunning_UnbindsReceiver()
        {
            // Arrange
            var receiverManager = new NetMQReceiverManager(receiver, poller);
            receiverManager.Start();

            // Act
            receiverManager.Stop();

            // Assert
            mockReceiver.Verify(m => m.UnbindAll(), Times.Once);
        }
        #endregion


        #region RequestReceived
        [Test]
        public void RequestReceived_WhenReceiverRaises_IsRaised()
        {
            // Arrange
            var receiverManager = new NetMQReceiverManager(receiver, poller);
            var called = false;
            receiverManager.RequestReceived += (sender, task) => called = true;

            // Act
            mockReceiver.Raise(r => r.RequestReceived += null, receiver, new RequestTask());

            // Assert
            Assert.That(called, Is.True);
        }
        #endregion
    }
}
