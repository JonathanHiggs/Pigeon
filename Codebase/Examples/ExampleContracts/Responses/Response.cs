using System;
using System.ComponentModel;

namespace ExampleContracts.Responses
{
    [Serializable]
    [ImmutableObject(true)]
    public class Response<T>
    {
        public Response(T body, bool success, string reason)
        {
            Body = body;
            Success = success;
            Reason = reason;
        }


        public static Response<T> Ok(T body) =>
            new Response<T>(body, true, string.Empty);


        public static Response<T> Failure(T body, string reason) =>
            new Response<T>(body, false, reason);


        public T Body { get; }
        public bool Success { get; }
        public string Reason { get; }
    }
}
