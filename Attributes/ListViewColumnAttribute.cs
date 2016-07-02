/*
 * Created by SharpDevelop.
 * User: Nyingi
 * Date: 26-Jun-16
 * Time: 8:50 AM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Windows.Forms;

namespace FeatherMvvm.Attributes
{
	/// <summary>
	/// Description of ListViewColumnAttribute.
	/// </summary>
	public class ListViewColumnAttribute : Attribute
	{
		public string Title { get; set; }
		public float WidthWeight { get; set; }
		
		public ListViewColumnAttribute(string title,int widthWeight )
		{
			Title = title;
			WidthWeight = widthWeight;
		}
	}
}
