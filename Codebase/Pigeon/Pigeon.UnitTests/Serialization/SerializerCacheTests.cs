using Moq;

using NUnit.Framework;

using Pigeon.Serialization;

namespace Pigeon.UnitTests.Serialization
{
    [TestFixture]
    public class SerializerCacheTests
    {
        private readonly Mock<ISerializer> mockSerializer = new Mock<ISerializer>();
        private ISerializer serializer;

        private readonly SerializationDescriptor descriptor = new SerializationDescriptor("test", "test", typeof(byte));


        [SetUp]
        public void Setup()
        {
            serializer = mockSerializer.Object;

            mockSerializer.Setup(m => m.Descriptor).Returns(descriptor);
        }


        [TearDown]
        public void TearDown()
        {
            mockSerializer.Reset();
        }

        
        [Test]
        public void AddSerializer_WithNullSerializer_ThrowsArgumentNullException()
        {
            // Arrange
            var cache = new SerializerCache();

            // Act
            TestDelegate addSerializer = () => cache.AddSerializer(null);

            // Assert
            Assert.That(addSerializer, Throws.ArgumentNullException);
        }


        [Test]
        public void AddSerializer_WithSerializer_DoesNotThrow()
        {
            // Arrange
            var cache = new SerializerCache();

            // Act
            TestDelegate addSerializer = () => cache.AddSerializer(serializer);

            // Assert
            Assert.That(addSerializer, Throws.Nothing);
        }


        [Test]
        public void SerializerFor_WithNoSerializerAdded_ReturnsFalse()
        {
            // Arrange
            var cache = new SerializerCache();

            // Act
            var hasSerializer = cache.SerializerFor("test", out var returnedSerializer);

            // Assert
            Assert.That(hasSerializer, Is.False);
            Assert.That(returnedSerializer, Is.Null);
        }


        [Test]
        public void SerializerFor_WithSerializerAdded_ReturnsTrue()
        {
            // Arrange
            var cache = new SerializerCache();
            cache.AddSerializer(serializer);

            // Act
            var hasSerializer = cache.SerializerFor("test", out var returnedSerializer);

            // Assert
            Assert.That(hasSerializer, Is.True);
        }


        [Test]
        public void SerializerFor_WithSerializerAdded_ReturnsSameInstance()
        {
            // Arrange
            var cache = new SerializerCache();
            cache.AddSerializer(serializer);

            // Act
            var hasSerializer = cache.SerializerFor("test", out var returnedSerializer);

            // Assert
            Assert.That(returnedSerializer, Is.SameAs(serializer));
        }
    }
}
