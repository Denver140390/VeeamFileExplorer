﻿using System;
using System.Collections.ObjectModel;
using System.IO;
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
                    Name = logicalDrive,
                    Path = logicalDrive
                    //ToDo async size calculation
                };

                _currentDirectoryContent.Add(folder);
            }
        }

        public void LoadDirectoryContent(string path)
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
                    Name = folderName,
                    Path = folderName
                    //ToDo async size calculation
                };

                _currentDirectoryContent.Add(folder);
            }
        }
    }
}
