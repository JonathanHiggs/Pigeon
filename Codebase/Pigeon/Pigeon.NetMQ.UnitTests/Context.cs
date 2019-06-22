using NetMQ;

using NUnit.Framework;

namespace Pigeon.NetMQ.UnitTests
{
    [SetUpFixture]
    public class Context
    {
        [OneTimeSetUp]
        public void OneTimeSetUp()
        {    
        }


        [OneTimeTearDown]
        public void OneTimeTearDown()
        {
            NetMQConfig.Cleanup(false);
        }
    }
}