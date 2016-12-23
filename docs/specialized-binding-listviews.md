# Specialized Binding, Part 2 of 3 : ListViews

The aims for binding to ListViews are:

1. Linking an arbitrary System.Collections.List to a ListView.

2. Providing arbitrary aliases for column names.

3. Specifying a logical method for sizing and scaling columns.

4. Handling refreshing of ListView contents when the source list gets updated.

5. Handling smooth redrawing of the ListView on refreshing to minimize flickering.

6. Handle lists with numerous items to ensure that items are listed fast and efficiently.


While DashMVVM will happily work with the regular old Windows Forms ListView, to achieve points 5 & 6 above, I built a new component that inherits the ListView and implements double-buffering.
The component can be found at *DashMvvm.Forms.DashListView*. Once you add it to your IDE's toolbox, you can drop it onto Forms in place of regular ListViews with no adjustments to your code required.

For the rest of the points above, binding is achieved as follows:

## 1. Model Code
```CSharp
    class Todo
    {
        public Todo()
        {
        }

        [ListViewColumnAttribute("Description", 2)]
        public string Description { get; set; }

        [ListViewColumn("Date", 1)]
        public DateTime TodoDate { get; set; }

        [ListViewColumn("Category", 1)]
        public string Category { get; set; }
    }
```
### Points of Interest
a). Properties in your Model that will be part of the ListView's columns are decorated with the ListViewColumn attribute. The attribute takes two parameters:-

    * Alias of the property/column.
    * Width weight of the column. The larger the value of the weight, the wider the column.

b). The order in which your properties appear, top to bottom in your model is the same order from left to right that the columns will be added to the ListView.

## 2. ViewModel Code

```CSharp
    class TodoListingViewModel : DashViewModel
	{
		public TodoListingViewModel()
		{
			RefreshTodos();
		}
		
		List<Todo> _listOfTodos;
		public List<Todo> ListOfTodos
		{
			get
			{
				return _listOfTodos;
			}
			set
			{
				SetField(ref _listOfTodos, value);
			}
		}
		
		public void RefreshTodos()
		{
			ListOfTodos = new TodoService().GetAll<Todo>();
		}
		
		
		
		Todo _selectedItem;
		public Todo SelectedItem
		{
			get
			{
				return _selectedItem;
			}
			set
			{
				SetField(ref _selectedItem, value);
			}
		}
		
	}
```

### Points of Interest

a). The List that will be used as source of items for our ListView is a list of the *Todo* Model.


## 2. View Code

```CSharp
    partial class TodoListingView : Form
    {
        private DashViewHandle<TodoListingViewModel> _viewHandle;

        internal DashViewHandle<TodoListingViewModel> ViewHandle
        {
            get
            {
                if (_viewHandle == null)
                {
                    _viewHandle = new DashViewHandle<TodoListingViewModel>(this);
                }
                return _viewHandle;
            }
        }

        public TodoListingView()
        {
            DoBindings();
        }

        private void DoBindings()
        {
            ViewHandle.Binder.Bind(lvTodos, obj => obj.Columns, vm => vm.ListOfTodos)
                .Binder.Bind(lvTodos, obj => obj.Items, vm => vm.ListOfTodos)
                .ViewItem.ItemSelectionChanged += (sender, e) =>
                {
                    ViewHandle.ViewModel.SelectedItem = e.Item.Tag as Todo;
                };
            ViewHandle.Binder.Bind(btnDelete, btn => btn.Enabled, vm => vm.DeleteEnabled)
                .Binder.Apply();
        }
    }
```
### Points of Interest

a). For ListViews, we bind to the same ViewModel list twice; once for columns and once for the list of items.

b). The ViewHandle's Bind method returns an object that also contains the ViewItem or component that has just been bound. In this instance, we take advantage of this by subscribing to the
ItemSelectionChanged event of the ListView.

c). The Tag property of each ListView Item is the actual object that was used to create the row including all the additional properties that are not visible in the row.

