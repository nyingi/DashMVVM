# Simple Binding
Binding refers to the process by which a View's properties, methods and events are linked to corresponding properties, methods and events  in a ViewModel.
By performing binding between a View and a ViewModel, DashMVVM is able to match values back and forth between the two with no additional effort on the developer's
part required.

Binding is the key strength that DashMVVM brings to the table. It is therefore important to understand this section of the documentation to allow you to take full advantage of the framework.

## A brief history of shortsightedness
DashMVVM started out as a bit of an experiment as such I made two notable mistakes. These are:-

1. **Form as the View**
    
    I made the assumption that since this was to be a WinForms binding framework, it would only make sense to have the Form as the View. This I did by extending the Form and creating a component
    called the *DashForm*. When building your app using DashMVVM, it became necessary that your Forms inherit DashForm. While this provided convenient and quick access to functionality required to 
    bind, it quickly became apparent that forcing a base class on developers was at times really inconveniencing (say if they wished to use a different base class). Additionally IDEs such
    as SharpDevelop do not really like Forms inherit from anything other than System.Windows.Form.

    So why bring this up? Since several versions of the framework were released to Nuget with The DashForm component, it has been left in
    for the time being but has been marked obsolete and will be removed in a future release. Binding on the View side is now accomplished by an object of type *DashBinder*.

2. **Automatic Binding For Components**

    As with the above, I made the assumption that I could perform automatic two-way binding for all Windows components. The idea had been that regardless of whatever UI component you 
    dropped on your form, once you setup binding, anything the user entered in the component would be pushed to the ViewModel and anything that was changed in the ViewModel would 
    automatically reflect in the UI. WinForms is one of the richest most mature user interface building environment out there and I believe it is no exaggeration to say, there are thousands upon thousands 
    of user controls out there. With binding being built on events fired when changes occur (e.g TextChanged, SelectedChanged, CheckStateChanged e.t.c) it soon became apparent that the only automatic binding
    I could implement was for a very limited set of components. As such the only components for which this automatic binding works are:-

        * TextBox
        * DateTimePicker
        * ComboBox (some additional work required)
        * ListBox (some additional work required)

    Way out of this? There is a simple interface called the *IBindingStrategy* that you can implement for each type of component that requires specialized binding. This includes third party controls
    whose code you have no access to.

    We cover the IBindingStrategy interface intensively in the next section.

## Binding
To get started, you need a ViewModel and a View. In most cases, a View is simply a Form with nothing special done to it. ViewModels on the other hand must have somewhere down in their 
inheritance hierarchy must have the type *DashViewModel*. The *DashViewModel* class implements *INotifyPropertyChanged* interface which allows DashMVVM to broadcast changes to properties.

### 1. The ViewModel

```CSharp

    class EditTodoViewModel : DashViewModel
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

        DateTime _date;
        public DateTime Date
        {
            get
            {
                return _date;
            }
            set
            {
                SetField(ref _date, value);
            }
        }
    }

```

#### Points of interest
a). The ViewModel inherits the *DashViewModel* 

b). All Properties to be used in binding must have a backing field and have their values set using the construct ```SetField(ref <backing-field>, value);``` This allows ViewModels to
report when a property's value is changed.


### 2. The View

```CSharp
    public partial class EditTodoView : Form
    {
        DashViewHandle<EditTodoViewModel> _viewHandle;
        public DashViewHandle<EditTodoViewModel> ViewHandle
        {
            get
            {
                if (_viewHandle == null)
                {
                    _viewHandle = new DashViewHandle<EditTodoViewModel>(this);
                }
                return _viewHandle;
            }

        }
    }
```

#### Points of interest
a). The View is nothing more than an ordinary Form.

b). We declare one special object of type *DashViewHandle*. It is generic and it takes in the type of the corresponding ViewModel. It should be noted that besides specifying the ViewModel the 
View binds to, there is an attempt to prevent coupling the two tighter so that the same ViewModel can be used by another View. Additionally, though not tested and probably not advisable, are is no foreseeable reason why
your View shouldn't declare multiple DashViewHandle objects each linking to a distinct ViewModel.


### Actual binding
Our ViewModel above has two properties, a string and a DateTime property. The corresponding UI elements for these are a TextBox and a DateTimePicker on the Form. We'll assume these have been
creatively named *txtDescription* and *dtpDate* respectively. We could create a method in the View to do the bindings as below. 

```CSharp

    private void DoBindings()
    {
        ViewHandle.Binder.Bind(txtDescription, txt => txt.Text, vm => vm.Description)
            .Binder.Bind(dtpDate, dtp => dtp.Value, vm => vm.Date)
            .Binder.Apply();
    }

```

#### Points of interest
a). The *ViewHandle.Binder.Bind* method returns an instance of the *ViewHandle* this allows you to chain multiple binding calls.

b). The *DoBindings()* method above is an arbitrary method. You can create your own methods with custom names in which you can setup your bindings. Just remember to call the methods in code.

c). As noted above, both the DateTimePicker and TextBox support automatic two-way binding so for now thats enough to pass data back and forth between the View and ViewModel.