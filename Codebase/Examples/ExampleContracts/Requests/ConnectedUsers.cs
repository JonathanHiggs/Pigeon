using System;
using System.ComponentModel;

using ExampleContracts.Responses;

using Pigeon.Annotations;

namespace ExampleContracts.Requests
{
    [Serializable]
    [ImmutableObject(true)]
    [Request(ResponseType = typeof(ConnectedUserList))]
    public class ConnectedUsers
    { }
}
