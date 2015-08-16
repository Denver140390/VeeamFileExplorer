using System;
using System.Windows.Media.Imaging;

namespace VeeamFileExplorer.Models
{
    class FolderModel : FileModelBase
    {
        public new string Extension { get; } = "Folder";

        public bool HasSubfolders { get; set; }
        public bool IsAccessible { get; set; }

        public FolderModel()
        {
            var uri = new Uri("pack://application:,,,/Images/folder.png");
            var source = new BitmapImage(uri);
            Icon = source;
        }
    }
}
