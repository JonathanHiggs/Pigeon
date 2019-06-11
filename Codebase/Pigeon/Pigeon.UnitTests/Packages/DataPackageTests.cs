using NUnit.Framework;

using Pigeon.Packages;

namespace Pigeon.UnitTests.Packages
{
    [TestFixture]
    public class DataPackageTests
    {
        private readonly IPackageId id = new GuidPackageId();
        private readonly object data = new object();


        [Test]
        public void DataMessage_WithRequiredFields_InitializesObject()
        {
            // Act
            var package = new DataPackage<object>(id, data);

            // Assert
            Assert.AreSame(id, package.Id);
            Assert.AreSame(data, package.Data);
        }


        [Test]
        public void DataMessage_WithNullId_ThrowsArgumentNullException()
        {
            // Act
            TestDelegate construct = () => new DataPackage<object>(null, data);

            // Assert
            Assert.That(construct, Throws.ArgumentNullException);
        }


        [Test]
        public void DataMessage_WithNullData_ThrowsArgumentNullException()
        {
            // Act
            TestDelegate construct = () => new DataPackage<object>(id, null);

            // Assert
            Assert.That(construct, Throws.ArgumentNullException);
        }
       

        [Test]
        public void Body_WithSuppliedData_ReturnsSameInstance()
        {
            // Arrange
            var package = new DataPackage<object>(id, data);

            // Act
            var body = package.Body;

            // Assert
            Assert.AreSame(body, data);
        }


        [Test]
        public void Data_WithSuppliedData_ReturnsSameInstance()
        {
            // Arrange
            var package = new DataPackage<object>(id, data);

            // Act
            var dataProperty = package.Data;

            // Assert
            Assert.AreSame(dataProperty, data);
        }
    }
}
