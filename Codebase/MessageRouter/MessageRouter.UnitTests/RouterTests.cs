using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MessageRouter.Messages;
using MessageRouter.Monitors;
using MessageRouter.Receivers;
using MessageRouter.Senders;
using MessageRouter.Server;
using Moq;
using NUnit.Framework;

namespace MessageRouter.UnitTests
{
    [TestFixture]
    public class RouterTests
    {
        private readonly Mock<ISenderCache> mockSenderCache = new Mock<ISenderCache>();
        private ISenderCache senderCache;

        private readonly Mock<IMonitorCache> mockMonitorCache = new Mock<IMonitorCache>();
        private IMonitorCache monitorCache;

        private readonly Mock<IMessageFactory> mockMessageFactory = new Mock<IMessageFactory>();
        private IMessageFactory messageFactory;

        private readonly Mock<IReceiverMonitor> mockReceiverMonitor = new Mock<IReceiverMonitor>();
        private IReceiverMonitor receiverMonitor;

        private readonly Mock<IRequestDispatcher> mockRequestDispatcher = new Mock<IRequestDispatcher>();
        private IRequestDispatcher requestDispatcher;
        
        private readonly string name = "some name";


        [SetUp]
        public void Setup()
        {
            senderCache = mockSenderCache.Object;
            monitorCache = mockMonitorCache.Object;
            messageFactory = mockMessageFactory.Object;
            receiverMonitor = mockReceiverMonitor.Object;
            requestDispatcher = mockRequestDispatcher.Object;
        }


        [TearDown]
        public void TearDown()
        {
            mockSenderCache.Reset();
            mockMonitorCache.Reset();
            mockMessageFactory.Reset();
            mockReceiverMonitor.Reset();
            mockRequestDispatcher.Reset();
        }
        

        public void SetupMirroredHandle<T>() where T : class
        {
            mockMessageFactory
                .Setup(m => m.ExtractRequest(It.IsAny<DataMessage<T>>()))
                .Returns<DataMessage<T>>(m => m.Body);

            mockMessageFactory
                .Setup(m => m.CreateResponse<T>(It.IsAny<T>()))
                .Returns<T>(t => new DataMessage<T>(new GuidMessageId(), t));

            mockRequestDispatcher
                .Setup(m => m.Handle(It.IsAny<object>()))
                .Returns<object>(o => o);
        }


        #region Constructor
        [Test]
        public void Router_WithNullName_ThrowsArgumentNullException()
        {
            // Act
            TestDelegate construct = () => new Router(null, senderCache, monitorCache, messageFactory, receiverMonitor, requestDispatcher);

            // Assert
            Assert.That(construct, Throws.ArgumentNullException);
        }


        [Test]
        public void Router_WitNullSenderCache_ThrowsArgumentNullException()
        {
            // Act
            TestDelegate construct = () => new Router(name, null, monitorCache, messageFactory, receiverMonitor, requestDispatcher);

            // Assert
            Assert.That(construct, Throws.ArgumentNullException);
        }


        [Test]
        public void Router_WithNullMonitorCache_ThrowsArgumentNullException()
        {
            // Act
            TestDelegate construct = () => new Router(name, senderCache, null, messageFactory, receiverMonitor, requestDispatcher);

            // Assert
            Assert.That(construct, Throws.ArgumentNullException);
        }


        [Test]
        public void Router_WithNullMessageFactory_ThrowsArgumentNullException()
        {
            // Act
            TestDelegate construct = () => new Router(name, senderCache, monitorCache, null, receiverMonitor, requestDispatcher);

            // Assert
            Assert.That(construct, Throws.ArgumentNullException);
        }


        [Test]
        public void Router_WithNullReceiverMonitor_ThrowsArgumentNullException()
        {
            // Act
            TestDelegate construct = () => new Router(name, senderCache, monitorCache, messageFactory, null, requestDispatcher);

            // Assert
            Assert.That(construct, Throws.ArgumentNullException);
        }


        [Test]
        public void Router_WithNullRequestDispatcher_ThrowsArgumentNullException()
        {
            // Act
            TestDelegate construct = () => new Router(name, senderCache, monitorCache, messageFactory, receiverMonitor, null);

            // Assert
            Assert.That(construct, Throws.ArgumentNullException);
        }
        #endregion


        #region Info
        [Test]
        public void Info_WithName_RetunsSameName()
        {
            // Arrange
            var router = new Router(name, senderCache, monitorCache, messageFactory, receiverMonitor, requestDispatcher);

            // Act
            var infoName = router.Info.Name;

            // Assert
            Assert.That(infoName, Is.EqualTo(name));
        }


