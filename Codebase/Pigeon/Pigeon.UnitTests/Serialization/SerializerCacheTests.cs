using System.Linq;

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

        private readonly SerializationDescriptor descriptor = new SerializationDescriptor("test");


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


        #region DefaultSerializer

        [Test]
        public void DefaultSerializer_WithNoneAdded_IsNull()
        {
            // Arrange
            var cache = new SerializerCache();

            // Act
            var defaultSerializer = cache.DefaultSerializer;

            // Assert
            Assert.That(defaultSerializer, Is.Null);
        }

        #endregion


        #region AddSerializer

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
        public void AddSerializer_WithNoOtherSerializers_SetsToDefault()
        {
            // Arrange
            var cache = new SerializerCache();

            // Act
            cache.AddSerializer(serializer);
            var defaultSerializer = cache.DefaultSerializer;

            // Assert
            Assert.That(serializer, Is.EqualTo(defaultSerializer));
        }


        [Test]
        public void AddSerializer_WithSameSerializerAlreadyInCache_DoesNotAddDuplicate()
        {
            // Arrange
            var cache = new SerializerCache();
            cache.AddSerializer(serializer);

            // Act
            cache.AddSerializer(serializer);

            // Assert
            Assert.That(cache.AllSerializers.Count(), Is.EqualTo(1));
        }


        [Test]
        public void AddSerializer_WithDifferentInstancePOfSerializerAlreadyInCache_DoesNotAddDuplicate()
        {
            // Arrange
            var cache = new SerializerCache();
            var otherMockSerializer = new Mock<ISerializer>();
            otherMockSerializer.Setup(m => m.Descriptor).Returns(descriptor);
            cache.AddSerializer(serializer);

            // Act
            cache.AddSerializer(otherMockSerializer.Object);

            // Assert
            Assert.That(cache.AllSerializers.Count(), Is.EqualTo(1));
        }

        #endregion


        #region SerializerFor

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


        [Test]
        public void SerializerFor_WithoutSerializerAdded_ReturnsFalse()
        {
            // Arrange
            var cache = new SerializerCache();

            // Act
            var hasSerializer = cache.SerializerFor("test", out var returnedSerializer);

            // Assert
            Assert.That(hasSerializer, Is.False);
            Assert.That(returnedSerializer, Is.Null);
        }

        #endregion


        #region SetDefaultSerializer

        [Test]
        public void SetDefaultSerializer_WithSerializer_AddsToCache()
        {
            // Arrange
            var cache = new SerializerCache();

            // Act
            cache.SetDefaultSerializer(serializer);
            var hasSerializer = cache.SerializerFor(descriptor, out var returnedSerializer);

            // Assert
            Assert.That(hasSerializer, Is.True);
            Assert.That(returnedSerializer, Is.EqualTo(serializer));
        }

        #endregion
    }
}
