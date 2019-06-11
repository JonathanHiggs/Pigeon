using System;
using System.Collections.Generic;

using NUnit.Framework;

using Pigeon.Packages;

namespace Pigeon.UnitTests.Packages
{
    [TestFixture]
    public class PackageFactoryTests
    {
        private readonly PackageFactory factory = new PackageFactory();


        #region Pack
        [Test]
        public void Pack_WithObject_ReturnsMessage()
        {
            // Arrange
            var request = new object();

            // Act
            var package = factory.Pack<object>(request);

            // Assert
            Assert.IsNotNull(package);
        }


        [Test]
        public void Pack_WithMessage_ReturnsSameMessage()
        {
            // Arrange
            var dataPackage = new DataPackage<object>(new GuidPackageId(), new object());

            // Act
            var package = factory.Pack<Package>(dataPackage);

            // Assert
            Assert.AreSame(dataPackage, package);
        }


        [Test]
        public void Pack_WithMessageDerivative_ReturnsSameMessage()
        {
            // Arrange
            var dataPackage = new DataPackage<object>(new GuidPackageId(), new object());

            // Act
            var package = factory.Pack(dataPackage);

            // Assert
            Assert.AreSame(dataPackage, package);
        }
        #endregion


        #region Unpack
        [Test]
        public void Unpack_WithWrappedObject_ReturnsObject()
        {
            // Arrange
            var message = new object();
            var dataPackage = new DataPackage<object>(new GuidPackageId(), message);

            // Act
            var extractedMessage = factory.Unpack(dataPackage);

            // Assert
            Assert.AreSame(message, extractedMessage);
        }
        #endregion


        #region UnpackGeneric
        [Test]
        public void UnpackGeneric_WithWrappedObject_ReturnsObject()
        {
            // Arrange
            var message = new object();
            var dataPackage = new DataPackage<object>(new GuidPackageId(), message);

            // Act
            var extractedMessage = factory.Unpack<object>(dataPackage);

            // Assert
            Assert.AreSame(message, extractedMessage);
        }


        [Test]
        public void UnpackGeneric_WithPackage_ReturnsPackage()
        {
            // Arrange
            var dataPackage = new DataPackage<object>(new GuidPackageId(), new object());

            // Act
            var message = factory.Unpack<Package>(dataPackage);

            // Assert
            Assert.AreSame(dataPackage, message);
        }


        [Test]
        public void UnpackGeneric_WithInnerPackage_ReturnsInnerPackage()
        {
            // Arrange
            var innerPackage = new DataPackage<object>(new GuidPackageId(), new object());
            var dataPackage = new DataPackage<DataPackage<object>>(new GuidPackageId(), innerPackage);

            // Act
            var message = factory.Unpack<DataPackage<object>>(dataPackage);

            // Assert
            Assert.AreSame(innerPackage, message);
        }


        [Test]
        public void UnpackGeneric_WithDifferentWrappedType_ThrowsInvalidCastException()
        {
            // Arrange
            var dataPackage = new DataPackage<String>(new GuidPackageId(), String.Empty);

            // Act
            TestDelegate extract = () => factory.Unpack<List<String>>(dataPackage);

            // Assert
            Assert.That(extract, Throws.InstanceOf<InvalidCastException>());
        }


        [Test]
        public void UnpackGeneric_WithExceptionMessage_RethrowsException()
        {
            // Arrange
            var exceptionPackage = new ExceptionPackage(new GuidPackageId(), new Exception());

            // Act
            TestDelegate extract = () => factory.Unpack<List<String>>(exceptionPackage);

            // Assert
            Assert.That(extract, Throws.Exception);
        }


        [Test]
        public void UnpackGeneric_WithExceptionMessage_RethrowsException_2()
        {
            // Arrange
            var exceptionPackage = new ExceptionPackage(new GuidPackageId(), new Exception());

            // Act
            TestDelegate extract = () => factory.Unpack<Exception>(exceptionPackage);

            // Assert
            Assert.That(extract, Throws.Exception);
        }
        #endregion
    }
}
