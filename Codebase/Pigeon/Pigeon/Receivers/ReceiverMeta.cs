using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Pigeon.Addresses;

namespace Pigeon.Receivers
{
    [Serializable]
    public class ReceiverMeta
    {
        public List<IAddress> Addresses { get; private set; }
        public Type Type { get; private set; }


        public ReceiverMeta(Type type, IEnumerable<IAddress> addresses)
        {
            Type = type;
            Addresses = addresses.ToList();
        }
    }
}
