/*
 * Created by SharpDevelop.
 * User: Nyingi
 * Date: 12-Jun-16
 * Time: 10:59 AM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections.Generic;
using System.Linq;

namespace DashMvvm.Validation
{
	/// <summary>
	/// Description of Validator.
	/// </summary>
	internal class Validator
	{
		internal event EventHandler<ValidationResultEventArgs> ControlValidated;
		internal Dictionary<object,bool> _validityStates = new Dictionary<object, bool>();
		internal event EventHandler ViewIsValid;
		
		public Validator()
		{
			Validations = new Dictionary<object, List<Func<object, string>>>();
		}
		
		private Dictionary<object, List<Func<object, string>>> Validations
		{
			get;
			set;
		}
		
		public bool Validate(object obj,object value)
		{
			_validityStates[obj] = true;
			if(!Validations.ContainsKey(obj))
			{
				return true;
			}
			foreach (var rule in Validations[obj])
			{
				string result = rule(value);
				_validityStates[obj] = string.IsNullOrEmpty(result);
				if (ControlValidated != null)
				{
					ControlValidated(this, new ValidationResultEventArgs(obj, result));
					if (!_validityStates[obj])
					{
						return false;
					}
				}
			}
			
			if(ViewIsValid != null && DataIsValid)
			{
				ViewIsValid(this, EventArgs.Empty);
			}
			
			return _validityStates[obj];
		}
		
		public void AddValidation(object validatableObject,Func<object,string> rule)
		{
			if(Validations.ContainsKey(validatableObject))
			{
				Validations[validatableObject].Add(rule);
			}
			else
			{
				Validations.Add(validatableObject, new List<Func<object, string>> { rule });
				_validityStates.Add(validatableObject, true);
			}
		}
		
		public bool DataIsValid
		{
			get
			{
				return _validityStates.All(a => a.Value);
			}
		}
	}
}
