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
    public class MessageFactoryTests
    {
        private readonly Mock<ISerializer> mockSerializer = new Mock<ISerializer>();
        private ISerializer serializer;


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
        public void MessageFactory_WithNullSerializer_ThrowsArgumentNullException()
        {
            // Act
            TestDelegate construct = () => new MessageFactory(null);

            // Assert
            Assert.That(construct, Throws.ArgumentNullException);
        }
    }
}
