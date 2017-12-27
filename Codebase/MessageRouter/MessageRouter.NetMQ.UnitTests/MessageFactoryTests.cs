using MessageRouter.Packages;
using MessageRouter.Serialization;
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
    public class MessageFactoryTests
    {
        #region Setup & Mocks
        private readonly Mock<ISerializer> mockSerializer = new Mock<ISerializer>();
        private ISerializer serializer;

        private readonly Mock<IPackageFactory> mockPackageFactory = new Mock<IPackageFactory>();
        private IPackageFactory packageFactory;

        private readonly string obj = "topicEvent";
        private readonly int requestId = 42;
        private readonly byte[] data = new byte[] { 0, 1 };
        private readonly byte[] address = new byte[] { 0, 192, 168, 1, 1 };
        private DataPackage<string> package;
                

        [SetUp]
        public void Setup()
        {
            serializer = mockSerializer.Object;
            packageFactory = mockPackageFactory.Object;

            package = new DataPackage<string>(new GuidPackageId(), obj);

            mockPackageFactory
                .Setup(m => m.Pack(It.IsAny<object>()))
                .Returns(package);

            mockPackageFactory
                .Setup(m => m.Unpack(It.IsAny<Package>()))
                .Returns(obj);

            mockSerializer
                .Setup(m => m.Serialize(It.IsAny<Package>()))
                .Returns(data);

            mockSerializer
                .Setup(m => m.Deserialize<Package>(It.IsIn<byte[]>(data)))
                .Returns(package);
        }


        [TearDown]
        public void TearDown()
        {
            mockSerializer.Reset();
            mockPackageFactory.Reset();
        }
        #endregion


        #region Constructor
        [Test]
        public void MessageFactory_WithNullSerializer_ThrowsArgumentNullException()
        {
            // Act
            TestDelegate construct = () => new MessageFactory(null, packageFactory);

            // Assert
            Assert.That(construct, Throws.ArgumentNullException);
        }


        [Test]
        public void MessageFactory_WithNullPackageFactory_ThrowsArgumentNullException()
        {
            // Act
            TestDelegate construct = () => new MessageFactory(serializer, null);

            // Assert
            Assert.That(construct, Throws.ArgumentNullException);
        }
        #endregion


        #region CreateTopicMessage
        [Test]
        public void CreateTopicMessage_WithTopicEvent_PacksInPackage()
        {
            // Arrange
            var messageFactory = new MessageFactory(serializer, packageFactory);

            // Act
            var message = messageFactory.CreateTopicMessage(obj);

            // Assert
            mockPackageFactory.Verify(m => m.Pack(It.IsIn<object>(obj)), Times.Once);
        }


        [Test]
        public void CreateTopicMessage_WithTopicEvent_SerializesPackage()
        {
            // Arrange
            var messageFactory = new MessageFactory(serializer, packageFactory);

            // Act
            var message = messageFactory.CreateTopicMessage(obj);

            // Assert
            mockSerializer.Verify(m => m.Serialize(It.IsIn<Package>(package)), Times.Once);
        }
        

        [Test]
        public void CreateTopicMessage_WithTopicEvent_MessageHasTwoFrames()
        {
            // Arrange
            var messageFactory = new MessageFactory(serializer, packageFactory);

            // Act
            var message = messageFactory.CreateTopicMessage(obj);

            // Assert
            Assert.That(message.FrameCount, Is.EqualTo(2));
        }


        [Test]
        public void CreateTopicMessage_WithTopicEvent_FirstFrameIsFullName()
        {
            // Arrange
            var messageFactory = new MessageFactory(serializer, packageFactory);

            // Act
            var message = messageFactory.CreateTopicMessage(obj);

            // Assert
            Assert.That(message[0].ConvertToString(), Is.EqualTo(obj.GetType().FullName));
        }


        [Test]
        public void CreateTopicMessage_WithTopicEvent_SecondFrameIsSerializedData()
        {
            // Arrange
            var messageFactory = new MessageFactory(serializer, packageFactory);

            // Act
            var message = messageFactory.CreateTopicMessage(obj);

            // Assert
            Assert.That(message[1].ToByteArray(), Is.EqualTo(data));
        }
        #endregion


        #region ExtractTopic
        [Test]
        public void ExtractTopic_WithMessage_ReturnsTopicEvent()
        {
            // Arrange
            var messageFactory = new MessageFactory(serializer, packageFactory);
            var message = messageFactory.CreateTopicMessage(obj);

            // Act
            var topicEvent = messageFactory.ExtractTopic(message);

            // Assert
            Assert.That(topicEvent, Is.EqualTo(obj));
        }


        [Test]
        public void ExtractTopic_WithMessage_DeserializesPackage()
        {
            // Arrange
            var messageFactory = new MessageFactory(serializer, packageFactory);
            var message = messageFactory.CreateTopicMessage(obj);

            // Act
            var topicEvent = messageFactory.ExtractTopic(message);

            // Assert
            mockSerializer.Verify(m => m.Deserialize<Package>(It.IsIn<byte[]>(data)), Times.Once);
        }


        [Test]
        public void ExtractTopic_WithMessage_UnpacksTopicEvent()
        {
            // Arrange
            var messageFactory = new MessageFactory(serializer, packageFactory);
            var message = messageFactory.CreateTopicMessage(obj);

            // Act
            var topicEvent = messageFactory.ExtractTopic(message);

            // Assert
            mockPackageFactory.Verify(m => m.Unpack(It.IsIn<Package>(package)), Times.Once);
        }
        #endregion


        #region CreateRequestMessage
        [Test]
        public void CreateRequestMessage_WithRequest_PacksRequest()
        {
            // Arrange
            var messageFactory = new MessageFactory(serializer, packageFactory);

            // Act
            var message = messageFactory.CreateRequestMessage(obj, requestId);

            // Assert
            mockPackageFactory.Verify(m => m.Pack(It.IsIn<object>(obj)), Times.Once);
        }


        [Test]
        public void CreateRequestMessage_WithRequest_SerializesPackage()
        {
            // Arrange
            var messageFactory = new MessageFactory(serializer, packageFactory);

            // Act
            var message = messageFactory.CreateRequestMessage(obj, requestId);

            // Assert
            mockSerializer.Verify(m => m.Serialize(It.IsIn<Package>(package)), Times.Once);
        }


        [Test]
        public void CreateRequestMessage_WithRequest_MessageHasTwoFrames()
        {
            // Arrange
            var messageFactory = new MessageFactory(serializer, packageFactory);

            // Act
            var message = messageFactory.CreateRequestMessage(obj, requestId);

            // Assert
            Assert.That(message.FrameCount, Is.EqualTo(4));
        }


        [Test]
        public void CreateRequestMessage_WithRequest_FirstFrameEmpty()
        {
            // Arrange
            var messageFactory = new MessageFactory(serializer, packageFactory);

            // Act
            var message = messageFactory.CreateRequestMessage(obj, requestId);

            // Assert
            Assert.That(message[0], Is.EqualTo(NetMQFrame.Empty));
        }


        [Test]
        public void CreateRequestMessage_WithRequest_SecondFrameIsRequestId()
        {
            // Arrange
            var messageFactory = new MessageFactory(serializer, packageFactory);

            // Act
            var message = messageFactory.CreateRequestMessage(obj, requestId);

            // Assert
            Assert.That(message[1].ConvertToInt32(), Is.EqualTo(requestId));
        }


        [Test]
        public void CreateRequestMessage_WithRequest_ThirdFrameEmpty()
        {
            // Arrange
            var messageFactory = new MessageFactory(serializer, packageFactory);

            // Act
            var message = messageFactory.CreateRequestMessage(obj, requestId);

            // Assert
            Assert.That(message[2], Is.EqualTo(NetMQFrame.Empty));
        }


        [Test]
        public void CreateRequestMessage_WithRequest_ForthFrameIsSerializedData()
        {
            // Arrange
            var messageFactory = new MessageFactory(serializer, packageFactory);

            // Act
            var message = messageFactory.CreateRequestMessage(obj, requestId);

            // Assert
            Assert.That(message[3].ToByteArray(), Is.EqualTo(data));
        }
        #endregion


        #region ExtractRequest
        [Test]
        public void ExtractRequest_WithMessage_ReturnsRequest()
        {
            // Arrange
            var messageFactory = new MessageFactory(serializer, packageFactory);
            var message = messageFactory.CreateRequestMessage(obj, requestId);
            message.Push(address); // NetMQ prepends an address frame

            // Act
            var (request, retAddress, retRequestId) = messageFactory.ExtractRequest(message);

            // Assert
            Assert.That(request, Is.EqualTo(obj));
        }


        [Test]
        public void ExtractRequest_WithMessage_ReturnsRequestId()
        {
            // Arrange
            var messageFactory = new MessageFactory(serializer, packageFactory);
            var message = messageFactory.CreateRequestMessage(obj, requestId);
            message.Push(address); // NetMQ prepends an address frame

            // Act
            var (request, retAddress, retRequestId) = messageFactory.ExtractRequest(message);

            // Assert
            Assert.That(retRequestId, Is.EqualTo(requestId));
        }


        [Test]
        public void ExtractRequest_WithMessage_ReturnsSameAddress()
        {
            // Arrange
            var messageFactory = new MessageFactory(serializer, packageFactory);
            var message = messageFactory.CreateRequestMessage(obj, requestId);
            message.Push(address); // NetMQ prepends an address frame

            // Act
            var (request, retAddress, retRequestId) = messageFactory.ExtractRequest(message);

            // Assert
            Assert.That(retAddress, Is.EqualTo(address));
        }


        [Test]
        public void ExtractRequest_WithMessage_DeserializesPackage()
        {
            // Arrange
            var messageFactory = new MessageFactory(serializer, packageFactory);
            var message = messageFactory.CreateRequestMessage(obj, requestId);
            message.Push(address); // NetMQ prepends an address frame

            // Act
            var (request, retAddress, retRequestId) = messageFactory.ExtractRequest(message);

            // Assert
            mockSerializer.Verify(m => m.Deserialize<Package>(It.IsIn<byte[]>(data)), Times.Once);
        }


        [Test]
        public void ExtractRequest_WithMessage_UnpacksRequestEvent()
        {
            // Arrange
            var messageFactory = new MessageFactory(serializer, packageFactory);
            var message = messageFactory.CreateRequestMessage(obj, requestId);
            message.Push(address); // NetMQ prepends an address frame

            // Act
            var (request, retAddress, retRequestId) = messageFactory.ExtractRequest(message);

            // Assert
            mockPackageFactory.Verify(m => m.Unpack(It.IsIn<Package>(package)), Times.Once);
        }
        #endregion


        #region CreateResponse
        [Test]
        public void CreateResponse_WithResponse_PacksResponse()
        {
            // Arrange
            var messageFactory = new MessageFactory(serializer, packageFactory);

            // Act
            var message = messageFactory.CreateResponseMessage(obj, address, requestId);

            // Assert
            mockPackageFactory.Verify(m => m.Pack(It.IsIn<object>(obj)), Times.Once);
        }


        [Test]
        public void CreateResponse_WithResponse_SerializesPackage()
        {
            // Arrange
            var messageFactory = new MessageFactory(serializer, packageFactory);

            // Act
            var message = messageFactory.CreateResponseMessage(obj, address, requestId);

            // Assert
            mockSerializer.Verify(m => m.Serialize(It.IsIn<Package>(package)), Times.Once);
        }


        [Test]
        public void CreateResponse_WithResponse_HasFiveFrames()
        {
            // Arrange
            var messageFactory = new MessageFactory(serializer, packageFactory);

            // Act
            var message = messageFactory.CreateResponseMessage(obj, address, requestId);

            // Assert
            Assert.That(message.FrameCount, Is.EqualTo(5));
        }


        [Test]
        public void CreateResponseMessage_WithResponse_FirstFrameIsAddress()
        {
            // Arrange
            var messageFactory = new MessageFactory(serializer, packageFactory);

            // Act
            var message = messageFactory.CreateResponseMessage(obj, address, requestId);

            // Assert
            Assert.That(message[0].ToByteArray(), Is.EqualTo(address));
        }


        [Test]
        public void CreateResponseMessage_WithResponse_SecondFrameEmpty()
        {
            // Arrange
            var messageFactory = new MessageFactory(serializer, packageFactory);

            // Act
            var message = messageFactory.CreateResponseMessage(obj, address, requestId);

            // Assert
            Assert.That(message[1], Is.EqualTo(NetMQFrame.Empty));
        }


        [Test]
        public void CreateResponseMessage_WithResponse_ThirdFrameIsRequestId()
        {
            // Arrange
            var messageFactory = new MessageFactory(serializer, packageFactory);

            // Act
            var message = messageFactory.CreateResponseMessage(obj, address, requestId);

            // Assert
            Assert.That(message[2].ConvertToInt32(), Is.EqualTo(requestId));
        }


        [Test]
        public void CreateResponseMessage_WithResponse_ForthFrameEmpty()
        {
            // Arrange
            var messageFactory = new MessageFactory(serializer, packageFactory);

            // Act
            var message = messageFactory.CreateResponseMessage(obj, address, requestId);

            // Assert
            Assert.That(message[3], Is.EqualTo(NetMQFrame.Empty));
        }


        [Test]
        public void CreateResponseMessage_WithResponse_FifthFrameIsSerializedData()
        {
            // Arrange
            var messageFactory = new MessageFactory(serializer, packageFactory);

            // Act
            var message = messageFactory.CreateResponseMessage(obj, address, requestId);

            // Assert
            Assert.That(message[4].ToByteArray(), Is.EqualTo(data));
        }
        #endregion
    }
}
