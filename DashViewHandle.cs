/*
 * Created by SharpDevelop.
 * User: Nyingi
 * Date: 23-Jun-16
 * Time: 8:59 PM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Threading.Tasks;
using System.Windows.Forms;
using DashMvvm;
using DashMvvm.Binding;
using DashMvvm.Messaging;
using DashMvvm.Validation;

namespace FeatherMvvm
{
	/// <summary>
	/// Description of DashViewHandle.
	/// </summary>
	public class DashViewHandle<TViewModel> : IDisposable where TViewModel : DashViewModel , new()
	{
		

		private Form _form;
		public Form Form
		{
			get
			{
				return _form;
			}
		}
		
		
		public DashViewHandle(IMessageBus messageBus,Form form)
		{
			MessageBus = messageBus;	
			_form = form;
			_form.Shown += (sender, e) => 
			Task.Run(async () => await ViewModel.OnStartAsync());
			
		}
		
		public DashViewHandle(Form form) : this(Messager.Instance,form)
		{
			
		}
		
		public event EventHandler<ValidationResultEventArgs> ControlValidated;
		public event EventHandler ViewIsValid;
		
		public virtual IMessageBus MessageBus
		{
			get;
			set;
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
		
		
		
		
		
		public void Dispose()
		{
			Binder = null;
			ViewModel = null;
		}
		
		public void AddValidation(Control control,Func<object,string> rule)
		{
			Binder.Validator.AddValidation(control, rule);
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
