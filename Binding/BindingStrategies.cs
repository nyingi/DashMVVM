using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DashMvvm.Binding
{
    public static class BindingStrategies
    {
        internal static List<IBindingStrategy> Strategies { get; set; } = new List<IBindingStrategy>();

        public static void AddBindingStrategy(IBindingStrategy bindingStrategy)
        {
            RemoveBindingStrategy(bindingStrategy);
            Strategies.Add(bindingStrategy);
        }

        public static void RemoveBindingStrategy(IBindingStrategy bindingStrategy)
        {
            Strategies = Strategies.Where(a => a.ViewType != bindingStrategy.ViewType).ToList();
        }
    }
}
