using MessageRouter.Addresses;
using MessageRouter.Receivers;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessageRouter.UnitTests.Receivers
{
    [TestFixture]
    public class ReceiverManagerTests
    {
        private readonly Mock<IReceiver> mockReceiver = new Mock<IReceiver>();
        private IReceiver receiver;


        [SetUp]
        public void Setup()
        {
            receiver = mockReceiver.Object;
        }


        [TearDown]
        public void TearDown()
        {
            mockReceiver.Reset();
        }


        #region Constructor
        [Test]
        public void ReceiverManager_WithNullReceiver_ThrowsArgumentNullException()
        {
            // Act
            TestDelegate test = () => new ReceiverManager(null);

            // Assert
            Assert.That(test, Throws.ArgumentNullException);
        }
        #endregion


        #region Receive
        [Test]
        public void Receive_WhenStarted_CallsReceiver()
        {
            // Arrange
            var receiverManager = new ReceiverManager(receiver);
            receiverManager.Start();

            // Act
            receiverManager.Receive();

            // Assert
            mockReceiver.Verify(m => m.Receive(), Times.Once);
        }


        [Test]
        public void Receive_BeforeStarted_ThrowsInvalidOperationException()
        {
            // Arrange
            var receiverManager = new ReceiverManager(receiver);

            // Act
            TestDelegate test = () => receiverManager.Receive();

            // Assert
            Assert.That(test, Throws.InvalidOperationException);
        }


        [Test]
        public void Receive_AfterStopped_ThrowsInvalidOperationException()
        {
            // Arrange
            var receiverManager = new ReceiverManager(receiver);
            receiverManager.Start();
            receiverManager.Stop();

            // Act
            TestDelegate test = () => receiverManager.Receive();

            // Assert
            Assert.That(test, Throws.InvalidOperationException);
        }
        #endregion


        #region TryReceive
        [Test]
        public void TryReceive_WhenStarted_CallsReceiver()
        {
            // Arrange
            var receiverManager = new ReceiverManager(receiver);
            receiverManager.Start();

            // Act
            receiverManager.TryReceive(out var requestTask);

            // Assert
            RequestTask rt;
            mockReceiver.Verify(m => m.TryReceive(out rt), Times.Once);
        }


        [Test]
        public void TryReceive_BeforeStarted_ThrowsInvalidOperationException()
        {
            // Arrange
            var receiverManager = new ReceiverManager(receiver);

            // Act
            TestDelegate test = () => receiverManager.TryReceive(out var requestTask);

            // Assert
            Assert.That(test, Throws.InvalidOperationException);
        }


        [Test]
        public void TryReceive_AfterStopped_ThrowsInvalidOperationException()
        {
            // Arrange
            var receiverManager = new ReceiverManager(receiver);
            receiverManager.Start();
            receiverManager.Stop();

            // Act
            TestDelegate test = () => receiverManager.TryReceive(out var requestTask);

            // Assert
            Assert.That(test, Throws.InvalidOperationException);
        }
        #endregion


        #region Start
        [Test]
        public void Start_BindsReceiver()
        {
            // Arrange
            var receiverManager = new ReceiverManager(receiver);

            // Act
            receiverManager.Start();

            // Assert
            mockReceiver.Verify(m => m.Bind(), Times.Once);
        }


        [Test]
        public void Start_WhenAlreadyStarted_DoesNothing()
        {
            // Arrange
            var receiverManager = new ReceiverManager(receiver);
            receiverManager.Start();

            // Act
            receiverManager.Start();

            // Assert
            mockReceiver.Verify(m => m.Bind(), Times.Once);
        }
        #endregion


        #region Stop
        [Test]
        public void Stop_BeforeStarted_DoesNothing()
        {
            // Arrange
            var receiverManager = new ReceiverManager(receiver);

            // Act
            TestDelegate test = () => receiverManager.Stop();

            // Assert
            Assert.That(test, Throws.Nothing);
        }


        [Test]
        public void Stop_WhenStarted_CallsUnbindAll()
        {
            // Arrange
            var receiverManager = new ReceiverManager(receiver);
            receiverManager.Start();

            // Act
            receiverManager.Stop();

            // Assert
            mockReceiver.Verify(m => m.UnbindAll(), Times.Once);
        }


        [Test]
        public void Stop_WhenStopped_DoesNothing()
        {
            // Arrange
            var receiverManager = new ReceiverManager(receiver);
            receiverManager.Start();
            receiverManager.Stop();

            // Act
            receiverManager.Stop();

            // Assert
            mockReceiver.Verify(m => m.UnbindAll(), Times.Once);
        }
        #endregion
    }
}
