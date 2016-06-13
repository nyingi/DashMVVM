# DashMVVM
Inspired by MvvmCross for Xamarin.Android and Xamarin.iOS, this is a basic Mvvm library for WinForms that seeks to channel the ease, structure and 
clarity that simple, robust and predictable databinding brings to apps.

## Components
### 1. Views
DashMVVM has a base class for views. This base class inherits System.Windows.Forms.Form thus when you base your views on this, not only do 
you gain extended binding functionality, you also maintain the drag-n-drop form building capabilities in Visual Studio.

### 2. ViewModels
These are the components of your program that store state information and act as an intermediary between your views, your business logic and your database models.

### 3. MessageBus
This component allows broadcasting of messages by either a View or a ViewModel to any other component that may be subscribed to the message.

### 4. Validator
The validator component checks the data input in the Views before writing it to the ViewModel. The validator verifies the data against user-defined rules and cancels transfer of data if it is invalid.

### 5. Binder
The binder links components in the View to properties in the ViewModel to enforce two-way binding.

## Usage
### 1. Grab Reference From Nuget.
Using nuget, search for DashMVVM and add to your project.

### 2. Create ViewModels. 
These are nothing more than C# classes that extend DashViewModel.

```C#
public class EditTodoViewModel : DashViewModel
{
	string _description;
	public string Description
	{
		get
		{
			return _description;
		}
		set
		{
			SetField(ref _description, value);
		}
}
}
```
### 3. Create Your Views
Add a new form in Visual Studio or your favourite IDE. Edit the form's code so it no longer extends 'Form' but 'DashView'.

```C#
public partial class EditTodoView : DashView<EditTodoViewModel>
{

}
```

Please note, DashView is generic and expects the type of the ViewModel to be supplied.

### 4. Configuring Validation
To configure validation do the following
#### 1. Create Validation Rules
Validation rules are simple functions that take in an object and return an validation error string if error occurs. If data is valid the the function should return a blank string. Example

```C#
public static string NotEmpty(object obj)
{
	if(obj == null || string.IsNullOrEmpty(obj.ToString()))
	{
		return "A value must be provided for this field";
	}
	return string.Empty;
}
```

####2. Register Control For Validation
Your View inherits an 'AddValidation' method. Use this method to register a control for validation. Each control can have multiple rules registered.

```C#
AddValidation(txtDescription, NotEmpty);
```
####3. Subscribe To Validate Error Occured Event
Each time a view component gets changed, its value is validated. If the value is invalid then a 'ValidationErrorOccured' event is fired. Subscribe to it to react to invalid states. Similarly each time a component is changed if the entire view is valid, then a 'ViewIsValid' event is fired as shown below. By also subscribing to this, you can manage the state of your view.

```C#
void SubscribeEvents()
{
	ValidationErrorOccured += (object sender, DashMVVM.Validation.ValidationErrorEventArgs e) => 
	{
		cmdSave.Enabled = false;
		MessageBox.Show(e.Error);
	};
	
	ViewIsValid += (sender, e) => cmdSave.Enabled = true;
}
```
###4. Messaging
The messaging bus allows components to broadcast messages and for subscribers to act on those broadcasts. To use messaging, do the following.

####1. Setup Subscription
Your Views and ViewModels all have a MessageBus object. To listen for broadcasts in your component simply subscribe to an event and in the event handler
filter and react to the messages you wish to action.

```C#
MessageBus.MessagePassed += MessageBus_MessagePassed;
```

```C#
void MessageBus_MessagePassed(object sender, DashMVVM.Messaging.MessageEventArgs e)
{
    switch(e.MessageTag)
    {
        case Messages.RefreshTodosList:
            ViewModel.RefreshTodos();
            break;
    }
}
```

Please note message tags are simply user defined strings. They can be anything you like.


####2. Send Message
Using the MessageBus object, simply send out the desired message as show below.

```C#
MessageBus.SendMessage(Messages.RefreshTodosList);
```

###5. Binding
Binding allows you to link controls in your View to properties in your ViewModel. Two way binding is automatically supported for Textboxes and DateTimePickers.
One-way binding is supported for ListViews. Binding for additional controls is planned.

```C#
private void DoBindings()
{
	Binder.Bind(lvTodos, obj => obj.Items, vm => vm.ListOfTodos)
		.ViewItem.ItemSelectionChanged += (sender, e) => 
	{
		ViewModel.SelectedItem = e.Item.Tag as Todo;
	};
	Binder.Bind(btnDelete, btn => btn.Enabled, vm => vm.DeleteEnabled);
	Binder.Apply();
}
```

###6. Example App
Go to the [FeatherTodo](https://github.com/nyingi/FeatherTodo/) app which implements all of the above features for examples on how to utilize DashMVVM 
