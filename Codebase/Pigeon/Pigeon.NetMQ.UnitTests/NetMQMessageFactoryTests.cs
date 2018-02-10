using Pigeon.Packages;
using Pigeon.Serialization;
using Moq;
using NetMQ;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pigeon.NetMQ.UnitTests
{
    [TestFixture]
    public class NetMQMessageFactoryTests
    {
        #region Setup & Mocks
        private readonly Mock<ISerializer> mockSerializer = new Mock<ISerializer>();
        private ISerializer serializer;
        private DotNetSerializer dotNetSerializer = new DotNetSerializer();

        private readonly Mock<IPackageFactory> mockPackageFactory = new Mock<IPackageFactory>();
        private IPackageFactory packageFactory;

        private readonly string obj = "thing";
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
                .Returns<Package>(p => dotNetSerializer.Serialize(p));

            mockSerializer
                .Setup(m => m.Serialize(It.IsAny<object>(), It.IsAny<int>()))
                .Returns<Package, int>((obj, offset) => dotNetSerializer.Serialize(obj, offset));

            mockSerializer
                .Setup(m => m.Deserialize<Package>(It.IsAny<byte[]>()))
                .Returns<byte[]>(d => dotNetSerializer.Deserialize<Package>(d));

            mockSerializer
                .Setup(m => m.Deserialize(It.IsAny<byte[]>(), It.IsAny<int>()))
                .Returns<byte[], int>((data, offset) => dotNetSerializer.Deserialize(data, offset));
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
            TestDelegate construct = () => new NetMQMessageFactory(null, packageFactory);

            // Assert
            Assert.That(construct, Throws.ArgumentNullException);
        }


        [Test]
        public void MessageFactory_WithNullPackageFactory_ThrowsArgumentNullException()
        {
            // Act
            TestDelegate construct = () => new NetMQMessageFactory(serializer, null);

            // Assert
            Assert.That(construct, Throws.ArgumentNullException);
        }
        #endregion


        #region CreateTopicMessage
        [Test]
        public void CreateTopicMessage_WithTopicEvent_PacksInPackage()
        {
            // Arrange
            var messageFactory = new NetMQMessageFactory(serializer, packageFactory);

            // Act
            var message = messageFactory.CreateTopicMessage(obj);

            // Assert
            mockPackageFactory.Verify(m => m.Pack(It.IsIn<object>(obj)), Times.Once);
        }


        [Test]
        public void CreateTopicMessage_WithTopicEvent_SerializesPackage()
        {
            // Arrange
            var messageFactory = new NetMQMessageFactory(serializer, packageFactory);

            // Act
            var message = messageFactory.CreateTopicMessage(obj);

            // Assert
            mockSerializer.Verify(m => m.Serialize(It.IsIn<Package>(package)), Times.Once);
        }
        

        [Test]
        public void CreateTopicMessage_WithTopicEvent_MessageHasTwoFrames()
        {
            // Arrange
            var messageFactory = new NetMQMessageFactory(serializer, packageFactory);

            // Act
            var message = messageFactory.CreateTopicMessage(obj);

            // Assert
            Assert.That(message.FrameCount, Is.EqualTo(2));
        }


        [Test]
        public void CreateTopicMessage_WithTopicEvent_FirstFrameIsFullName()
        {
            // Arrange
            var messageFactory = new NetMQMessageFactory(serializer, packageFactory);

            // Act
            var message = messageFactory.CreateTopicMessage(obj);

            // Assert
            Assert.That(message[0].ConvertToString(), Is.EqualTo(obj.GetType().FullName));
        }


        [Test]
        public void CreateTopicMessage_WithTopicEvent_SecondFrameIsSerializedData()
        {
            // Arrange
            var messageFactory = new NetMQMessageFactory(serializer, packageFactory);

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
            var messageFactory = new NetMQMessageFactory(serializer, packageFactory);
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
            var messageFactory = new NetMQMessageFactory(serializer, packageFactory);
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
            var messageFactory = new NetMQMessageFactory(serializer, packageFactory);
            var message = messageFactory.CreateTopicMessage(obj);

            // Act
            var topicEvent = messageFactory.ExtractTopic(message);

            // Assert
            mockPackageFactory.Verify(m => m.Unpack(It.IsIn<Package>(package)), Times.Once);
        }
        #endregion


        #region IsValidTopicMessage
        [Test]
        public void IsValidTopicMessage_WithWellFormedMessage_IsTrue()
        {
            // Arrange
            var messageFactory = new NetMQMessageFactory(serializer, packageFactory);
            var message = new NetMQMessage(2);
            message.Push("Topic");
            message.Push(data);

            // Act
            var isValid = messageFactory.IsValidTopicMessage(message);

            // Assert
            Assert.That(isValid, Is.True);
        }


        [Test]
        public void IsValidTopicMessage_WithNullMessage_IsFalse()
        {
            // Arrange
            var messageFactory = new NetMQMessageFactory(serializer, packageFactory);

            // Act
            var isValid = messageFactory.IsValidTopicMessage(null);

            // Assert
            Assert.That(isValid, Is.False);
        }


        [Test]
        public void IsValidTopicMessage_WithEmptyTopicName_IsFalse()
        {
            // Arrange
            var messageFactory = new NetMQMessageFactory(serializer, packageFactory);
            var message = new NetMQMessage(2);
            message.PushEmptyFrame();
            message.Push(data);

            // Act
            var isValid = messageFactory.IsValidTopicMessage(message);

            // Assert
            Assert.That(isValid, Is.False);
        }


        [Test]
        public void IsValidTopicMessage_WithNoData_IsFalse()
        {
            // Arrange
            var messageFactory = new NetMQMessageFactory(serializer, packageFactory);
            var message = new NetMQMessage(2);
            message.Push("Topic");
            message.PushEmptyFrame();

            // Act
            var isValid = messageFactory.IsValidTopicMessage(message);

            // Assert
            Assert.That(isValid, Is.False);
        }
        #endregion

        #region CreateRequestMessage
        [Test]
        public void CreateRequestMessage_WithRequest_PacksRequest()
        {
            // Arrange
            var messageFactory = new NetMQMessageFactory(serializer, packageFactory);

            // Act
            var message = messageFactory.CreateRequestMessage(obj, requestId);

            // Assert
            mockPackageFactory.Verify(m => m.Pack(It.IsIn<object>(obj)), Times.Once);
        }


        [Test]
        public void CreateRequestMessage_WithRequest_SerializesPackage()
        {
            // Arrange
            var messageFactory = new NetMQMessageFactory(serializer, packageFactory);

            // Act
            var message = messageFactory.CreateRequestMessage(obj, requestId);

            // Assert
            mockSerializer.Verify(m => m.Serialize(It.IsIn<Package>(package)), Times.Once);
        }


        [Test]
        public void CreateRequestMessage_WithRequest_MessageHasFourFrames()
        {
            // Arrange
            var messageFactory = new NetMQMessageFactory(serializer, packageFactory);

            // Act
            var message = messageFactory.CreateRequestMessage(obj, requestId);

            // Assert
            Assert.That(message.FrameCount, Is.EqualTo(4));
        }


        [Test]
        public void CreateRequestMessage_WithRequest_FirstFrameEmpty()
        {
            // Arrange
            var messageFactory = new NetMQMessageFactory(serializer, packageFactory);

            // Act
            var message = messageFactory.CreateRequestMessage(obj, requestId);

            // Assert
            Assert.That(message[0], Is.EqualTo(NetMQFrame.Empty));
        }


        [Test]
        public void CreateRequestMessage_WithRequest_SecondFrameIsRequestId()
        {
            // Arrange
            var messageFactory = new NetMQMessageFactory(serializer, packageFactory);

            // Act
            var message = messageFactory.CreateRequestMessage(obj, requestId);

            // Assert
            Assert.That(message[1].ConvertToInt32(), Is.EqualTo(requestId));
        }


        [Test]
        public void CreateRequestMessage_WithRequest_ThirdFrameEmpty()
        {
            // Arrange
            var messageFactory = new NetMQMessageFactory(serializer, packageFactory);

            // Act
            var message = messageFactory.CreateRequestMessage(obj, requestId);

            // Assert
            Assert.That(message[2], Is.EqualTo(NetMQFrame.Empty));
        }


        [Test]
        public void CreateRequestMessage_WithRequest_ForthFrameIsSerializedData()
        {
            // Arrange
            var messageFactory = new NetMQMessageFactory(serializer, packageFactory);

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
            var messageFactory = new NetMQMessageFactory(serializer, packageFactory);
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
            var messageFactory = new NetMQMessageFactory(serializer, packageFactory);
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
            var messageFactory = new NetMQMessageFactory(serializer, packageFactory);
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
            var messageFactory = new NetMQMessageFactory(serializer, packageFactory);
            var message = messageFactory.CreateRequestMessage(obj, requestId);
            message.Push(address); // NetMQ prepends an address frame

            // Act
            var (request, retAddress, retRequestId) = messageFactory.ExtractRequest(message);

            // Assert
            mockSerializer.Verify(m => m.Deserialize<Package>(It.IsIn<byte[]>(data)), Times.Once);
        }


        [Test]
        public void ExtractRequest_WithMessage_UnpacksRequestObject()
        {
            // Arrange
            var messageFactory = new NetMQMessageFactory(serializer, packageFactory);
            var message = messageFactory.CreateRequestMessage(obj, requestId);
            message.Push(address); // NetMQ prepends an address frame

            // Act
            var (request, retAddress, retRequestId) = messageFactory.ExtractRequest(message);

            // Assert
            mockPackageFactory.Verify(m => m.Unpack(It.IsIn<Package>(package)), Times.Once);
        }
        #endregion


        #region IsValidRequestMessage
        [Test]
        public void IsValidRequestMessage_WithNullMessage_IsFalse()
        {
            // Arrange
            var messageFactory = new NetMQMessageFactory(serializer, packageFactory);

            // Act
            var isValid = messageFactory.IsValidRequestMessage(null);

            // Assert
            Assert.That(isValid, Is.False);
        }


        [Test]
        public void IsValidRequestMessage_WithFiveFrameMessage_IsTrue()
        {
            // Arrange
            var messageFactory = new NetMQMessageFactory(serializer, packageFactory);
            var message = messageFactory.CreateRequestMessage(obj, 1);
            message.Push(address);

            // Act
            var isValid = messageFactory.IsValidRequestMessage(message);

            // Assert
            Assert.That(isValid, Is.True);
        }


        [Test]
        public void IsValidRequestMessage_WithEmptyAddress_IsFalse()
        {
            // Arrange
            var messageFactory = new NetMQMessageFactory(serializer, packageFactory);
            var message = messageFactory.CreateRequestMessage(obj, requestId);
            message.PushEmptyFrame();

            // Act
            var isValid = messageFactory.IsValidRequestMessage(message);

            // Assert
            Assert.That(isValid, Is.False);
        }


        [Test]
        public void IsValidRequestMessage_WithEmptyRequestId_IsFalse()
        {
            // Arrange
            var messageFactory = new NetMQMessageFactory(serializer, packageFactory);
            var message = new NetMQMessage(5);
            message.Append(address);
            message.AppendEmptyFrame();
            message.AppendEmptyFrame();
            message.AppendEmptyFrame();
            message.Append(obj);

            // Act
            var isValid = messageFactory.IsValidRequestMessage(message);

            // Assert
            Assert.That(isValid, Is.False);
        }


        [Test]
        public void IsValidRequestMessage_WithEmptyDataFrame_IsFalse()
        {
            // Arrange
            var messageFactory = new NetMQMessageFactory(serializer, packageFactory);
            var message = new NetMQMessage(5);
            message.Append(address);
            message.AppendEmptyFrame();
            message.Append(requestId);
            message.AppendEmptyFrame();
            message.AppendEmptyFrame();

            // Act
            var isValid = messageFactory.IsValidRequestMessage(message);

            // Assert
            Assert.That(isValid, Is.False);
        }
        #endregion


        #region CreateResponse
        [Test]
        public void CreateResponse_WithResponse_PacksResponse()
        {
            // Arrange
            var messageFactory = new NetMQMessageFactory(serializer, packageFactory);

            // Act
            var message = messageFactory.CreateResponseMessage(obj, address, requestId);

            // Assert
            mockPackageFactory.Verify(m => m.Pack(It.IsIn<object>(obj)), Times.Once);
        }


        [Test]
        public void CreateResponse_WithResponse_SerializesPackage()
        {
            // Arrange
            var messageFactory = new NetMQMessageFactory(serializer, packageFactory);

            // Act
            var message = messageFactory.CreateResponseMessage(obj, address, requestId);

            // Assert
            mockSerializer.Verify(m => m.Serialize(It.IsIn<Package>(package)), Times.Once);
        }


        [Test]
        public void CreateResponse_WithResponse_HasFiveFrames()
        {
            // Arrange
            var messageFactory = new NetMQMessageFactory(serializer, packageFactory);

            // Act
            var message = messageFactory.CreateResponseMessage(obj, address, requestId);

            // Assert
            Assert.That(message.FrameCount, Is.EqualTo(5));
        }


        [Test]
        public void CreateResponseMessage_WithResponse_FirstFrameIsAddress()
        {
            // Arrange
            var messageFactory = new NetMQMessageFactory(serializer, packageFactory);

            // Act
            var message = messageFactory.CreateResponseMessage(obj, address, requestId);

            // Assert
            Assert.That(message[0].ToByteArray(), Is.EqualTo(address));
        }


        [Test]
        public void CreateResponseMessage_WithResponse_SecondFrameEmpty()
        {
            // Arrange
            var messageFactory = new NetMQMessageFactory(serializer, packageFactory);

            // Act
            var message = messageFactory.CreateResponseMessage(obj, address, requestId);

            // Assert
            Assert.That(message[1], Is.EqualTo(NetMQFrame.Empty));
        }


        [Test]
        public void CreateResponseMessage_WithResponse_ThirdFrameIsRequestId()
        {
            // Arrange
            var messageFactory = new NetMQMessageFactory(serializer, packageFactory);

            // Act
            var message = messageFactory.CreateResponseMessage(obj, address, requestId);

            // Assert
            Assert.That(message[2].ConvertToInt32(), Is.EqualTo(requestId));
        }


        [Test]
        public void CreateResponseMessage_WithResponse_ForthFrameEmpty()
        {
            // Arrange
            var messageFactory = new NetMQMessageFactory(serializer, packageFactory);

            // Act
            var message = messageFactory.CreateResponseMessage(obj, address, requestId);

            // Assert
            Assert.That(message[3], Is.EqualTo(NetMQFrame.Empty));
        }


        [Test]
        public void CreateResponseMessage_WithResponse_FifthFrameIsSerializedData()
        {
            // Arrange
            var messageFactory = new NetMQMessageFactory(serializer, packageFactory);

            // Act
            var message = messageFactory.CreateResponseMessage(obj, address, requestId);

            // Assert
            Assert.That(message[4].ToByteArray(), Is.EqualTo(data));
        }
        #endregion


        #region ExtractResponse
        [Test]
        public void ExtractResonse_WithMessage_ReturnsResponse()
        {
            // Arrange
            var messageFactory = new NetMQMessageFactory(serializer, packageFactory);
            var message = new NetMQMessage(4);
            message.AppendEmptyFrame();
            message.Append(requestId);
            message.AppendEmptyFrame();
            message.Append(data);

            // Act
            var (retRequestId, response) = messageFactory.ExtractResponse(message);

            // Assert
            Assert.That(response, Is.EqualTo(obj));
        }


        [Test]
        public void ExtractResponse_WithMessage_ReturnsRequestId()
        {
            // Arrange
            var messageFactory = new NetMQMessageFactory(serializer, packageFactory);
            var message = new NetMQMessage(4);
            message.AppendEmptyFrame();
            message.Append(requestId);
            message.AppendEmptyFrame();
            message.Append(data);

            // Act
            var (retRequestId, response) = messageFactory.ExtractResponse(message);

            // Assert
            Assert.That(retRequestId, Is.EqualTo(requestId));
        }


        [Test]
        public void ExtractResponse_WithMessage_DeserializesPackage()
        {
            // Arrange
            var messageFactory = new NetMQMessageFactory(serializer, packageFactory);
            var message = new NetMQMessage(4);
            message.AppendEmptyFrame();
            message.Append(requestId);
            message.AppendEmptyFrame();
            message.Append(data);

            // Act
            var (retRequestId, response) = messageFactory.ExtractResponse(message);

            // Assert
            mockSerializer.Verify(m => m.Deserialize<Package>(It.IsIn<byte[]>(data)), Times.Once);
        }


        [Test]
        public void ExtractResponse_WithMessage_UnpacksResponseObject()
        {
            // Arrange
            var messageFactory = new NetMQMessageFactory(serializer, packageFactory);
            var message = new NetMQMessage(4);
            message.AppendEmptyFrame();
            message.Append(requestId);
            message.AppendEmptyFrame();
            message.Append(data);

            // Act
            var (retRequestId, response) = messageFactory.ExtractResponse(message);

            // Assert
            mockPackageFactory.Verify(m => m.Unpack(It.IsIn(package)), Times.Once);
        }
        #endregion


        #region IsValidResponseMessage
        [Test]
        public void IsValidResponseMessage_WithNullMessage_IsFalse()
        {
            // Arrange
            var messageFactory = new NetMQMessageFactory(serializer, packageFactory);

            // Act
            var isValid = messageFactory.IsValidResponseMessage(null);

            // Assert
            Assert.That(isValid, Is.False);
        }


        [Test]
        public void IsValidResponseMessage_WithFourFrameMessage_IsTrue()
        {
            // Arrange
            var messageFactory = new NetMQMessageFactory(serializer, packageFactory);
            var message = new NetMQMessage(4);
            message.AppendEmptyFrame();
            message.Append(requestId);
            message.AppendEmptyFrame();
            message.Append(data);

            // Act
            var isValid = messageFactory.IsValidResponseMessage(message);

            // Assert
            Assert.That(isValid, Is.True);
        }


        [Test]
        public void IsValidResponseMessage_WithEmptyRequestId_IsFalse()
        {
            // Arrange
            var messageFactory = new NetMQMessageFactory(serializer, packageFactory);
            var message = new NetMQMessage(4);
            message.AppendEmptyFrame();
            message.AppendEmptyFrame();
            message.AppendEmptyFrame();
            message.Append(data);

            // Act
            var isValid = messageFactory.IsValidResponseMessage(message);

            // Assert
            Assert.That(isValid, Is.False);
        }


        [Test]
        public void IsValidResponseMessage_WithEmptyDataFrame_IsFalse()
        {
            // Arrange
            var messageFactory = new NetMQMessageFactory(serializer, packageFactory);
            var message = new NetMQMessage(4);
            message.AppendEmptyFrame();
            message.Append(requestId);
            message.AppendEmptyFrame();
            message.AppendEmptyFrame();

            // Act
            var isValid = messageFactory.IsValidResponseMessage(message);

            // Assert
            Assert.That(isValid, Is.False);
        }
        #endregion
    }
}
