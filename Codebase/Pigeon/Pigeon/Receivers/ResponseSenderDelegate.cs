namespace Pigeon.Receivers
{
    /// <summary>
    /// Delegate for sending response messages back to the client
    /// </summary>
    /// <param name="response">Response message to send</param>
    public delegate void ResponseSenderDelegate(object response);
}
