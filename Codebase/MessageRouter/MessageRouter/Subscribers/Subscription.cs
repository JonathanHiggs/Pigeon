using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessageRouter.Subscribers
{
    public class Subscription : IDisposable
    {
        private readonly Type topicType;
        private Action onUnsubscribe;
        private ISubscriber subscriber;


        public Type TopicType => topicType;


        public Subscription(ISubscriber subscriber, Type topicType, Action onUnsubscribe)
        {
            this.topicType = topicType ?? throw new ArgumentNullException(nameof(topicType));
            this.subscriber = subscriber ?? throw new ArgumentNullException(nameof(subscriber));
            this.onUnsubscribe = onUnsubscribe ?? throw new ArgumentNullException(nameof(onUnsubscribe));
        }


        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls


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


        public void Dispose() => Dispose(true);
        #endregion
    }
}
