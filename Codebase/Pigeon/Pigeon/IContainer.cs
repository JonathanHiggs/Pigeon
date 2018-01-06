using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pigeon.Requests;

namespace Pigeon
{
    public interface IContainer
    {
        T Resolve<T>();

        void Register<T>(bool singleton);
        void Register<T>(T instance);
        void Register<TBase, TImpl>(bool singleton) where TImpl : TBase;

        bool IsRegistered<T>();
    }
}
