using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pigeon.Topics
{
    public interface IDITopicDispatcher : ITopicDispatcher
    {
        void Register<TTopic, THandler>()
            where THandler : ITopicHandler<TTopic>;
    }
}
