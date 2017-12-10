using MessageRouter.Senders;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessageRouter.UnitTests.Senders
{
    [TestFixture]
    public class MonitorCacheTests
    {
        private readonly Mock<ISenderMonitor> mockSenderMonitor = new Mock<ISenderMonitor>();
        private ISenderMonitor senderMonitor;


        private readonly Mock<ISenderMonitor> mockSenderMonitor2 = new Mock<ISenderMonitor>();
        private ISenderMonitor senderMonitor2;


        [SetUp]
        public void Setup()
        {
            senderMonitor = mockSenderMonitor.Object;
            senderMonitor2 = mockSenderMonitor2.Object;
        }


        [TearDown]
        public void TearDown()
        {
            mockSenderMonitor.Reset();
            mockSenderMonitor2.Reset();
        }


        #region Constructor
        [Test]
        public void MonitorCache_WithNoDependencies_Constructs()
        {
            // Act
            TestDelegate test = () => new MonitorCache();

            // Assert
            Assert.That(test, Throws.Nothing);
        }
        #endregion


        #region AddMonitor
        [Test]
        public void AddMonitor_BeforeStarted_DoesNothing()
        {
            // Arrange
            var monitorCache = new MonitorCache();

            // Act
            monitorCache.AddMonitor(senderMonitor);

            // Assert
            mockSenderMonitor.Verify(m => m.StartSenders(), Times.Never);
        }


        [Test]
        public void AddMonitor_AfterStarted_StartsSenderMonitor()
        {
            // Arrange
            var monitorCache = new MonitorCache();
            monitorCache.StartAllMonitors();

            // Act
            monitorCache.AddMonitor(senderMonitor);

            // Assert
            mockSenderMonitor.Verify(m => m.StartSenders(), Times.Once);
        }
        #endregion


        #region StartAllMonitors
        [Test]
        public void StartAllMonitors_WithNoMonitorsAdded_DoesNothing()
        {
            // Arrange
            var monitorCache = new MonitorCache();

            // Act
            TestDelegate test = () => monitorCache.StartAllMonitors();

            // Assert
            Assert.That(test, Throws.Nothing);
        }


        [Test]
        public void StartAllMonitors_WithMonitorsAdded_StartsTheMonitors()
        {
            // Arrange
            var monitorCache = new MonitorCache();
            monitorCache.AddMonitor(senderMonitor);
            monitorCache.AddMonitor(senderMonitor2);

            // Act
            monitorCache.StartAllMonitors();

            // Assert
            mockSenderMonitor.Verify(m => m.StartSenders(), Times.Once);
            mockSenderMonitor2.Verify(m => m.StartSenders(), Times.Once);
        }


        [Test]
        public void StartAllMonitors_WhenAlreadyStarted_DoesNothing()
        {
            // Arrange
            var monitorCache = new MonitorCache();
            monitorCache.AddMonitor(senderMonitor);
            monitorCache.StartAllMonitors();
            mockSenderMonitor.ResetCalls();

            // Act
            monitorCache.StartAllMonitors();

            // Assert
            mockSenderMonitor.Verify(m => m.StartSenders(), Times.Never);
        }
        #endregion


        #region StopAllMonitors
        [Test]
        public void StopAllMonitors_WithNoMonitorsAdded_DoesNothing()
        {
            // Arrange
            var monitorCache = new MonitorCache();

            // Act
            TestDelegate test = () => monitorCache.StopAllMonitors();

            // Assert
            Assert.That(test, Throws.Nothing);
        }


        [Test]
        public void StopAllMonitors_WhenStarted_StopsTheMonitors()
        {
            // Arrange
            var monitorCache = new MonitorCache();
            monitorCache.AddMonitor(senderMonitor);
            monitorCache.AddMonitor(senderMonitor2);
            monitorCache.StartAllMonitors();

            // Act
            monitorCache.StopAllMonitors();

            // Assert
            mockSenderMonitor.Verify(m => m.StopSenders(), Times.Once);
            mockSenderMonitor2.Verify(m => m.StopSenders(), Times.Once);
        }


        [Test]
        public void StopAllMonitors_BeforeStarted_DoesNothing()
        {
            // Arrange
            var monitorCache = new MonitorCache();
            monitorCache.AddMonitor(senderMonitor);

            // Act
            monitorCache.StopAllMonitors();

            // Assert
            mockSenderMonitor.Verify(m => m.StopSenders(), Times.Never);
        }


        [Test]
        public void StopAllMonitors_AfterStopped_DoesNothing()
        {
            // Arrange
            var monitorCache = new MonitorCache();
            monitorCache.AddMonitor(senderMonitor);
            monitorCache.StartAllMonitors();
            monitorCache.StopAllMonitors();
            mockSenderMonitor.ResetCalls();

            // Act
            monitorCache.StopAllMonitors();

            // Assert
            mockSenderMonitor.Verify(m => m.StopSenders(), Times.Never);
        }
        #endregion
    }
}
