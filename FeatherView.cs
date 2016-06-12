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
using System.Threading.Tasks;
using System.Windows.Forms;
using FeatherMvvm.Binding;
using FeatherMvvm.Messaging;
using FeatherMvvm.Validation;

namespace FeatherMvvm
{
	/// <summary>
	/// Description of IView.
	/// </summary>
	public  class FeatherView<TViewModel> : Form where TViewModel : FeatherViewModel , new()
	{
		public event EventHandler<ValidationErrorEventArgs> ValidationErrorOccured;
		public event EventHandler ViewIsValid;
		
		public virtual IMessageBus MessageBus
		{
			get;
			set;
		}
		

		
		protected override void OnShown(EventArgs e)
		{
			base.OnShown(e);
			Task.Run(async() => await ViewModel.OnStartAsync());
		}
		
		
		public FeatherView() : this(Messager.Instance)
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
					_binder.Validator = new Validator();
					_binder.Validator.ValidationErrorOccured += (object sender, ValidationErrorEventArgs e) => 
					{
						if(ValidationErrorOccured != null)
						{
							ValidationErrorOccured(sender,e);
						}
					};
					_binder.Validator.ViewIsValid += (object sender, EventArgs e) => 
					{
						if(ViewIsValid != null)
						{
							ViewIsValid(sender,e);
						}
					};
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
		
		public FeatherView<TViewModel> AddValidation(Control control,Func<object,string> rule)
		{
			Binder.Validator.AddValidation(control, rule);
			return this;
		}
		
		public bool ViewDataIsValid
		{
			get
			{
				return Binder.Validator.DataIsValid;
			}
		}
		
	}
}
