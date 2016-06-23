/*
 * Created by SharpDevelop.
 * User: Nyingi
 * Date: 5/27/2016
 * Time: 10:25 PM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Threading.Tasks;
using System.Windows.Forms;
using DashMvvm.Binding;
using DashMvvm.Messaging;
using DashMvvm.Validation;
using FeatherMvvm;

namespace DashMvvm
{
	/// <summary>
	/// Description of IView.
	/// </summary>
	public  class DashView<TViewModel> : Form where TViewModel : DashViewModel , new()
	{
		public event EventHandler<ValidationResultEventArgs> ControlValidated;
		public event EventHandler ViewIsValid;
		
		public virtual IMessageBus MessageBus
		{
			get;
			set;
		}
		
		private DashViewHandle<TViewModel> _viewHandle;
		
		
		protected override void OnShown(EventArgs e)
		{
			base.OnShown(e);
			Task.Run(async() => await ViewModel.OnStartAsync());
		}
		
		
		public DashView() : this(Messager.Instance)
		{
			
		}
		
		public DashView(IMessageBus messageBus)
		{
			MessageBus = messageBus;
			_viewHandle = new DashViewHandle<TViewModel>(messageBus, this);
			_viewHandle.ControlValidated += (sender, e) => 
			{
				if(ControlValidated != null)
				{
					ControlValidated(sender,e);
				}
			};
			_viewHandle.ViewIsValid += (sender, e) => 
			{
				if(ViewIsValid != null)
				{
					ViewIsValid(sender,e);
				}
			};
		}
		
		
		private DashBinder<TViewModel> _binder;

		public DashBinder<TViewModel> Binder
		{
			get
			{
				return _viewHandle.Binder;
			}
		}
		private TViewModel _viewModel;

		public TViewModel ViewModel
		{
			get
			{
				return _viewHandle.ViewModel;
			}
		}
		
		
		
		void Apply()
		{
			
		}
		
		protected override void Dispose(bool disposing)
		{
			_viewHandle.Dispose();
			base.Dispose(disposing);
		}
		
		public DashView<TViewModel> AddValidation(Control control,Func<object,string> rule)
		{
			_viewHandle.Binder.Validator.AddValidation(control, rule);
			return this;
		}
		
		public bool ViewDataIsValid
		{
			get
			{
				return _viewHandle.Binder.Validator.DataIsValid;
			}
		}
		
	}
}
