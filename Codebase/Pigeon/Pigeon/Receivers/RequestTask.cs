using System;

namespace Pigeon.Receivers
{
    /// <summary>
    /// Data structure to combine an incoming request with a handler to return the response
    /// </summary>
    public readonly struct RequestTask
    {
        /// <summary>
        /// Stores a read-only reference to the <see cref="IReceiver"/>
        /// </summary>
        public readonly IReceiver Receiver;


        /// <summary>
        /// Stores a read-only reference to an incoming request message
        /// </summary>
        public readonly object Request;


        /// <summary>
        /// Stores a read-only reference to a delegate to send a response
        /// </summary>
        public readonly ResponseSenderDelegate ResponseSender;


        /// <summary>
        /// Stores a read-only reference to a delegate to send an error response
        /// </summary>
        public readonly ErrorSenderDelegate ErrorSender;
        

        /// <summary>
        /// Initializes a new instance of a RequestTask composed of the supplied request object and handler
        /// </summary>
        /// <param name="receiver"><see cref="IReceiver"/> that received the request message</param>
        /// <param name="request">Incoming request message</param>
        /// <param name="responseSender">Delegate to return the response to the client</param>
        /// <param name="errorSender">Delegate to return an error response to the client</param>
        public RequestTask(IReceiver receiver, object request, ResponseSenderDelegate responseSender, ErrorSenderDelegate errorSender)
        {
            Receiver = receiver ?? throw new ArgumentNullException(nameof(receiver));
            Request = request ?? throw new ArgumentNullException(nameof(request));
            ResponseSender = responseSender ?? throw new ArgumentNullException(nameof(responseSender));
            ErrorSender = errorSender ?? throw new ArgumentNullException(nameof(errorSender));
        }
    }
}
