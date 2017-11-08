using System;
using NUnit.Framework;
using MessageRouter.Messages;

namespace MessageRouter.UnitTests
{
    [TestFixture]
    public class GuidMessageIdTests
    {
        [Test]
        public void Equals_WithSameGuid_ReturnsTrue()
        {
            // Arrange
            var guid = Guid.NewGuid();
            var message1 = new GuidMessageId(guid);
            var message2 = new GuidMessageId(guid);

            // Act
            var equal = message1.Equals(message2);

            // Assert
            Assert.That(equal, Is.True);
        }


        [Test]
        public void Equals_WithDifferentGuids_ReturnsFalse()
        {
            // Arrange
            var message1 = new GuidMessageId();
            var message2 = new GuidMessageId();

            // Act
            var equal = message1.Equals(message2);

            // Assert
            Assert.That(equal, Is.False);
        }
    }
}
