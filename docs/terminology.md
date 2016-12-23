# DashMVVM Terminology

Throughout this docs, we shall constantly reference various keywords; we shall try to define as many of those as possible here.

## View
A View is a screen element that a user can interact with. Typically Views are used to get information from the user and communicate output of the program back to them.

## ViewModel
This conceptually is a layer that underpins the View. It holds information currently displayed in the View, it is able to do this by binding to a View.

## Binding
Binding refers to the process by which a View's properties, methods and events are linked to corresponding properties, methods and events  in a ViewModel.
By performing binding between a View and a ViewModel, DashMVVM is able to match values back and forth between the two with no additional effort on the developer's
part required.

## Model
A class that ideally consists of properties only and that maps directly to the persistance layer. Usually the Model will map to a table in your database and its properties to the table's columns.

## Service
The type of object that performs the business logic required by your program.

## ViewHandle.
An object that associates a given View with its corresponding ViewModel.

## MessageBus.
An object that handles broadcast of messages from one part of the program to another.