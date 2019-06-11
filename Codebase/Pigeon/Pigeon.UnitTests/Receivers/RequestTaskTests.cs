using System;

using NUnit.Framework;

using Pigeon.Packages;
using Pigeon.Receivers;

namespace Pigeon.UnitTests.Receivers
{
    [TestFixture]
    public class RequestTaskTests
    {
        [Test]
        public void RequestTask_WithAllRequiredFields_InitializesObject()
        {
            // Arrange
            var request = new object();
            Action<object> handler = _ => { };

            // Act
            var requestTask = new RequestTask(request, handler);

            // Assert
            Assert.AreSame(request, requestTask.Request);
            Assert.AreSame(handler, requestTask.ResponseHandler);
        }


        [Test]
        public void RequestTask_WithNullRequest_ThrowsArgumentNullException()
        {
            // Arrange
            Action<object> handler = _ => { };

            // Act
            TestDelegate test = () => new RequestTask(null, handler);

            // Assert
            Assert.That(test, Throws.ArgumentNullException);
        }


        [Test]
        public void RequestTask_WithNullRequestHandler_ThrowsArgumentNullException()
        {
            // Arrange
            var package = new DataPackage<object>(new GuidPackageId(), new object());

            // Act
            TestDelegate test = () => new RequestTask(package, null);

            // Assert
            Assert.That(test, Throws.ArgumentNullException);
        }
    }
}