        [Test]
        public void Info_BeforeStart_HasNoStartTimestamp()
        {
            // Arrange
            var router = new Router(name, senderCache, monitorCache, messageFactory, receiverMonitor, requestDispatcher);

            // Act
            var startTimestamp = router.Info.StartedTimestamp.HasValue;

            // Assert
            Assert.That(startTimestamp, Is.False);
        }


        [Test]
        public void Info_BeforeStart_HasNoStopTimestamp()
        {
            // Arrange
            var router = new Router(name, senderCache, monitorCache, messageFactory, receiverMonitor, requestDispatcher);

            // Act
            var stopTimestamp = router.Info.StoppedTimestamp.HasValue;

            // Assert
            Assert.That(stopTimestamp, Is.False);
        }


        [Test]
        public void Info_BeforeStart_IsNotRunning()
        {
            // Arrange
            var router = new Router(name, senderCache, monitorCache, messageFactory, receiverMonitor, requestDispatcher);

            // Act
            var running = router.Info.Running;

            // Assert
            Assert.That(running, Is.False);
        }
        #endregion


        #region Send
        [Test]
        public async Task Send_WithNoTimeout_CallsSenderCache()
        {
            // Arrange
            var router = new Router(name, senderCache, monitorCache, messageFactory, receiverMonitor, requestDispatcher);
            mockSenderCache
                .Setup(m => m.Send<string, string>(It.IsAny<string>()))
                .ReturnsAsync("something");
            
            // Act
            var response = await router.Send<string, string>("something");

            // Assert
            mockSenderCache
                .Verify(
                    m => m.Send<string, string>(It.IsAny<string>()), 
                    Times.Once);
        }


        [Test]
        public async Task Send_WithTimeout_CallsSenderCache()
        {
            // Arrange
            var router = new Router(name, senderCache, monitorCache, messageFactory, receiverMonitor, requestDispatcher);
            mockSenderCache
                .Setup(m => m.Send<string, string>(It.IsAny<string>()))
                .ReturnsAsync("something");
            
            // Act
            var response = await router.Send<string, string>("something", TimeSpan.FromMilliseconds(10));

            // Assert
            mockSenderCache
                .Verify(
                    m => m.Send<string, string>(It.IsAny<string>(), It.IsAny<TimeSpan>()), 
                    Times.Once);
        }
        #endregion


        #region Start
        [Test]
        public void Start_BeforeStarted_StartsMonitors()
        {
            // Arrange
            var router = new Router(name, senderCache, monitorCache, messageFactory, receiverMonitor, requestDispatcher);

            // Act
            router.Start();

            // Assert
            mockMonitorCache.Verify(m => m.StartAllMonitors(), Times.Once);
            mockReceiverMonitor.Verify(m => m.StartReceivers(), Times.Once);
        }


        [Test]
        public void Start_WhenAlreadyStarted_DoesNothing()
        {
            // Arrange
            var router = new Router(name, senderCache, monitorCache, messageFactory, receiverMonitor, requestDispatcher);
            router.Start();
            mockMonitorCache.ResetCalls();
            mockReceiverMonitor.ResetCalls();

            // Act
            router.Start();

            // Assert
            mockMonitorCache.Verify(m => m.StartAllMonitors(), Times.Never);
            mockReceiverMonitor.Verify(m => m.StartReceivers(), Times.Never);
        }


        [Test]
        public void Start_BeforeStarted_SetsStartedTimestamp()
        {
            // Arrange
            var router = new Router(name, senderCache, monitorCache, messageFactory, receiverMonitor, requestDispatcher);

            // Act
            router.Start();

            // Assert
            Assert.That(router.Info.StartedTimestamp.HasValue, Is.True);
        }


        [Test]
        public void Start_WhenAlreadyStarted_DoesNotChangeStartedTimestamp()
        {
            // Arrange
            var router = new Router(name, senderCache, monitorCache, messageFactory, receiverMonitor, requestDispatcher);
            router.Start();
            var startedTimestamp = router.Info.StartedTimestamp.Value;

            // Act
            router.Start();

            // Assert
            Assert.That(router.Info.StartedTimestamp.Value, Is.EqualTo(startedTimestamp));
        }
        #endregion


        #region Stop
        [Test]
        public void Stop_BeforeStarted_DoesNothing()
        {
            // Arrange
            var router = new Router(name, senderCache, monitorCache, messageFactory, receiverMonitor, requestDispatcher);

            // Act
            router.Stop();

            // Assert
            mockMonitorCache.Verify(m => m.StopAllMonitors(), Times.Never);
            mockReceiverMonitor.Verify(m => m.StopReceivers(), Times.Never);
        }


        [Test]
        public void Stop_WhenRunning_StopsMonitors()
        {
            // Arrange
            var router = new Router(name, senderCache, monitorCache, messageFactory, receiverMonitor, requestDispatcher);
            router.Start();

            // Act
            router.Stop();

            // Assert
            mockMonitorCache.Verify(m => m.StopAllMonitors(), Times.Once);
            mockReceiverMonitor.Verify(m => m.StopReceivers(), Times.Once);
        }


