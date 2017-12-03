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


        [Test]
        public void MonitorCache_WithNoDependencies_Constructs()
        {
            // Act
            TestDelegate test = () => new MonitorCache();

            // Assert
            Assert.That(test, Throws.Nothing);
        }


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
        public void StopAllMonitors_WithMonitorsAdded_StopsTheMonitors()
        {
            // Arrange
            var monitorCache = new MonitorCache();
            monitorCache.AddMonitor(senderMonitor);
            monitorCache.AddMonitor(senderMonitor2);

            // Act
            monitorCache.StopAllMonitors();

            // Assert
            mockSenderMonitor.Verify(m => m.StopSenders(), Times.Once);
            mockSenderMonitor2.Verify(m => m.StopSenders(), Times.Once);
        }
        #endregion
    }
}
