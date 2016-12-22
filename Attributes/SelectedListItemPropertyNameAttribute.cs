/*
 * Created by SharpDevelop.
 * User: Nyingi
 * Date: 18-Jun-16
 * Time: 7:04 AM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;

namespace DashMvvm.Attributes
{
	/// <summary>
	/// Description of SelectedListItemPropertyNameAttribute.
	/// </summary>
	public class SelectedListItemPropertyNameAttribute : Attribute
	{
		public string SelectedListItemPropertyName { get; set; }
		public SelectedListItemPropertyNameAttribute(string selectedListItemPropertyName)
		{
			SelectedListItemPropertyName = selectedListItemPropertyName;
		}
	}
}
