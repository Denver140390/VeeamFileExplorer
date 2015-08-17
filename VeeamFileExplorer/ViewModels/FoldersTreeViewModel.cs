using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Threading.Tasks;
using VeeamFileExplorer.Helpers;
using VeeamFileExplorer.Helpers.Messenger;
using VeeamFileExplorer.Models;

namespace VeeamFileExplorer.ViewModels
{
    class FoldersTreeViewModel : PropertyChangedBase
    {
        public ObservableCollection<FolderModel> CurrentDirectoryContent { get; }

        public FoldersTreeViewModel()
        {
            CurrentDirectoryContent = new ObservableCollection<FolderModel>();
            var fileSystemWatcher = new FileSystemWatcher("D:\\");
            fileSystemWatcher.EnableRaisingEvents = true;
            //TODO Use FileSystemWatcher for detecting file system changes
            fileSystemWatcher.Changed += (sender, args) =>
            {

            };
            fileSystemWatcher.Created += (sender, args) =>
            {

            };
            fileSystemWatcher.Deleted += (sender, args) =>
            {

            };
            fileSystemWatcher.Renamed += (sender, args) =>
            {

            };
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

                CurrentDirectoryContent.Add(folder);

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
            
            CurrentDirectoryContent.Clear();
            int foldersCount = 0;
            foreach (string folderPath in directories)
            {
                var folder = new FolderModel();
                
                await Task.Run(() => CreateFolderModel(folderPath, folder));
                //CreateFolderModel(folderPath, folder);
                CurrentDirectoryContent.Add(folder);
                foldersCount++;
//                if (foldersCount % 100 == 0)
//                {
//                    await Task.Delay(10); // give time for UI to response
//                }
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
            Messenger.Default.Send(path);
        }
    }
}
