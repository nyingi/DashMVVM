/*
 * Created by SharpDevelop.
 * User: Nyingi
 * Date: 5/26/2016
 * Time: 6:17 AM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;
using FeatherMvvm.Messaging;

namespace FeatherMvvm
{
	/// <summary>
	/// Description of MyClass.
	/// </summary>
	public class FeatherViewModel : INotifyPropertyChanged
	{
		
		internal IMessageBus MessageBus { get; set; }
		
		public event PropertyChangedEventHandler PropertyChanged;

		protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
		{
			if(PropertyChanged != null)
			{
				PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
			}
		}
		
		protected void OnPropertyChanged<TObj,TProp>(TObj Obj,Expression<Func<TObj,TProp>> viewProperty)
		{
			var propInfo = GetPropertyInfo(Obj,viewProperty);
			OnPropertyChanged(propInfo.Name);
		}
		
		protected bool SetField<T>(ref T field, T value, [CallerMemberName] string propertyName = null)
		{
			if (EqualityComparer<T>.Default.Equals(field, value))
			{
				return false;
			}
			field = value;
			OnPropertyChanged(propertyName);
			return true;
		}
		
		
		public virtual void Apply()
		{
			foreach(PropertyInfo prop in GetType().GetProperties())
			{
				OnPropertyChanged(prop.Name);
			}
		}
		
		protected PropertyInfo GetPropertyInfo<TSource, TProperty>(
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