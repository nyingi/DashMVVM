/*
 * Created by SharpDevelop.
 * User: Nyingi
 * Date: 12-Jun-16
 * Time: 10:52 AM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Windows.Forms;

namespace FeatherMvvm.Validation
{
	/// <summary>
	/// Description of ValidationErrorEventArgs.
	/// </summary>
	public class ValidationResultEventArgs
	{

		public object ValidatableObject
		{
			get;
			set;
		}

		public string Error
		{
			get;
			set;
		}

		public ValidationResultEventArgs(object validatableObject,string error)
		{
			Error = error;
			ValidatableObject = validatableObject;
		}
	}
}
