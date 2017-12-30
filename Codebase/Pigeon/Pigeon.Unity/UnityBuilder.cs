using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pigeon.Fluent;
using Unity;

namespace Pigeon.Unity
{
    public static class UnityBuilder
    {
        public static IFluentBuilder WithName(string name)
        {
            var container = new UnityContainerWrapper(new UnityContainer());
            return new DependencyInjectionBuilder(name, container);
        }
    }
}
