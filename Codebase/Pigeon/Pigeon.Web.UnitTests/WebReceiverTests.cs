using System.Net;
using System.Threading.Tasks;

using NUnit.Framework;

namespace Pigeon.Web.UnitTests
{
    [TestFixture]
    public class WebReceiverTests
    {
        private readonly HttpListener listener = new HttpListener();
        private readonly WebTaskHandler handler = (rec, context) => Task.CompletedTask;


        #region Constructor

        [Test]
        public void WebRecevier_WithNullListener_ThrowsArgumentNullException()
        {
            // Act
            TestDelegate construct = () => new WebReceiver(null, handler);

            // Assert
            Assert.That(construct, Throws.ArgumentNullException);
        }


        [Test]
        public void WebReceiver_WithNullHandler_ThrowsArgumentNullException()
        {
            // Act
            TestDelegate construct = () => new WebReceiver(listener, null);

            // Assert
            Assert.That(construct, Throws.ArgumentNullException);
        }


        [Test]
        public void WebReceiver_WithHandler_HandlerPropertyReturnsSame()
        {
            // Act
            var receiver = new WebReceiver(listener, handler);

            // Assert
            Assert.That(receiver.Handler, Is.SameAs(handler));
        }

        #endregion
    }
}
