* About program
Framework version: 4.5.2

The program is designed to browse local file system.
UI consists of 3 views: address bar, folders tree view and currently-opened-directory-content view (DataGrid).
Selecting a folder in folders' tree updates address bar and asyncronously loads info into DataGrid.
Right mouse button click on folders' tree opens context menu with "Open in Windows Explorer" item.
Correct input + Enter in Address Bar = folders' tree + DataGrid are filled with corresponding data.

* About code
In this File Explorer I tried to follow the MVVM principles of separating business logic (Models) from UI (Views) by connecting them with bridges (ViewModels). Pretty usual.

There are two main Models in the application: Files and Folders. Both inherit from FileModelBase class, which implements INotifyPropertyChanged interface.
The idea here is to separate Files and Folders as different entities, while still providing something common between them (Path and Name) and leaving the possibility to add something else.
Making easy things hard, yay!

ViewModels are to decorate Models to make them bindable by Views. Having a collection of FileModelBase class instances seems to be a nice way to pass the data about Files and Folders to the Views.
Using MVVMLight Toolkit Messenger class to pass data between ViewModels.

Views are showing cool stuff and catching user inputs. Using code-behind tho, could not quite manage, how to implement the logic in XAML-only.
Implemented View-First approach. ViewModel objects are being created in Views.

Folders and Files are loading asyncronously on TreeView item selection or expanding.

* Possible improvements
Add comments in the code.
Improve variables and classes namings. Folder, Directory... Quite messy.
Interactive DirectoryContentView - open folders and files, copy, paste and other usual Windows Explorer features.
Multiple ContentViews - for comparison.
Navigation Buttons - backward and forward actions.
Asynchronous Loading - load as much info as possible asynchronously (load subfolders aswell to improve UX). Without affecting too much RAM tho.
Serialization.
Cool green-styled UI.
Use FileInfo class instead of custom Models? Probably would be too bulky for few properties, and INPC implementation is required anyway.
A LOT MORE...
(And there are few more TODOs in code, which you can explore using R# To-do Explorer)