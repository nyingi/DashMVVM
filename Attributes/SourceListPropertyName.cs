/*
 * Created by SharpDevelop.
 * User: Nyingi
 * Date: 21-Jun-16
 * Time: 3:09 PM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;

namespace FeatherMvvm.Attributes
{
	/// <summary>
	/// Description of SourceListPropertyName.
	/// </summary>
	public class SourceListPropertyNameAttribute : Attribute
	{
		public string SourceListPropertyName { get; set; }
		public SourceListPropertyNameAttribute(string sourceListPropertyName)
		{
			SourceListPropertyName = sourceListPropertyName;
		}
	}
}
