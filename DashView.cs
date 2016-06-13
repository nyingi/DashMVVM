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
		}
		
		
		private DashBinder<TViewModel> _binder;

		public DashBinder<TViewModel> Binder
		{
			get
			{
				if(_binder == null)
				{
					_binder = new DashBinder<TViewModel>(ViewModel,this);
					_binder.Validator = new Validator();
					_binder.Validator.ControlValidated += (object sender, ValidationResultEventArgs e) => 
					{
						if(ControlValidated != null)
						{
							ControlValidated(sender,e);
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
		
		public DashView<TViewModel> AddValidation(Control control,Func<object,string> rule)
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
