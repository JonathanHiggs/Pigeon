using MessageRouter.Messages;
using NUnit.Framework;
using System;

namespace MessageRouter.UnitTests
{
    [TestFixture]
    public class DataMessageTests
    {
        private readonly IMessageId id = new GuidMessageId();
        private readonly object data = new object();


        [Test]
        public void DataMessage_WithRequiredFields_InitializesObject()
        {
            // Act
            var message = new DataMessage<object>(id, data);

            // Assert
            Assert.AreSame(id, message.Id);
            Assert.AreSame(data, message.Data);
        }


        [Test]
        public void DataMessage_WithNullId_ThrowsArgumentNullException()
        {
            // Act
            TestDelegate construct = () => new DataMessage<object>(null, data);

            // Assert
            Assert.That(construct, Throws.ArgumentNullException);
        }


        [Test]
        public void DataMessage_WithNullData_ThrowsArgumentNullException()
        {
            // Act
            TestDelegate construct = () => new DataMessage<object>(id, null);

            // Assert
            Assert.That(construct, Throws.ArgumentNullException);
        }
       

        [Test]
        public void Body_WithSuppliedData_ReturnsSameInstance()
        {
            // Arrange
            var message = new DataMessage<object>(id, data);

            // Act
            var body = message.Body;

            // Assert
            Assert.AreSame(body, data);
        }


        [Test]
        public void Data_WithSuppliedData_ReturnsSameInstance()
        {
            // Arrange
            var message = new DataMessage<object>(id, data);

            // Act
            var dataProperty = message.Data;

            // Assert
            Assert.AreSame(dataProperty, data);
        }
    }
}
