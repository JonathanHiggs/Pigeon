using MessageRouter.Server;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessageRouter.UnitTests.Server
{
    [TestFixture]
    public class RequestDispatcherTests
    {
        #region Request Classes
        #pragma warning disable CS0169 
        class Request
        {
            int Field1;
        }

        class Request2 : Request
        {
            int Field2;
        }
        #pragma warning restore CS0169
        #endregion


        #region Handle
        [Test]
        public void Handle_WithNull_ThrowsArgumentNullException()
        {
            // Arrange
            var dispatcher = new RequestDispatcher();

            // Act
            TestDelegate handleNull = () => dispatcher.Handle(null);

            // Assert
            Assert.That(handleNull, Throws.ArgumentNullException);
        }


        [Test]
        public void Handle_WithNoRegisteredHandler_ThrowsInvalidOperationException()
        {
            // Arrange
            var dispatcher = new RequestDispatcher();

            // Act
            TestDelegate handleUnregistered = () => dispatcher.Handle(DateTime.Now);

            // Assert
            Assert.That(handleUnregistered, Throws.TypeOf<InvalidOperationException>());
        }


        [Test]
        public void Handle_WithHandlerRegistered_CallsHandler()
        {
            // Arrange
            var handled = false;
            var dispatcher = new RequestDispatcher();
            dispatcher.Register<DateTime, DateTime>(dt => { handled = true; return dt; });

            // Act
            var response = dispatcher.Handle(DateTime.Now);

            // Assert
            Assert.That(handled, Is.True);
        }


        [Test]
        public void Handle_WithBaseClassRegisterHandled_ThrowsInvalidOperationException()
        {
            // Arrange
            var dispatcher = new RequestDispatcher();
            dispatcher.Register<Request, Request>(r => r);

            // Act
            TestDelegate handle = () => dispatcher.Handle(new Request2());

            // Assert
            Assert.That(handle, Throws.TypeOf<InvalidOperationException>());
        }
        #endregion
    }
}
