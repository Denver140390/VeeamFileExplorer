* About program
Framework version: 4.5.2

* About code
In this File Explorer I tried to follow the MVVM principles of separating business logic (Models) from UI (Views) by connecting them with bridges (ViewModels). Pretty usual.

There are two main Models in the application: Files and Folders. Both inherit from FileModelBase class, which implements INotifyPropertyChanged interface.
The idea here is to separate Files and Folders as different entities, while still providing something common between them (Path and Name) and leaving the possibility to add something else.
Making easy things hard, yay!

ViewModels are to decorate Models to make them bindable by Views. Having a collection of FileModelBase class instances seems to be a nice way to pass the data about Files and Folders to the Views. 

Views are showing cool stuff and catching user inputs.
