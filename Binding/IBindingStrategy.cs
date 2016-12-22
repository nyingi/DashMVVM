using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace DashMvvm.Binding
{
    public interface IBindingStrategy
    {
        Type ViewType { get; }
        void SetupBind(object view, Action<object, PropertyInfo> viewChangedAction);
    }
}
