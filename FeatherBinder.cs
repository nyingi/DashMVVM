/*
 * Created by SharpDevelop.
 * User: Nyingi
 * Date: 5/27/2016
 * Time: 11:25 PM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Windows.Forms;

namespace FeatherMvvm
{
	/// <summary>
	/// Description of BindingContext.
	/// </summary>
	public class FeatherBinder<TViewModel> where TViewModel : FeatherViewModel , new()
	{
		TViewModel _viewModel;
		FeatherView<TViewModel> _view;
		Dictionary<object,List<PropertyInfo>> _bindings;
		public FeatherBinder(TViewModel viewModel,FeatherView<TViewModel> view)
		{
			_viewModel = viewModel;
			_view = view;
			_bindings = new Dictionary<object, List<PropertyInfo>>();
			
		}
		
		public void Apply()
		{
			_viewModel.Apply();
		}
		
		public BindingInformation<TViewObject> Bind<TViewObject,TViewProperty,TViewModelProperty>(TViewObject viewObj,Expression<Func<TViewObject,TViewProperty>> viewProperty,Expression<Func<TViewModel,TViewModelProperty>> viewModelProperty)
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
					SetValue(_viewModel, viewObj, vmProp, viewProp);
				}
			};
			AutoBindView(viewObj);
			return new BindingInformation<TViewObject>(viewObj);
		}
		
		private void AutoBindView(object obj)
		{
			if(obj.GetType() == typeof(TextBox))
			{
				TextBox txt = obj as TextBox;
				txt.TextChanged += (sender, e) => ViewChanged(txt,vw => vw.Text);
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
		
		public FeatherBinder<TViewModel> ViewChanged<TViewObject,TViewProperty>(TViewObject viewObj,Expression<Func<TViewObject,TViewProperty>> viewProperty)
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
