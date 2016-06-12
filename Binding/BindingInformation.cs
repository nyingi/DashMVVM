/*
 * Created by SharpDevelop.
 * User: Nyingi
 * Date: 5/29/2016
 * Time: 6:04 PM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;

namespace FeatherMvvm.Binding
{
	/// <summary>
	/// Description of BindingInformation.
	/// </summary>
	public class BindingInformation<TItem,TViewModel>  where TViewModel : FeatherViewModel , new()
	{
		public BindingInformation(TItem item,FeatherBinder<TViewModel> binder)
		{
			ViewItem = item;
			Binder = binder;
		}
		
		public TItem ViewItem { get; private set; }
		
		public FeatherBinder<TViewModel> Binder { get; set; }
	}
}
