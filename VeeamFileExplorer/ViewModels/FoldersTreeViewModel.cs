using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Threading.Tasks;
using GalaSoft.MvvmLight.Messaging;
using VeeamFileExplorer.Helpers;
using VeeamFileExplorer.Models;

namespace VeeamFileExplorer.ViewModels
{
    //TODO Add async check to see if there are new drives or folders?
    class FoldersTreeViewModel : PropertyChangedBase
    {
        private readonly ObservableCollection<FolderModel> _currentDirectoryContent;

        public ObservableCollection<FolderModel> CurrentDirectoryContent
        {
            get { return _currentDirectoryContent; }
            //set { SetProperty(ref _currentDirectoryContent, value, () => CurrentDirectoryContent); }
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
                    FullPath = logicalDrive
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

        public async Task LoadDirectoryFoldersAsync(string path)
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
            foreach (string folderPath in directories)
            {
                var folder = new FolderModel();
                
                await Task.Run(() => CreateFolderModel(folderPath, folder));
                //CreateFolderModel(folderPath, folder);
                _currentDirectoryContent.Add(folder);
            }
        }

        private void CreateFolderModel(string folderPath, FolderModel folder)
        {
            folder.Name = folderPath.Substring(folderPath.LastIndexOf("\\", StringComparison.Ordinal) + 1);
            folder.FullPath = folderPath;

            try
            {
                var subfoldersCount = Directory.GetDirectories(folder.FullPath).Length;
                folder.HasSubfolders = subfoldersCount != 0;
                folder.IsAccessible = true;
            }
                catch (Exception)
            {
                folder.IsAccessible = false;
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
