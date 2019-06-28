using System;

namespace Pigeon.Subscribers
{
    /// <summary>
    /// Represents an active subscription to facilatate tracking and cleanup of resources
    /// </summary>
    public class Subscription : IDisposable
    {
        private Action onUnsubscribe;
        private ISubscriber subscriber;


        /// <summary>
        /// Initializes a new instance of <see cref="Subscription"/>
        /// </summary>
        /// <param name="subscriber">The <see cref="ISubscriber"/> that receives the topic messages</param>
        /// <param name="topicType">The type of the topic messages for this subscription</param>
        /// <param name="subject">Topic subject name</param>
        /// <param name="onUnsubscribe">An action that will be performed when the subscription is terminated</param>
        public Subscription(ISubscriber subscriber, Type topicType, string subject, Action onUnsubscribe)
        {
            TopicType = topicType ?? throw new ArgumentNullException(nameof(topicType));
            Subject = subject;
            this.subscriber = subscriber ?? throw new ArgumentNullException(nameof(subscriber));
            this.onUnsubscribe = onUnsubscribe ?? throw new ArgumentNullException(nameof(onUnsubscribe));
        }


        /// <summary>
        /// Gets the subscription topic subject
        /// </summary>
        public string Subject { get; }


        /// <summary>
        /// The type of the topic message for this subscription
        /// </summary>
        public Type TopicType { get; private set; }


        #region IDisposable Support

        private bool disposedValue = false; // To detect redundant calls


        /// <summary>
        /// Performs the subscription cleanup action supplied during initialization and free other resources
        /// </summary>
        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    onUnsubscribe();
                    subscriber = null;
                    onUnsubscribe = null;
                }

                disposedValue = true;
            }
        }


        /// <summary>
        /// Performs the subscription cleanup action supplied during initialization and free other resources
        /// </summary>
        public void Dispose() => Dispose(true);

        #endregion
    }
}
