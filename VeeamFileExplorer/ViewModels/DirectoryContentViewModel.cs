using System;
using System.Collections.ObjectModel;
using System.IO;
using VeeamFileExplorer.Helpers;
using VeeamFileExplorer.Models;

namespace VeeamFileExplorer.ViewModels
{
    class DirectoryContentViewModel : PropertyChangedBase
    {
        private ObservableCollection<FileModelBase> _content;

        public ObservableCollection<FileModelBase> Content
        {
            get { return _content; }
            set { SetProperty(ref _content, value, () => Content); }
        }

        public DirectoryContentViewModel()
        {
            _content = new ObservableCollection<FileModelBase>();

            var folderModel = new FolderModel
            {
                Name="Folder",
                Path ="/Folder",
                ChangedDate = DateTime.Now,
                Size = 123
            };

            _content.Add(folderModel);

            var fileModel = new FileModel
            {
                Name = "File",
                Path = "/File",
                ChangedDate = DateTime.Now,
                Size = 321
            };

            _content.Add(fileModel);
        }

        public void LoadDirectoryContent(string path)
        {
            _content.Clear();

            foreach (var file in Directory.GetFiles(path))
            {
                var folderModel = new FolderModel
                {
                    Name = file,
                    Path = path,
                    ChangedDate = File.GetLastWriteTime(path)
                    //ToDo async folder size calculation
                };

                _content.Add(folderModel);
            }

            foreach (var file in Directory.GetFiles(path))
            {
                var fileModel = new FileModel
                {
                    Name = file,
                    Path = path,
                    ChangedDate = File.GetLastWriteTime(path),
                    Size = (new FileInfo(path)).Length //TODO (new FileInfo(path)).Length??? For real???
                };

                _content.Add(fileModel);
            }
        }
    }
}
