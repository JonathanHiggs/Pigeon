using System;

using NUnit.Framework;

using Pigeon.Addresses;

namespace Pigeon.UnitTests.Addresses
{
    [TestFixture]
    public class HttpAddressTests
    {
        [Test]
        public void Named_WithNullName_ThrowsArgumentException()
        {
            // Act
            TestDelegate named = () => HttpAddress.Named(null);

            // Assert
            Assert.That(named, Throws.ArgumentException);
        }


        [Test]
        public void Named_WithEmptyName_ThrowsArgumentException()
        {
            // Act
            TestDelegate named = () => HttpAddress.Named(String.Empty);

            // Assert
            Assert.That(named, Throws.ArgumentException);
        }


        [Test]
        public void Equals_WithSameInstance_IsTrue()
        {
            // Arrange
            var a1 = HttpAddress.Named("google.com");
            var a2 = HttpAddress.Named("google.com");

            // Act
            var equal1 = a1.Equals(a2);
            var equal2 = a2.Equals(a1);

            // Assert
            Assert.That(equal1, Is.True);
            Assert.That(equal2, Is.True);
        }


        [Test]
        public void Equals_WithDifferentName_IsFalse()
        {
            // Arrange
            var a1 = HttpAddress.Named("google.com");
            var a2 = HttpAddress.Named("apple.com");

            // Act
            var equal1 = a1.Equals(a2);
            var equal2 = a2.Equals(a1);

            // Assert
            Assert.That(equal1, Is.False);
            Assert.That(equal2, Is.False);
        }


        [Test]
        public void Equals_WithDifferentPort_IsFalse()
        {
            // Arrange
            var a1 = HttpAddress.Named("google.com", 80);
            var a2 = HttpAddress.Named("google.com", 8080);

            // Act
            var equal1 = a1.Equals(a2);
            var equal2 = a2.Equals(a1);

            // Assert
            Assert.That(equal1, Is.False);
            Assert.That(equal2, Is.False);
        }


        [Test]
        public void Equals_WithTcpAddress_IsFalse()
        {
            // Arrange
            var a1 = HttpAddress.Named("google.com");
            var a2 = TcpAddress.FromNameAndPort("google.com", 80);

            // Act
            var equal1 = a1.Equals(a2);
            var equal2 = a2.Equals(a1);

            // Assert
            Assert.That(equal1, Is.False);
            Assert.That(equal2, Is.False);
        }


        [Test]
        public void Equals_WithNull_IsFalse()
        {
            // Arrange
            var address = HttpAddress.WebHost;

            // Act
            var equals = address.Equals(null);

            // Assert
            Assert.That(equals, Is.False);
        }
    }
}
