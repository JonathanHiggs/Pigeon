using System;

using Pigeon.Annotations;

namespace Pigeon.UnitTests.TestFixtures
{
    [Serializable]
    [Request(ResponseType = typeof(Response))]
    public class Request
    { }
}
