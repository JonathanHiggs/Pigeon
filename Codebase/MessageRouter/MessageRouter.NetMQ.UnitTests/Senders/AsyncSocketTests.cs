using MessageRouter.NetMQ.Senders;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessageRouter.NetMQ.UnitTests.Senders
{
    [TestFixture]
    public class AsyncSocketTests
    {
        [Test]
        public void AsyncSocket_WithNullDealerSocket_ThrowsArgumentNullException()
        {
            // Act
            TestDelegate construct = () => new AsyncSocket(null);

            // Assert
            Assert.That(construct, Throws.ArgumentNullException);
        }
    }
}
