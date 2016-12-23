# Specialized Binding, Part 1 of 4 : ComboBoxes and ListBoxes

For ComboBoxes and ListBoxes, there are two aims when binding:

1. Have a ViewModel's list of items filled into the component.

2. Have a single property in the ViewModel updated each time the user makes a selection in the ComboBox or ListBox.

This is easily achieved using the code below.


## 1. ViewModel code

```CSharp
    class EditTodoViewModel : DashViewModel
    {
        private const String CategoryWork = "Work";
        private const String CategoryPlay = "Play";
        private const String CategoryOther = "Other";

        public EditTodoViewModel()
        {
            Category = CategoryWork;
        }

        [SelectedListItemPropertyName("Category")]
        public List<string> Categories
        {
            get
            {
                return new List<string>
                {
                    CategoryOther, CategoryPlay, CategoryWork
                };
            }
        }

        string _selectedCategory;
        public string Category
        {
            get
            {
                return _selectedCategory;
            }
            set
            {
                SetField(ref _selectedCategory, value);
            }
        }
    }
```
### Points of Interest

a). The categories list which is the source list for the component in the view is decorated with the attribute *'SelectedListItemPropertyName'*. This attribute specifies the 
ViewModel property that is to be updated whenever the user makes a selection in the view.


## 2. View code

```CSharp
    public partial class EditTodoView : Form
    {

        public EditTodoView()
        {
            InitializeComponent();
            DoBindings();
        }

        

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

        private void DoBindings()
        {
            ViewHandle.Binder.Bind(cboCategory, cbo => cbo.Items, vm => vm.Categories)
                .Binder.Bind(cboCategory,cbo => cbo.Text, vm => vm.Category)
                .Binder.Apply();
        }
    }
```

### Points of Interest

a). The ComboBox is bound twice; once for the list of items (ViewModel property Categories) and once to the selected value in the ViewModel.

[<< Back : Simple Binding]() | [Next: Specialized Binding - ListViews] ()