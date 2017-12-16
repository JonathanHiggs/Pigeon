using MessageRouter.Messages;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessageRouter.UnitTests.Messages
{
    [TestFixture]
    public class ExceptionMessageTests
    {
        private readonly IMessageId id = new GuidMessageId();
        private readonly Exception exception = new Exception();


        [Test]
        public void ExceptionMessage_WithNullId_ThrowsArgumentNullException()
        {
            // Act
            TestDelegate construct = () => new ExceptionMessage(null, exception);

            // Assert
            Assert.That(construct, Throws.ArgumentNullException);
        }


        [Test]
        public void ExceptionMessage_WithNullException_ThrowsArgumentNullException()
        {
            // Act
            TestDelegate construct = () => new ExceptionMessage(id, null);

            // Assert
            Assert.That(construct, Throws.ArgumentNullException);
        }


        [Test]
        public void Body_WithExcepionSupplied_ReturnsSameInstance()
        {
            // Arrange
            var message = new ExceptionMessage(id, exception);

            // Act
            var body = message.Body;

            // Assert
            Assert.AreSame(body, exception);
        }


        [Test]
        public void Exception_WithExceptionSupplied_ReturnsSameInstance()
        {
            // Arrange
            var message = new ExceptionMessage(id, exception);

            // Act
            var exceptionProperty = message.Exception;

            // Assert
            Assert.AreSame(exceptionProperty, exception);
        }
    }
}
