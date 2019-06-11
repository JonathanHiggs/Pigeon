using NUnit.Framework;

using Pigeon.Serialization;

namespace Pigeon.UnitTests.Serialization
{
    [TestFixture]
    public class SerializationHeaderTests
    {
        [Test]
        public void FromToBytes()
        {
            // Arrange
            var version = new ProtocolVersion { Major = 1, Minor = 0 };
            var name = "Something";

            // Act
            var header = new SerializationHeader(version, name);
            var bytes = header.ToBytes();
            var (header2, count) = SerializationHeader.FromBytes(bytes);

            // Assert
            Assert.That(header2.Protocol, Is.EqualTo(version));
            Assert.That(header2.SerializationName, Is.EqualTo(name));
            Assert.That(count, Is.EqualTo(bytes.Length));
        }
    }
}
