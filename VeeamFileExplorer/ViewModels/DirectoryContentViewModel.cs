using System;
using System.Collections.ObjectModel;
using System.IO;
using GalaSoft.MvvmLight.Messaging;
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

            Messenger.Default.Register<string>(this, LoadDirectoryContent);
        }

        public void LoadDirectoryContent(string path)
        {
            _content.Clear();

            foreach (var folder in Directory.GetDirectories(path))
            {
                var folderModel = new FolderModel
                {
                    Name = folder.Substring(folder.LastIndexOf("\\", StringComparison.Ordinal) + 1),
                    Path = path,
                    ChangedDate = File.GetLastWriteTime(path)
                    //Size = (new FileInfo(file)).Length //ToDo async folder size calculation
                };

                _content.Add(folderModel);
            }
            
            foreach (var file in Directory.GetFiles(path))
            {
                var fileModel = new FileModel
                {
                    Name = file.Substring(file.LastIndexOf("\\", StringComparison.Ordinal) + 1),
                    Path = path,
                    ChangedDate = File.GetLastWriteTime(path),
                    Size = (new FileInfo(file)).Length
                };

                _content.Add(fileModel);
            }
        }
    }
}
