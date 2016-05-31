/*
 * Created by SharpDevelop.
 * User: Nyingi
 * Date: 5/29/2016
 * Time: 6:04 PM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;

namespace FeatherMvvm
{
	/// <summary>
	/// Description of BindingInformation.
	/// </summary>
	public class BindingInformation<TItem>
	{
		public BindingInformation(TItem item)
		{
			ViewItem = item;
		}
		
		public TItem ViewItem { get; private set; }
	}
}
