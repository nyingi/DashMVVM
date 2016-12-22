using System;
using System.Reflection;
using System.Windows.Forms;
using DashMvvm;

namespace DashMvvm.Binding.Components
{
    public class CheckboxHelper
    {
        private Action<object, object, PropertyInfo, PropertyInfo> _setValue;
        
        public CheckboxHelper(Action<object, object, PropertyInfo, PropertyInfo> setValue)
        {
            _setValue = setValue;
        }

        public void Bind(CheckBox chk, object viewModel, PropertyInfo viewModelProperty)
        {
            
            chk.CheckStateChanged += (sender, args) =>
            {
                viewModelProperty.SetValue(viewModel, chk.Checked);
            };

            (viewModel as DashViewModel).PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName == viewModelProperty.Name)
                {
                    _setValue(viewModel, chk, viewModelProperty, chk.GetType().GetProperty("Checked"));
                }
            };
            _setValue(viewModel, chk, viewModelProperty, chk.GetType().GetProperty("Checked"));
        }
    }
}