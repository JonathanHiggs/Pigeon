using NUnit.Framework;

using Pigeon.UnitTests.Helpers;

using static Pigeon.Addresses.TcpAddress;

namespace Pigeon.UnitTests.Addresses
{
    [TestFixture]
    public class TcpAddressTests
    {
        [Test, Combinatorial]
        public void FromNameAndPort_WithDNSNonComplientName_ThrowsArgumentException([Values(null, "", "01010", "A0c-", "-A0c", " space")] string name)
        {
            // Act
            TestDelegate test = () => FromNameAndPort(name, 5555);

            // Assert
            Assert.That(test, Throws.ArgumentException);
        }


        [Test, Combinatorial]
        public void FromNameAndPort_WithValidName_DoesNotThrowException(
            [Values("*", "host", "sub.host", "127.0.0.1", "some-host")] string name)
        {
            // Act
            TestDelegate test = () => FromNameAndPort(name, 5555);

            // Assert
            Assert.DoesNotThrow(test);
        }


        [Test, Combinatorial]
        public void NamePattern_WithValidAddressStrings_IsMatch(
            [Values("*", "host", "some.name")] string addressString)
        {
            // Assert
            RegexAssert.IsMatch(addressString, NamePattern);
        }


        [Test, Combinatorial]
        public void NamePattern_WithInvalidAddressStrings_IsNotMatch(
            [Values("hi*", "somethign:123", "as?asd", "0somethign", "somethign.0asdk")] string addressString)
        {
            // Assert
            RegexAssert.IsNotMatch(addressString, NamePattern);
        }


        [Test]
        public void IPPattern_WithValidIP_IsMatch(
            [Values("1.2.3.5", "255.255.255.255")] string ip)
        {
            // Assert
            RegexAssert.IsMatch(ip, IPPattern);
        }


        [Test]
        public void IPPattern_WithInvalidIP_IsNotMatch(
            [Values("1.2.3", "255.25a5.255.255")] string ip)
        {
            // Assert
            RegexAssert.IsNotMatch(ip, IPPattern);
        }


        [Test]
        public void Equals_WithSameAddress_IsTrue()
        {
            // Arrange
            var name = "name";
            ushort port = 5555;
            var a1 = FromNameAndPort(name, port);
            var a2 = FromNameAndPort(name, port);

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
            var a1 = FromNameAndPort("name", 5555);
            var a2 = FromNameAndPort("othername", 5555);

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
            var a1 = FromNameAndPort("name", 5555);
            var a2 = FromNameAndPort("name", 5556);

            // Act
            var equal1 = a1.Equals(a2);
            var equal2 = a2.Equals(a1);

            // Assert
            Assert.That(equal1, Is.False);
            Assert.That(equal2, Is.False);
        }


        [Test]
        public void Equals_WithHttpAddress_IsFalse()
        {
            // Arrange
            var a1 = HttpAddress.Named("google.com");
            var a2 = FromNameAndPort("google.com", 80);

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
            var address = FromNameAndPort("name", 5555);

            // Act
            var equals = address.Equals(null);

            // Assert
            Assert.That(equals, Is.False);
        }
    }
}
