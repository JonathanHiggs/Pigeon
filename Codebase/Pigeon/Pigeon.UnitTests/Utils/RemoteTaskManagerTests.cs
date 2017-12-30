using Pigeon.Utils;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pigeon.UnitTests.Utils
{
    [TestFixture]
    public class RemoteTaskManagerTests
    {
        private readonly int firstId = 1;
        private readonly Func<int, int> nextId = i => i + 1;
        private readonly Action<int> action = i => { };
        private readonly TimeSpan timeout = TimeSpan.FromMilliseconds(5);
        private readonly object result = new object();


        [Test]
        public void RemoteTaskManager_WithNullGeneratorFunction_ThrowsArgumentNullException()
        {
            // Act
            TestDelegate construct = () => new RemoteTaskManager<object, int>(1, null);

            // Assert
            Assert.That(construct, Throws.ArgumentNullException);
        }


        [Test]
        public void StartRemoteTask_WithNoResponse_TimesOut()
        {
            // Arrange
            var taskManager = new RemoteTaskManager<object, int>(firstId, nextId);

            // Act
            AsyncTestDelegate start = async () => await taskManager.StartRemoteTask(action, timeout);

            // Assert
            Assert.ThrowsAsync<TimeoutException>(start);
        }


        [Test]
        public async Task StartRemoteTask_WithResult_ReturnsResult()
        {
            // Arrange
            var taskManager = new RemoteTaskManager<object, int>(firstId, nextId);
            var id = 0;
            Action<int> action = i => { id = i; };

            // Act
            var task = taskManager.StartRemoteTask(action, timeout);
            taskManager.CompleteTask(id, result);

            var res = await task;

            // Assert
            Assert.That(res, Is.EqualTo(result));
        }


        [Test]
        public void StartTaskRemote_WithNegativeTimespan_ThrowsArgumentException()
        {
            // Arrange
            var taskManager = new RemoteTaskManager<object, int>(firstId, nextId);

            // Act
            AsyncTestDelegate start = async () => await taskManager.StartRemoteTask(action, TimeSpan.FromMilliseconds(-1));

            // Assert
            Assert.ThrowsAsync<ArgumentException>(start);
        }


        [Test]
        public void CompleteTask_WithNoTaskStarted_DoesNothing()
        {
            // Arrange
            var taskManager = new RemoteTaskManager<object, int>(firstId, nextId);

            // Act
            TestDelegate complete = () => taskManager.CompleteTask(10, result);

            // Assert
            Assert.That(complete, Throws.Nothing);
        }
    }
}
