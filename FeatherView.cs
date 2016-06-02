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
using FeatherMvvm.Messaging;

namespace FeatherMvvm
{
	/// <summary>
	/// Description of IView.
	/// </summary>
	public  class FeatherView<TViewModel> : Form where TViewModel : FeatherViewModel , new()
	{
		

		public virtual IMessageBus MessageBus
		{
			get;
			set;
		}

		
		public FeatherView()
		{
			
		}
		
		public FeatherView(IMessageBus messageBus)
		{
			MessageBus = messageBus;
		}
		
		
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
			private set { _binder = value; }
		}
		private TViewModel _viewModel;

		public TViewModel ViewModel
		{
			get
			{
				if(_viewModel == null)
				{
					_viewModel = new TViewModel();
					_viewModel.MessageBus = MessageBus;
				}
				return _viewModel;
			}
			private set { _viewModel = value; }
		}
		
		
		
		void Apply()
		{
			
		}
		
		protected override void Dispose(bool disposing)
		{
			Binder = null;
			ViewModel = null;
			base.Dispose(disposing);
		}
		
		
	}
}
