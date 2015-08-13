using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Windows;
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

            if (DesignerProperties.GetIsInDesignMode(new DependencyObject())) LoadDummyInfo(); // lifehack for DataGrid designing

            Messenger.Default.Register<string>(this, LoadDirectoryContent);
        }

        private void LoadDummyInfo()
        {
            var folderModel = new FolderModel
            {
                Name = "Folder",
                Path = "D:\\",
                ChangedDate = DateTime.Now
            };

            _content.Add(folderModel);

            var fileModel = new FileModel
            {
                Name = "File",
                Path = "D:\\",
                ChangedDate = DateTime.Now,
                Size = 123
            };

            _content.Add(fileModel);
        }

        public void LoadDirectoryContent(string path)
        {
            _content.Clear();

            string[] directories;
            string[] files;
            try
            {
                directories = Directory.GetDirectories(path);
                files = Directory.GetFiles(path);
            }
            catch (Exception e)
            {
                directories = new string[0];
                files = new string[0];
                //throw new Exception(e.Message);
            }

            foreach (var folder in directories)
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
            
            foreach (var file in files)
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
