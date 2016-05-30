# FeatherMvvm
Inspired by MvvmCross for Xamarin.Android and Xamarin.iOS, this is a basic Mvvm library for WinForms that seeks to channel the ease, structure and 
clarity that simple, robust and predictable databinding brings to apps.

## Components
### 1. Views
FeatherMvvm has a base class for views. This base class inherits System.Windows.Forms.Form thus when you base your views on this, not only do 
you gain extended binding functionality, you also maintain the drag-n-drop form building capabilities in Visual Studio.

### 2. ViewModels
These are the components of your program that store state information and act as an intermediary between your views, your business logic and your database models.
