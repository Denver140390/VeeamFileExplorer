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
    class FoldersTreeViewModel : PropertyChangedBase
    {
        private ObservableCollection<FolderModel> _currentDirectoryContent;

        public ObservableCollection<FolderModel> CurrentDirectoryContent
        {
            get { return _currentDirectoryContent; }
            set { SetProperty(ref _currentDirectoryContent, value, () => CurrentDirectoryContent); }
        }

        public FoldersTreeViewModel()
        {
            _currentDirectoryContent = new ObservableCollection<FolderModel>();
        }

        public void LoadLogicalDrives()
        {
            foreach (string logicalDrive in Directory.GetLogicalDrives())
            {
                var folder = new FolderModel
                {
                    Name = logicalDrive
                };

                _currentDirectoryContent.Add(folder);
            }
        }

        public void LoadDirectoryFolders(string path)
        {
            string[] directories;
            try
            {
                directories = Directory.GetDirectories(path);
            }
            catch (Exception e)
            {
                directories = new string[0];
                //throw new Exception(e.Message);
            }

            _currentDirectoryContent.Clear();
            foreach (string folderName in directories)
            {
                var name = folderName.Substring(folderName.LastIndexOf("\\", StringComparison.Ordinal) + 1);
                var folder = new FolderModel
                {
                    Name = name,
                    Path = path,
                };

                try
                {
                    folder.HasSubfolders = Directory.GetDirectories(string.Concat(path, @"\", name)).Length != 0;
                }
                catch (Exception)
                {
                    folder.HasSubfolders = false;
                }
                _currentDirectoryContent.Add(folder);
            }
        }

        public void SetCurrentPath(string path)
        {
            CurrentPathViewModel.Instance.Value = path;
        }

        public void SetSelectedFolder(string path)
        {
            //TODO Use own Messenger. 4 references for 1 feature is not cool (MVVMLight toolkit for Messenger system). It feels wrong.
            Messenger.Default.Send(path);
        }
    }
}
