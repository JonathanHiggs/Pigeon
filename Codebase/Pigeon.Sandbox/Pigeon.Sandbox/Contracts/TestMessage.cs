using System;

using Pigeon.Annotations;

namespace Pigeon.Sandbox.Contracts
{
    [Serializable]
    [Request(ResponseType = typeof(TestMessage))]
    public class TestMessage
    {
        public int Num { get; set; }
    }
}
