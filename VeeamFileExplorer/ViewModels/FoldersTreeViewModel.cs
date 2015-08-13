using System;
using System.Collections.ObjectModel;
using System.IO;
using GalaSoft.MvvmLight.Messaging;
using VeeamFileExplorer.Helpers;
using VeeamFileExplorer.Models;

namespace VeeamFileExplorer.ViewModels
{
    //TODO Add async check to see if there are new drives or folders?
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
                    Name = logicalDrive,
                    Path = String.Empty
                };

                _currentDirectoryContent.Add(folder);

                try
                {
                    var subfoldersCount = Directory.GetDirectories(folder.Name).Length;
                    folder.HasSubfolders = subfoldersCount != 0;
                    folder.IsAccessible = true;
                }
                catch (Exception)
                {
                    folder.IsAccessible = false;
                }
            }
        }

        public void LoadDirectoryFolders(string path)
        {
            string[] directories;
            try
            {
                directories = Directory.GetDirectories(path);
            }
            catch (Exception)
            {
                directories = new string[0];
                //throw new Exception(e.Message);
            }

            _currentDirectoryContent.Clear();
            foreach (string folderName in directories)
            {
                var folder = new FolderModel
                {
                    Name = folderName.Substring(folderName.LastIndexOf("\\", StringComparison.Ordinal) + 1),
                    Path = path
                };

                try
                {
                    //var subfoldersCount = Directory.GetDirectories(string.Concat(folder.Path, @"\", folder.Name)).Length;
                    var subfoldersCount = Directory.GetDirectories(Path.Combine(folder.Path, folder.Name)).Length;
                    folder.HasSubfolders = subfoldersCount != 0;
                    folder.IsAccessible = true;
                }
                catch (Exception)
                {
                    folder.IsAccessible = false;
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
