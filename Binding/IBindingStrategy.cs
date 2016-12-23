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

        /// <summary>
        /// This method configures how user input is to be treated for the components of the type specified in property ViewType
        /// </summary>
        /// <param name="view">The user component gets passed into the binding strategy</param>
        /// <param name="viewChangedAction">An action that describes what to do when the view changes</param>
        void SetupBind(object view, Action<object, PropertyInfo> viewChangedAction);
    }
}
