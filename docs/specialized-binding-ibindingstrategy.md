# Specialized Binding, Part 3 of 3 : IBindingStrategy

As explained in [point 2 of *A brief history of shortsightedness*](https://gist.github.com/nyingi/d2ef20be030a938b1adba77d9ec8a3b3#a-brief-history-of-shortsightedness), after starting with a lot of
optimism on building a library that supported automatic two-way binding for all Windows Forms components, it became clear after just doing four native Windows Forms components that even for
stuff well documented by Microsoft, building two-way binding was a taxing and probably undoable.

The biggest challange was that each component utilizes a distinct event to report user action. For example, Textboxes can utilize *TextChanged*, *KeyUp*, *KeyDown* and for added fun *KeyPress*.
For CheckBoxes, you have *CheckedChanged*, *CheckStateChanged*, *MouseClick*, *Clicked* and the same Keyboard events as a Textbox. This are just two components and already we are swamped by
possible events to subscribe to. Granted as a developer, you'll generally want to pick just one event per component to subscribe to, there isn't a foolproof way to pick the one event that all the 
developers everywhere will want to use for a given component.

Throw in custom components into the mix and it should be clear that there is not possible way to build automatic two-way binding for every single component.

To overcome help us overcome this huddle, we have the IBindingStrategy interface which helps us describe to DashMVVM how to handle user input for components.

## IBindingStrategy
```CSharp
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
```

### Points of Interest
The IBindingStrategy interface is composed of the following property and Method

* ViewType - This property returns the type of component the binding strategy applies to.

* SetupBind - This method configures how user input is to be treated for the components of the type specified in property *ViewType*

## Example Implementation of IBindingStrategy

For our example implementation, we'll assume we have a third party CheckBox that mimics the look and feel of Android's material user interfaces. Its binding strategy would look as follows:-

```CSharp
    class MaterialCheckboxBindingStrategy : IBindingStrategy
    {
        public Type ViewType
        {
            get
            {
                return typeof(MaterialCheckBox);
            }
        }

        public void SetupBind(object view, Action<object, PropertyInfo> viewChangedAction)
        {
            if (view == null)
            {
                throw new NullReferenceException("Cannot setup binding for a null object");
            }
            var chk = view as MaterialCheckBox;
            if (chk == null)
            {
                throw new Exception($"Cannot cast {view.GetType()} to {ViewType.Name}");
            }
            chk.CheckedChanged += (sender, e) =>
            {
                viewChangedAction?.Invoke(chk, chk.GetType().GetProperty("Checked"));
            };
        }
    }
```