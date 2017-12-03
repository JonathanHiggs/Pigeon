﻿using MessageRouter.NetMQ.Senders;
using NetMQ;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MessageRouter.NetMQ.UnitTests.Senders
{
    [TestFixture]
    public class NetMQTaskTests
    {
        [Test]
        public void NetMQTask_WithNullTaskCompletionSource_ThrowsArgumentNullException()
        {
            // Act
            TestDelegate construct = () => new NetMQTask(null, 1.0, (s, e) => { });

            // Assert
            Assert.That(construct, Throws.ArgumentNullException);
        }


        [Test]
        public void NetMQTask_WithNegativeTimeout_ThrowsArgumentException()
        {
            // Arrange
            var taskCompletionSource = new TaskCompletionSource<NetMQMessage>();

            // Act
            TestDelegate construct = () => new NetMQTask(taskCompletionSource, -1.0, (s, e) => { });

            // Assert
            Assert.That(construct, Throws.ArgumentException);
        }


        [Test]
        public void NetMQTask_WithNullElapsedEventHandler_ThrowsArgumentNullException()
        {
            // Arrange
            var taskCompletionSource = new TaskCompletionSource<NetMQMessage>();

            // Act
            TestDelegate construct = () => new NetMQTask(taskCompletionSource, 1.0, null);

            // Assert
            Assert.That(construct, Throws.ArgumentNullException);
        }


        [Test]
        public void NetMQTask_AfterTimeout_CallsElapsedEventHandler()
        {
            // Arrange
            var called = false;
            var taskCompletionSource = new TaskCompletionSource<NetMQMessage>();
            var task = new NetMQTask(taskCompletionSource, 1.0, (s, e) => { called = true; });

            // Act
            Thread.Sleep(10);

            // Assert
            Assert.That(called, Is.True);
        }
    }
}
