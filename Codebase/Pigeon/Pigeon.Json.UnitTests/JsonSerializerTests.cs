using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using NUnit.Framework;

using Pigeon.Packages;

namespace Pigeon.Json.UnitTests
{
    [TestFixture]
    public class JsonSerializerTests
    {
        private readonly JsonSerializer serializer = new JsonSerializer();
        private readonly TestObject testObject = new TestObject { Field = "some string" };


        [Test]
        public void Serialize_WithTestObject_ProducesByteArray()
        {
            // Act
            var bytes = serializer.Serialize(testObject);

            // Assert
            Assert.That(bytes, Is.Not.Null);
            CollectionAssert.IsNotEmpty(bytes);
        }


        [Test]
        public void Serialize_WithTestObjectAndOffset_ConsistentWithNoOffset()
        {
            // Arrange
            var offset = (new Random()).Next(100);

            // Act
            var bytes1 = serializer.Serialize(testObject);
            var bytes2 = serializer.Serialize(testObject, offset).Skip(offset).ToArray();

            // Assert
            Assert.That(bytes2, Is.EqualTo(bytes1));
        }


        [Test]
        public void Deserialize_WithTestObjectData_ReproducesTestObject()
        {
            // Arrange
            var bytes = serializer.Serialize(testObject);

            // Act
            var deserializedObject = serializer.Deserialize<TestObject>(bytes);

            // Assert
            Assert.That(deserializedObject, Is.EqualTo(testObject));
        }


        [Test]
        public void Deserialize_WithTestObjectDataZeroOffset_ReproducesTestObject()
        {
            // Arrange
            var data = serializer.Serialize(testObject);

            // Act
            var deserializedStr = (TestObject)serializer.Deserialize<TestObject>(data, 0);

            // Assert
            Assert.AreEqual(testObject, deserializedStr);
        }


        [Test]
        public void Deserialize_WithTestObjectDataOffset_ReproducesTestObject()
        {
            // Arrange
            var offset = (new Random()).Next(100);
            var data = serializer.Serialize(testObject, offset);

            // Act
            var deserializedStr = serializer.Deserialize<TestObject>(data, offset);

            // Assert
            Assert.AreEqual(testObject, deserializedStr);
        }


        [Test]
        public void Serialize_WithDataPackage()
        {
            // Arrange
            var package = new DataPackage<TestObject>(new GuidPackageId(), new TestObject { Field = "Some value" });

            // Act
            var data = serializer.Serialize(package);
            var str = Encoding.UTF8.GetString(data);
            var deserializedPackage = serializer.Deserialize<Package>(data);

        }


        class TestObject
        {
            public string Field { get; set; }

            public override bool Equals(object obj)
            {
                return obj is TestObject testObj && Field == testObj.Field;
            }

            public override int GetHashCode()
            {
                return 1998067999 + EqualityComparer<string>.Default.GetHashCode(Field);
            }
        }
    }
}
