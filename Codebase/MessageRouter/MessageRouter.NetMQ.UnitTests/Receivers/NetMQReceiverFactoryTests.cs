using MessageRouter.NetMQ.Receivers;
using MessageRouter.Serialization;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessageRouter.NetMQ.UnitTests.Receivers
{
    [TestFixture]
    public class NetMQReceiverFactoryTests
    {
        private readonly Mock<ISerializer<byte[]>> mockSerializer = new Mock<ISerializer<byte[]>>();
        private ISerializer<byte[]> serializer;


        [SetUp]
        public void Setup()
        {
            serializer = mockSerializer.Object;
        }


        [TearDown]
        public void TearDown()
        {
            mockSerializer.Reset();
        }


        [Test]
        public void NetMQReceiverFactory_WithNullSerializer_ThrowsArgumentNullException()
        {
            // Act
            TestDelegate construct = () => new NetMQReceiverFactory(null);

            // Assert
            Assert.That(construct, Throws.ArgumentNullException);
        }


        [Test]
        public void Create_WithNullAddress_ThrowArgumentNullException()
        {
            // Arrange
            var factory = new NetMQReceiverFactory(serializer);

            // Act
            TestDelegate create = () => factory.Create(null);

            // Assert
            Assert.That(create, Throws.ArgumentNullException);
        }
    }
}
