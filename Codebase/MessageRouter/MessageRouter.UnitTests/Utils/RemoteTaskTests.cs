using MessageRouter.Utils;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;

namespace MessageRouter.UnitTests.Utils
{
    [TestFixture]
    public class RemoteTaskTests
    {
        [Test]
        public void RemoteTask_WithNullTaskCompletionSource_ThrowsArgumentNullException()
        {
            // Arrange
            var taskCompletionSource = new TaskCompletionSource<object>();
            var timeout = TimeSpan.FromMilliseconds(5);
            Func<Exception> onTimeout = () => new TimeoutException();

            // Act
            TestDelegate construct = () => new RemoteTask<object>(null, timeout, onTimeout);

            // Assert
            Assert.That(construct, Throws.ArgumentNullException);
        }


        [Test]
        public void RemoteTask_WithNegativeTimeout_ThrowsArgumentException()
        {
            // Arrange
            var taskCompletionSource = new TaskCompletionSource<object>();
            var timeout = TimeSpan.FromMilliseconds(-5);
            Func<Exception> onTimeout = () => new TimeoutException();

            // Act
            TestDelegate construct = () => new RemoteTask<object>(taskCompletionSource, timeout, onTimeout);

            // Assert
            Assert.That(construct, Throws.ArgumentException);
        }


        [Test]
        public void RemoteTask_WithNullTimeoutHandler_ThrowsArgumentNullException()
        {
            // Arrange
            var taskCompletionSource = new TaskCompletionSource<object>();
            var timeout = TimeSpan.FromMilliseconds(5);
            Func<Exception> onTimeout = null;

            // Act
            TestDelegate construct = () => new RemoteTask<object>(taskCompletionSource, timeout, onTimeout);

            // Assert
            Assert.That(construct, Throws.ArgumentNullException);
        }


        [Test]
        public void RemoteTask_WithNoResultSet_InvokesTimeoutHandler()
        {
            // Arrange
            var handlerCalled = false;
            var wait = new ManualResetEvent(false);
            var taskCompletionSource = new TaskCompletionSource<object>();
            var timeout = TimeSpan.FromMilliseconds(5);

            Func<Exception> onTimeout = () => 
            {
                handlerCalled = true;
                wait.Set();
                return new TimeoutException();
            };

            // Act
            var remoteTask = new RemoteTask<object>(taskCompletionSource, timeout, onTimeout);

            wait.WaitOne(TimeSpan.FromMilliseconds(100));

            // Assert
            Assert.That(handlerCalled, Is.True);
        }


        [Test]
        public async Task RemoteTask_WithResultSet_DoesNotInvokeTimeoutHandler()
        {
            // Arrange
            var handlerCalled = false;
            var taskCompletionSource = new TaskCompletionSource<object>();
            var timeout = TimeSpan.FromMilliseconds(5);
            Func<Exception> onTimeout = () => new TimeoutException();
            var remoteTask = new RemoteTask<object>(taskCompletionSource, timeout, onTimeout);

            // Act
            remoteTask.CompleteWithResult(null);
            await Task.Delay(TimeSpan.FromMilliseconds(7));

            // Assert
            Assert.That(handlerCalled, Is.False);
        }
    }
}
