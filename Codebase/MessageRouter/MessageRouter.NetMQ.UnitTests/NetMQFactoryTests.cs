using MessageRouter.NetMQ.Receivers;
using MessageRouter.NetMQ.Senders;
using MessageRouter.Receivers;
using MessageRouter.Senders;
using MessageRouter.Serialization;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessageRouter.NetMQ.UnitTests
{
    [TestFixture]
    public class NetMQFactoryTests
    {
        private readonly Mock<ISenderMonitor<INetMQSender>> mockSenderMonitor = new Mock<ISenderMonitor<INetMQSender>>();
        private ISenderMonitor<INetMQSender> senderMonitor;

        private readonly Mock<IReceiverMonitor<INetMQReceiver>> mockReceiverMonitor = new Mock<IReceiverMonitor<INetMQReceiver>>();
        private IReceiverMonitor<INetMQReceiver> receiverMonitor;

        private readonly Mock<ISerializer<byte[]>> mockSerializer = new Mock<ISerializer<byte[]>>();
        private ISerializer<byte[]> serializer;


        [SetUp]
        public void Setup()
        {
            senderMonitor = mockSenderMonitor.Object;
            receiverMonitor = mockReceiverMonitor.Object;
            serializer = mockSerializer.Object;
        }


        [TearDown]
        public void TearDown()
        {
            mockSenderMonitor.Reset();
            mockReceiverMonitor.Reset();
            mockSerializer.Reset();
        }


        #region Constructor
        [Test]
        public void NetMQFactory_WithNullSenderMonitor_ThrowsArgumentNullException()
        {
            // Act
            TestDelegate construct = () => new NetMQFactory(null, receiverMonitor, serializer);

            // Assert
            Assert.That(construct, Throws.ArgumentNullException);
        }


        [Test]
        public void NetMQFactory_WithNullReceiverMonitor_ThrowsArgumentNullException()
        {
            // Act
            TestDelegate construct = () => new NetMQFactory(senderMonitor, null, serializer);

            // Assert
            Assert.That(construct, Throws.ArgumentNullException);
        }


        [Test]
        public void NetMQFactory_WithNullSerializer_ThrowsArgumentNullException()
        {
            // Act
            TestDelegate construct = () => new NetMQFactory(senderMonitor, receiverMonitor, null);

            // Assert
            Assert.That(construct, Throws.ArgumentNullException);
        }
        #endregion
    }
}
