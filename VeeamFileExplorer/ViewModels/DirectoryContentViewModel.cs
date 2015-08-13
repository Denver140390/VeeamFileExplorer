using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Threading.Tasks;
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
                FullPath = "D:\\Folder",
                ChangedDate = DateTime.Now
            };

            _content.Add(folderModel);

            var fileModel = new FileModel
            {
                Name = "File",
                FullPath = "D:\\File",
                ChangedDate = DateTime.Now,
                Size = 123,
                Extension = "no"
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
                    FullPath = folder,
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

//                var fileInfo = new FileInfo(file);
//                var fileModel = new FileModel
//                {
//                    Name = file.Substring(file.LastIndexOf("\\", StringComparison.Ordinal) + 1),
//                    FullPath = file,
//                    ChangedDate = File.GetLastWriteTime(file),
//                    Size = fileInfo.Length,
//                    Extension = fileInfo.Extension,
//                    Icon = Icon.ExtractAssociatedIcon(file)
//                };
//

                var fileModel = new FileModel();
                CreateFileModel(file, fileModel);
                //CreateFileModelAsync(file, fileModel);
                _content.Add(fileModel);
            }

            //CalculateFileSizeAsync();
        }

        private void CreateFileModel(string filePath, FileModel file)
        {
            var fileInfo = new FileInfo(filePath);

            file.Name = filePath.Substring(filePath.LastIndexOf("\\", StringComparison.Ordinal) + 1);
            file.FullPath = filePath;
            file.ChangedDate = File.GetLastWriteTime(filePath);
            file.Size = fileInfo.Length;
            file.Extension = fileInfo.Extension;
            file.Icon = Icon.ExtractAssociatedIcon(filePath);
        }

        private async void CreateFileModelAsync(string filePath, FileModel file)
        {
            await Task.Run(() =>
            {
                var fileInfo = new FileInfo(filePath);

                file.Name = filePath.Substring(filePath.LastIndexOf("\\", StringComparison.Ordinal) + 1);
                file.FullPath = filePath;
                file.ChangedDate = File.GetLastWriteTime(filePath);
                file.Size = fileInfo.Length;
                file.Extension = fileInfo.Extension;
                file.Icon = Icon.ExtractAssociatedIcon(filePath);
            });
        }

        private async void CalculateFileSizeAsync()
        {
            await Task.Run(() =>
            {
                foreach(var file in _content)
            {
                    var fileModel = file as FileModel;
                    if (fileModel != null)
                    {
                        var fileInfo = new FileInfo(fileModel.FullPath);
                        file.Size = fileInfo.Length;
                    }
                }
            });
        }
    }
}
