using System.Diagnostics;
using System.Windows.Controls;
using System.Windows.Input;
using VeeamFileExplorer.Models;
using VeeamFileExplorer.ViewModels;

namespace VeeamFileExplorer.Views
{
    public partial class DirectoryContentView : UserControl
    {
        public DirectoryContentView()
        {
            InitializeComponent();
        }

        private void DataGridRow_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var row = (DataGridRow) sender;
            var file = (FileModelBase) row.DataContext;
            if (file.GetType() == typeof (FolderModel))
            {
                CurrentPathViewModel.Instance.LoadNewPath(file.FullPath);
            }
            else if (file.GetType() == typeof (FileModel))
            {
                Process.Start(file.FullPath);
            }
        }
    }
}
