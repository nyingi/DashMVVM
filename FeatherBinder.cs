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
using System.Linq.Expressions;
using System.Reflection;

namespace Jattac.Libs.WinFormsBinding
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
		
		
		public TViewObject Bind<TViewObject,TViewProperty,TViewModelProperty>(TViewObject viewObj,Expression<Func<TViewObject,TViewProperty>> viewProperty,Expression<Func<TViewModel,TViewModelProperty>> viewModelProperty)
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
					viewProp.SetValue(viewObj,vmProp.GetValue(_viewModel));
				}
			};
			return viewObj;
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
				propInfo.SetValue(_viewModel, value);
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
