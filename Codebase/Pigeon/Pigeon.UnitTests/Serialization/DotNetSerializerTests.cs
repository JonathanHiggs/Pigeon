using System;
using System.Linq;

using NUnit.Framework;

using Pigeon.Serialization;

namespace Pigeon.UnitTests.Serialization
{
    [TestFixture]
    public class DotNetSerializerTests
    {
        [Test]
        public void Serialize_WithString_ProducesByteArray()
        {
            // Arrange
            var serializer = new DotNetSerializer();
            var str = "Some string";

            // Act
            var bytes = serializer.Serialize(str);

            // Assert
            Assert.That(bytes, Is.Not.Null);
            CollectionAssert.IsNotEmpty(bytes);
        }


        [Test]
        public void Serialize_WithStringAndOffset_ConsistentWithNoOffset()
        {
            // Arrange
            var serializer = new DotNetSerializer();
            var offset = (new Random()).Next(100);
            var str = "Some string";

            // Act
            var bytes1 = serializer.Serialize(str);
            var bytes2 = serializer.Serialize(str, offset).Skip(offset).ToArray();

            // Assert
            Assert.AreEqual(bytes1, bytes2);
        }


        [Test]
        public void Deserialize_WithStringData_ReproducesString()
        {
            // Arrange
            var serializer = new DotNetSerializer();
            var str = "Some string";
            var bytes = serializer.Serialize(str);

            // Act
            var deserializedStr = serializer.Deserialize<string>(bytes);

            // Assert
            Assert.That(deserializedStr, Is.EqualTo(str));
        }


        [Test]
        public void Deserialize_WithStringDataZeroOffset_ReproducesString()
        {
            // Arrange
            var serializer = new DotNetSerializer();
            var str = "Some string";
            var data = serializer.Serialize(str);

            // Act
            var deserializedStr = serializer.Deserialize<string>(data, 0);

            // Assert
            Assert.AreEqual(str, deserializedStr);
        }


        [Test]
        public void Deserialize_WithStringDataOffset_ReproducesString()
        {
            // Arrange
            var serializer = new DotNetSerializer();
            var str = "Some string";
            var offset = (new Random()).Next(100);
            var data = serializer.Serialize(str, offset);

            // Act
            var deserializedStr = serializer.Deserialize<string>(data, offset);

            // Assert
            Assert.AreEqual(str, deserializedStr);
        }
    }
}
