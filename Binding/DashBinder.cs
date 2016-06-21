/*
 * Created by SharpDevelop.
 * User: Nyingi
 * Date: 5/27/2016
 * Time: 11:25 PM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Windows.Forms;
using DashMvvm.Validation;
using FeatherMvvm.Attributes;

namespace DashMvvm.Binding
{
	/// <summary>
	/// Description of BindingContext.
	/// </summary>
	public class DashBinder<TViewModel> where TViewModel : DashViewModel , new()
	{
		TViewModel _viewModel;
		DashView<TViewModel> _view;
		Dictionary<object,List<PropertyInfo>> _bindings;
		internal Validator Validator { get; set; }
		
		public DashBinder(TViewModel viewModel,DashView<TViewModel> view)
		{
			_viewModel = viewModel;
			_view = view;
			_bindings = new Dictionary<object, List<PropertyInfo>>();
			
		}
		
		public void Apply()
		{
			_viewModel.Apply();
		}

		
		private void BindComboBox(ComboBox cbo, object viewModel,PropertyInfo viewModelProperty)
		{
			if(cbo.InvokeRequired)
			{
				cbo.Invoke((MethodInvoker)delegate
					{
						BindComboBox(cbo, viewModel, viewModelProperty);
					});
				return;
			}
			
			IEnumerable list = viewModelProperty.GetValue(viewModel) as IEnumerable;
			SelectedListItemPropertyNameAttribute selectedPropAttrib = viewModelProperty.GetCustomAttribute
				<SelectedListItemPropertyNameAttribute>();
			
			if(selectedPropAttrib != null)
			{
				PropertyInfo destProperty = viewModel.GetType()
					.GetProperty(selectedPropAttrib.SelectedListItemPropertyName);
				if(destProperty != null)
				{
					cbo.SelectedValueChanged += (sender, e) => 
						SetValue(cbo,viewModel,cbo.GetType().GetProperty("Text"),destProperty);
					
					
					(viewModel as DashViewModel).PropertyChanged += (sender, e) => 
					{
						Action setValuesAction = () =>
						{
							var newValue = destProperty.GetValue(viewModel);
							if(newValue == null || string.IsNullOrEmpty(newValue.ToString()))
							{
								cbo.SelectedIndex = -1;
							}
							else
							{
								cbo.Text = newValue.ToString();
							}
						};
						if(cbo.InvokeRequired)
						{
							cbo.Invoke(setValuesAction);
						}
						else
						{
							setValuesAction();
						}
					};
				}
			}
			
			
			cbo.Items.Clear();
			
			if(list == null)
			{
				return;
			}
			
			foreach(var item in list)
			{
				cbo.Items.Add(item);
			}
			
		}
		
		private void SetList(ListView lv,object viewModel,PropertyInfo viewModelProperty)
		{
			if(lv.InvokeRequired)
			{
				lv.Invoke((MethodInvoker)delegate
					{
						SetList(lv, viewModel, viewModelProperty);
					});
				return;
			}
			
			IEnumerable list = viewModelProperty.GetValue(viewModel) as IEnumerable;
		
			lv.Items.Clear();
			if(list == null)
			{
				return;
			}
			foreach(var item in list)
			{
				ListViewItem lvi = lv.Items.Add(item.ToString());
				lvi.Tag = item;
			}
		}
		
		public BindingInformation<TViewObject,TViewModel> BindList<TViewObject,TViewProperty,TViewModelProperty>(TViewObject viewObj,Expression<Func<TViewModel,TViewModelProperty>> viewModelProperty)
		{
			return Bind(viewObj, default(Expression<Func<TViewObject,TViewProperty>>), viewModelProperty);
		}
		public BindingInformation<TViewObject,TViewModel> Bind<TViewObject,TViewProperty,TViewModelProperty>(TViewObject viewObj,Expression<Func<TViewObject,TViewProperty>> viewProperty,Expression<Func<TViewModel,TViewModelProperty>> viewModelProperty)
		{
			PropertyInfo vmProp = GetPropertyInfo(_viewModel, viewModelProperty);
			PropertyInfo viewProp = GetPropertyInfo(viewObj, viewProperty);
			
			if(!_bindings.ContainsKey(viewObj))
			{
				_bindings.Add(viewObj, new List<PropertyInfo>());
			}
			_bindings[viewObj].Add(vmProp);
			_viewModel.PropertyChanged += (object sender, System.ComponentModel.PropertyChangedEventArgs e) => 
			{
				if(e.PropertyName == vmProp.Name)
				{
					if(viewObj.GetType() == typeof(ListView))
					{
						SetList(viewObj as ListView, _viewModel, vmProp);						
					}
					else if(viewObj.GetType() == typeof(ComboBox))
					{
						BindComboBox(viewObj as ComboBox, _viewModel, vmProp);
					}
					else
					{
						SetValue(_viewModel, viewObj, vmProp, viewProp);
					}
				}
			};
			AutoBindView(viewObj);
			return new BindingInformation<TViewObject,TViewModel>(viewObj,this);
		}
		
		private void AutoBindView(object obj)
		{
			if(obj.GetType() == typeof(TextBox))
			{
				TextBox txt = obj as TextBox;
				txt.TextChanged += (sender, e) => ViewChanged(txt,vw => vw.Text);
			}
			if(obj.GetType() == typeof(DateTimePicker))
			{
				DateTimePicker dtp = obj as DateTimePicker;
				dtp.ValueChanged += (sender, e) => ViewChanged(dtp,vw => vw.Value);
			}
		}
		
		
		private TReturn GetEnumValue<TReturn>(string value)
		{
			int enumVal;
			if (!int.TryParse(value, out enumVal))
			{
				var enumVals = Enum.GetValues(typeof(TReturn)).OfType<object>().ToList();
				for (int i = 0; i < enumVals.Count; i++)
				{
					if (Enum.GetName(typeof(TReturn), enumVals[i]) == value)
					{
						return (TReturn)enumVals[i];
					}
				}
			}
			try
			{
				return (TReturn)Enum.GetValues(typeof(TReturn)).OfType<object>()
						.FirstOrDefault(v => ((int)v) == enumVal);
			}
			catch
			{
				return default(TReturn);
			}
		}
		
		public TReturn GetTyped<TReturn>(string value) where TReturn : IConvertible
		{
			if(string.IsNullOrEmpty(value))
			{
				return default(TReturn);
			}
			if(typeof(TReturn).IsEnum)
			{
				return GetEnumValue<TReturn>(value);
			}
			return (TReturn)Convert.ChangeType(value, typeof(TReturn));
		}
		
		private void SetValue(object sourceObj, object destinationObj, PropertyInfo source, PropertyInfo destination)
		{
			Control viewControl = (Control)_view;
			if(viewControl.InvokeRequired)
			{
				viewControl.Invoke
					(
						(MethodInvoker)
						delegate
						{
							SetValue(sourceObj,destinationObj,source,destination);
						}
				);
				return;
			}
			object intermediaryValue = source.GetValue(sourceObj);
			if(!Validator.Validate(sourceObj,intermediaryValue))
			{
				return;
			}
			string value = intermediaryValue == null ? "" : intermediaryValue.ToString();
			if(destination.PropertyType.GetInterfaces().Contains(typeof(IConvertible)))
			{
				Type thisType = this.GetType();
				MethodInfo theMethod = thisType.GetMethod("GetTyped");
				var genericMethod = theMethod.MakeGenericMethod(destination.PropertyType);
				destination.SetValue(destinationObj, genericMethod.Invoke(this, new[]{ value}));
			}
			else
			{
				destination.SetValue(destinationObj, value);
			}
		}
		
		public DashBinder<TViewModel> ViewChanged<TViewObject,TViewProperty>(TViewObject viewObj,Expression<Func<TViewObject,TViewProperty>> viewProperty)
		{
			
			if(!_bindings.ContainsKey(viewObj))
			{
				return this;
			}
			PropertyInfo viewProp = GetPropertyInfo(viewObj, viewProperty);
			var props = _bindings[viewObj];
			object value = viewProp.GetValue(viewObj);
			
			foreach (PropertyInfo propInfo in props)
			{
				SetValue(viewObj, _viewModel, viewProp, propInfo);
			}
			return this;
		}
		
		
		
		
		private PropertyInfo GetPropertyInfo<TSource, TProperty>(
			TSource source,
			Expression<Func<TSource, TProperty>> propertyLambda)
		{
			Type type = typeof(TSource);

			MemberExpression member = propertyLambda.Body as MemberExpression;
			if (member == null)
				throw new ArgumentException(string.Format(
						"Expression '{0}' refers to a method, not a property.",
						propertyLambda.ToString()));

			PropertyInfo propInfo = member.Member as PropertyInfo;
			if (propInfo == null)
				throw new ArgumentException(string.Format(
						"Expression '{0}' refers to a field, not a property.",
						propertyLambda.ToString()));

			if (type != propInfo.ReflectedType &&
			     !type.IsSubclassOf(propInfo.ReflectedType))
				throw new ArgumentException(string.Format(
						"Expresion '{0}' refers to a property that is not from type {1}.",
						propertyLambda.ToString(),
						type));

			return propInfo;
		}
	}
}
