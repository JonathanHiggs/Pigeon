using Pigeon.Addresses;
using Pigeon.UnitTests.Helpers;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            var equals = a1.Equals(a2);

            // Assert
            Assert.That(equals, Is.True);
        }
    }
}
