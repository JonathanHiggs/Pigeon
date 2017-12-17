using System;
using NUnit.Framework;
using MessageRouter.Packages;
using MessageRouter.Receivers;

namespace MessageRouter.UnitTests.Receivers
{
    [TestFixture]
    public class RequestTaskTests
    {
        [Test]
        public void RequestTask_WithAllRequiredFields_InitializesObject()
        {
            // Arrange
            var package = new DataPackage<object>(new GuidPackageId(), new object());
            Action<Package> handler = _ => { };

            // Act
            var requestTask = new RequestTask(package, handler);

            // Assert
            Assert.AreSame(package, requestTask.Request);
            Assert.AreSame(handler, requestTask.ResponseHandler);
        }


        [Test]
        public void RequestTask_WithNullRequest_ThrowsArgumentNullException()
        {
            // Arrange
            Action<Package> handler = _ => { };

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
