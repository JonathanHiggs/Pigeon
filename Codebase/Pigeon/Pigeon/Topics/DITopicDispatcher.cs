using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pigeon.Diagnostics;
using Pigeon.Utils;

namespace Pigeon.Topics
{
    public class DITopicDispatcher : TopicDispatcher, IDITopicDispatcher
    {
        private IContainer container;


        public DITopicDispatcher(IContainer container)
        {
            this.container = container ?? throw new ArgumentNullException(nameof(container));
        }


        public void Register<TTopic, THandler>() where THandler : ITopicHandler<TTopic>
        {
            Validate<TTopic>();
            if (!container.IsRegistered<THandler>())
                throw new NotRegisteredException(typeof(THandler));

            handlers.Add(typeof(TTopic), eventMessage => Task.Run(() => container.Resolve<THandler>().Handle((TTopic)eventMessage)));
        }
    }
}
