using System;

namespace Pigeon.Receivers
{
    /// <summary>
    /// Delegate for sending error messages back to the client
    /// </summary>
    /// <param name="ex"><see cref="Exception"/> to send</param>
    public delegate void ErrorSenderDelegate(Exception ex);
}
