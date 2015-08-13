using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Drawing;
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

            // Lifehack for DataGrid designing
            if (DesignerProperties.GetIsInDesignMode(new DependencyObject())) LoadDummyInfo();

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
            catch (Exception)
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
                //Fixing an odd behaviour. I've got a file: D:\Autorun.inf\lpt1.UsbFix, which I can see in Windows File Explorer,
                //but which I can't delete. It says, this file does not exist anymore.
                //And of course such file causes exception, when trying to calculate its size. Ignore the case.
                if (!File.Exists(file)) continue;

                var fileModel = new FileModel
                {
                    Name = file.Substring(file.LastIndexOf("\\", StringComparison.Ordinal) + 1),
                    Path = path,
                    ChangedDate = File.GetLastWriteTime(path),
                    Size = (new FileInfo(file)).Length
                };
                var icon = Icon.ExtractAssociatedIcon(file);
                //var fullPath = string.Concat(fileModel.Path, @"\", fileModel.Name);
                var fullPath = Path.Combine(fileModel.Path, fileModel.Name);
                fileModel.ChangedDate = File.GetLastWriteTime(fullPath);

                _content.Add(fileModel);
            }
        }
    }
}