        [Test]
        public void Stop_AfterAlreadyStopped_DoesNothing()
        {
            // Arrange
            var router = new Router(name, senderCache, monitorCache, messageFactory, receiverMonitor, requestDispatcher);
            router.Start();
            router.Stop();
            mockMonitorCache.ResetCalls();
            mockReceiverMonitor.ResetCalls();

            // Act
            router.Stop();

            // Assert
            mockMonitorCache.Verify(m => m.StopAllMonitors(), Times.Never);
            mockReceiverMonitor.Verify(m => m.StopReceivers(), Times.Never);
        }


        [Test]
        public void Stop_BeforeStarted_DoesNotSetStopTimestamp()
        {
            // Arrange
            var router = new Router(name, senderCache, monitorCache, messageFactory, receiverMonitor, requestDispatcher);

            // Act
            router.Stop();

            // Assert
            Assert.That(router.Info.StoppedTimestamp.HasValue, Is.False);
        }


        [Test]
        public void Stop_AfterRunning_SetsStoppedTimestamp()
        {
            // Arrange
            var router = new Router(name, senderCache, monitorCache, messageFactory, receiverMonitor, requestDispatcher);
            router.Start();

            // Act
            router.Stop();

            // Assert
            Assert.That(router.Info.StoppedTimestamp.HasValue, Is.True);
        }
        #endregion


        #region CreateResponse
        [Test]
        public void CreateResponse_WithValidResponse_CallsMessageFactory()
        {
            // Arrange
            var router = new Router(name, senderCache, monitorCache, messageFactory, receiverMonitor, requestDispatcher);
            var responseObject = "response";

            // Act
            var responseMessage = router.CreateResponse(responseObject);

            // Assert
            mockMessageFactory
                .Verify(
                    m => m.CreateResponse<string>(It.IsAny<string>()),
                    Times.Once);
        }


        [Test]
        public void CreateResponse_WithNullResponse_ThrowsArgumentException()
        {
            // Arrange
            var router = new Router(name, senderCache, monitorCache, messageFactory, receiverMonitor, requestDispatcher);
            string responseObject = null;

            // Act
            TestDelegate test = () => router.CreateResponse(responseObject);

            // Assert
            Assert.That(test, Throws.ArgumentNullException);
        }
        #endregion


        #region RequestHandler
        [Test]
        public void RequestHandler_WithHandler_ExtractsRequest()
        {
            // Arrange
            var router = new Router(name, senderCache, monitorCache, messageFactory, receiverMonitor, requestDispatcher);
            var message = new DataMessage<string>(new GuidMessageId(), "something");
            var requestTask = new RequestTask(message, m => { });
            SetupMirroredHandle<string>();

            // Act
            router.RequestHandler(requestTask);

            // Assert
            mockMessageFactory.Verify(m => m.ExtractRequest(It.IsAny<Message>()), Times.Once);
        }


        [Test]
        public void RequestHandler_WithHandledRequest_CallsRequestDispatcher()
        {
            // Arrange
            var router = new Router(name, senderCache, monitorCache, messageFactory, receiverMonitor, requestDispatcher);
            var message = new DataMessage<string>(new GuidMessageId(), "something");
            var requestTask = new RequestTask(message, m => { });
            SetupMirroredHandle<string>();

            // Act
            router.RequestHandler(requestTask);

            // Assert
            mockRequestDispatcher.Verify(m => m.Handle(It.IsAny<object>()), Times.Once);
        }


        [Test]
        public void RequestHandler_WithHandledRequest_CreateResponseMessage()
        {
            // Arrange
            var router = new Router(name, senderCache, monitorCache, messageFactory, receiverMonitor, requestDispatcher);
            var message = new DataMessage<string>(new GuidMessageId(), "something");
            var requestTask = new RequestTask(message, m => { });
            SetupMirroredHandle<string>();

            // Act
            router.RequestHandler(requestTask);

            // Assert
            mockMessageFactory.Verify(m => m.CreateResponse<string>(It.IsAny<string>()), Times.Once);
        }


        [Test]
        public void RequestHandler_WithHandledRequest_CallsResponseHandler()
        {
            // Arrange
            var router = new Router(name, senderCache, monitorCache, messageFactory, receiverMonitor, requestDispatcher);
            var message = new DataMessage<string>(new GuidMessageId(), "something");
            var responseHandlerCalled = false;
            var requestTask = new RequestTask(message, m => { responseHandlerCalled = true; });
            SetupMirroredHandle<string>();

            // Act
            router.RequestHandler(requestTask);

            // Assert
            Assert.That(responseHandlerCalled, Is.True);
        }
        #endregion
    }
}
