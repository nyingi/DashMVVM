/*
 * Created by SharpDevelop.
 * User: Nyingi
 * Date: 5/27/2016
 * Time: 10:25 PM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Reflection;
using System.Windows.Forms;

namespace Jattac.Libs.WinFormsBinding
{
	/// <summary>
	/// Description of IView.
	/// </summary>
	public  class FeatherView<TViewModel> : Form where TViewModel : FeatherViewModel , new()
	{
		
		
		
		private FeatherBinder<TViewModel> _binder;

		public FeatherBinder<TViewModel> Binder
		{
			get
			{
				if(_binder == null)
				{
					_binder = new FeatherBinder<TViewModel>(ViewModel,this);
				}
				return _binder;
			}
		}
		private TViewModel _viewModel;

		public TViewModel ViewModel
		{
			get
			{
				if(_viewModel == null)
				{
					_viewModel = new TViewModel();
				}
				return _viewModel;
			}
		}
		
		
		
		void Apply()
		{
			
		}
		
		
	}
}
