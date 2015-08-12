using System;
using System.Collections.ObjectModel;
using System.IO;
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

            LoadLogicalDrives();
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
                throw new Exception(e.Message);
            }

            _currentDirectoryContent.Clear();
            foreach (string folderName in directories)
            {
                var folder = new FolderModel
                {
                    Name = folderName.Substring(folderName.LastIndexOf("\\", StringComparison.Ordinal) + 1),
                    Path = path
                };

                _currentDirectoryContent.Add(folder);
            }
        }

        public void SetCurrentPath(string path)
        {
            CurrentPathViewModel.Instance.Value = path;
        }

        public void SetSelectedFolder(string path)
        {
            //TODO Use own Messenger. 4 references for 1 feature is not cool. It feels wrong.
            Messenger.Default.Send(path);
        }
    }
}
