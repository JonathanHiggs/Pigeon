using NetMQ;

using NUnit.Framework;

using Pigeon.Fluent.Simple;
using Pigeon.Serialization;

namespace Pigeon.NetMQ.UnitTests
{
    [TestFixture]
    public class FluentConstruction
    {
        [Test]
        public void SimpleBuilder()
        {
            // Act
            var builder = Builder.WithName("name");

            builder.WithSerializer(new DotNetSerializer(), true);

            var transportFactory = NetMQTransport.FromBuilder(
                builder,
                NetMQFactory.FromBuilder(
                    builder,
                    new NetMQMonitor(new NetMQPoller()),
                    NetMQMessageFactory.FromBuilder(builder)));

            var router = builder.Build();

            // Assert
            Assert.That(router, Is.Not.Null);
        }
    }
}
