using System.Collections.ObjectModel;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using VeeamFileExplorer.Helpers;
using VeeamFileExplorer.Models;

namespace VeeamFileExplorer.ViewModels
{
    class FoldersTreeViewModel : PropertyChangedBase
    {
        private ObservableCollection<FolderModel> _directories;

        public ObservableCollection<FolderModel> Directories
        {
            get { return _directories; }
            set { SetProperty(ref _directories, value, () => Directories); }
        }

        public FoldersTreeViewModel()
        {
            _directories = new ObservableCollection<FolderModel>();

            LoadLogicalDrives();
        }

        public void LoadLogicalDrives()
        {
            _directories.Add(new FolderModel
            {
                Name = "1",
                Path = "1"
            });

            foreach (var logicalDrive in Directory.GetLogicalDrives())
            {
                var folder = new FolderModel
                {
                    Name = logicalDrive,
                    Path = logicalDrive
                };

                _directories.Add(folder);
            }
        }
    }
}
