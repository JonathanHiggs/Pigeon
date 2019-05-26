using System;
using NUnit.Framework;
using Pigeon.Packages;

namespace Pigeon.UnitTests.Packages
{
    [TestFixture]
    public class GuidPackageIdTests
    {
        [Test]
        public void Equals_WithSameGuid_ReturnsTrue()
        {
            // Arrange
            var guid = Guid.NewGuid();
            var package1 = new GuidPackageId(guid);
            var package2 = new GuidPackageId(guid);

            // Act
            var equal = package1.Equals(package2);

            // Assert
            Assert.That(equal, Is.True);
        }


        [Test]
        public void Equals_WithDifferentGuids_ReturnsFalse()
        {
            // Arrange
            var package1 = new GuidPackageId();
            var package2 = new GuidPackageId();

            // Act
            var equal = package1.Equals(package2);

            // Assert
            Assert.That(equal, Is.False);
        }
    }
}
