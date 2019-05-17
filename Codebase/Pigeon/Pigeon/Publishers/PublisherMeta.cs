using System;
using System.Collections.Generic;
using System.Text;

namespace Pigeon.Publishers
{
    [Serializable]
    public class PublisherMeta
    {
        public Type Type { get; private set; }


        public PublisherMeta(Type type)
        {
            Type = type ?? throw new ArgumentNullException(nameof(type));
        }
    }
}
