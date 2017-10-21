using MessageRouter.Messages;
using NUnit.Framework;
using System;

namespace MessageRouter.Tests
{
    [TestFixture]
    public class DataMessageTests
    {
        [Test]
        public void DataMessage_WithRequiredFields_InitializesObject()
        {
            // Arrange
            var id = new GuidMessageId();
            var data = new object();

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
            var data = new object();

            // Act
            TestDelegate test = () => new DataMessage<object>(null, data);

            // Assert
            Assert.That(test, Throws.InstanceOf<ArgumentNullException>());
        }


        [Test]
        public void DataMessage_WithNullData_ThrowsArgumentNullException()
        {
            // Act
            var id = new GuidMessageId();

            // Act
            TestDelegate test = () => new DataMessage<object>(id, null);

            // Assert
            Assert.That(test, Throws.InstanceOf<ArgumentNullException>());
        }


        [Test]
        public void Body_WithSuppliedData_ReturnsData()
        {
            // Arrange
            var id = new GuidMessageId();
            var data = new object();
            var message = new DataMessage<object>(id, data);

            // Act
            var body = message.Body;

            // Assert
            Assert.AreSame(body, data);
        }
    }
}
