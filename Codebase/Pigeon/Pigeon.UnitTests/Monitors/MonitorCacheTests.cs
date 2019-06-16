using Moq;

using NUnit.Framework;

using Pigeon.Monitors;

namespace Pigeon.UnitTests.Monitors
{
    [TestFixture]
    public class MonitorCacheTests
    {
        private readonly Mock<IMonitor> mockMonitor = new Mock<IMonitor>();
        private IMonitor monitor;
        
        private readonly Mock<IMonitor> mockMonitor2 = new Mock<IMonitor>();
        private IMonitor monitor2;


        [SetUp]
        public void Setup()
        {
            monitor = mockMonitor.Object;
            monitor2 = mockMonitor2.Object;
        }


        [TearDown]
        public void TearDown()
        {
            mockMonitor.Reset();
            mockMonitor2.Reset();
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
            monitorCache.AddMonitor(monitor);

            // Assert
            mockMonitor.Verify(m => m.StartMonitoring(), Times.Never);
        }


        [Test]
        public void AddMonitor_AfterStarted_StartsSenderMonitor()
        {
            // Arrange
            var monitorCache = new MonitorCache();
            monitorCache.StartAllMonitors();

            // Act
            monitorCache.AddMonitor(monitor);

            // Assert
            mockMonitor.Verify(m => m.StartMonitoring(), Times.Once);
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
            monitorCache.AddMonitor(monitor);
            monitorCache.AddMonitor(monitor2);

            // Act
            monitorCache.StartAllMonitors();

            // Assert
            mockMonitor.Verify(m => m.StartMonitoring(), Times.Once);
            mockMonitor2.Verify(m => m.StartMonitoring(), Times.Once);
        }


        [Test]
        public void StartAllMonitors_WhenAlreadyStarted_DoesNothing()
        {
            // Arrange
            var monitorCache = new MonitorCache();
            monitorCache.AddMonitor(monitor);
            monitorCache.StartAllMonitors();
            mockMonitor.Invocations.Clear();

            // Act
            monitorCache.StartAllMonitors();

            // Assert
            mockMonitor.Verify(m => m.StartMonitoring(), Times.Never);
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
            monitorCache.AddMonitor(monitor);
            monitorCache.AddMonitor(monitor2);
            monitorCache.StartAllMonitors();

            // Act
            monitorCache.StopAllMonitors();

            // Assert
            mockMonitor.Verify(m => m.StopMonitoring(), Times.Once);
            mockMonitor2.Verify(m => m.StopMonitoring(), Times.Once);
        }


        [Test]
        public void StopAllMonitors_BeforeStarted_DoesNothing()
        {
            // Arrange
            var monitorCache = new MonitorCache();
            monitorCache.AddMonitor(monitor);

            // Act
            monitorCache.StopAllMonitors();

            // Assert
            mockMonitor.Verify(m => m.StopMonitoring(), Times.Never);
        }


        [Test]
        public void StopAllMonitors_AfterStopped_DoesNothing()
        {
            // Arrange
            var monitorCache = new MonitorCache();
            monitorCache.AddMonitor(monitor);
            monitorCache.StartAllMonitors();
            monitorCache.StopAllMonitors();
            mockMonitor.Invocations.Clear();

            // Act
            monitorCache.StopAllMonitors();

            // Assert
            mockMonitor.Verify(m => m.StopMonitoring(), Times.Never);
        }
        #endregion
    }
}
