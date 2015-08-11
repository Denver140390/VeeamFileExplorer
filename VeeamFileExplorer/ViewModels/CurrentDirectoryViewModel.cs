using System;
using System.Collections.ObjectModel;
using VeeamFileExplorer.Helpers;
using VeeamFileExplorer.Models;

namespace VeeamFileExplorer.ViewModels
{
    class CurrentDirectoryViewModel : PropertyChangedBase
    {
        private ObservableCollection<FileModelBase> _files;

        public ObservableCollection<FileModelBase> Files
        {
            get { return _files; }
            set { SetProperty(ref _files, value, () => Files); }
        }

        public CurrentDirectoryViewModel()
        {
            _files = new ObservableCollection<FileModelBase>();

            var folderModel = new FolderModel
            {
                Name="Folder",
                Path ="/Folder",
                ChangedDate = DateTime.Now,
                Size = 123
            };

            _files.Add(folderModel);

            var fileModel = new FileModel
            {
                Name = "File",
                Path = "/File",
                ChangedDate = DateTime.Now,
                Size = 321
            };

            _files.Add(fileModel);
        }
    }
}
