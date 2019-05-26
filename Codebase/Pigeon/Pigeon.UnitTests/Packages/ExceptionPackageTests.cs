using Pigeon.Packages;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pigeon.UnitTests.Packages
{
    [TestFixture]
    public class ExceptionPackageTests
    {
        private readonly IPackageId id = new GuidPackageId();
        private readonly Exception exception = new Exception();


        [Test]
        public void ExceptionMessage_WithNullId_ThrowsArgumentNullException()
        {
            // Act
            TestDelegate construct = () => new ExceptionPackage(null, exception);

            // Assert
            Assert.That(construct, Throws.ArgumentNullException);
        }


        [Test]
        public void ExceptionMessage_WithNullException_ThrowsArgumentNullException()
        {
            // Act
            TestDelegate construct = () => new ExceptionPackage(id, null);

            // Assert
            Assert.That(construct, Throws.ArgumentNullException);
        }


        [Test]
        public void Body_WithExcepionSupplied_ReturnsSameInstance()
        {
            // Arrange
            var package = new ExceptionPackage(id, exception);

            // Act
            var body = package.Body;

            // Assert
            Assert.AreSame(body, exception);
        }


        [Test]
        public void Exception_WithExceptionSupplied_ReturnsSameInstance()
        {
            // Arrange
            var package = new ExceptionPackage(id, exception);

            // Act
            var exceptionProperty = package.Exception;

            // Assert
            Assert.AreSame(exceptionProperty, exception);
        }
    }
}
