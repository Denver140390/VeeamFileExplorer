using System;
using System.Windows;
using System.Windows.Controls;
using VeeamFileExplorer.ViewModels;

namespace VeeamFileExplorer.Views
{
    public partial class FoldersTreeView : UserControl
    {
        private FoldersTreeViewModel _foldersTreeViewModel;
        private readonly object _dummyItem = new object();

        public FoldersTreeView()
        {
            InitializeComponent();
        }

        private void TreeView_OnLoaded(object sender, RoutedEventArgs e)
        {
            _foldersTreeViewModel = DataContext as FoldersTreeViewModel;
            if (_foldersTreeViewModel == null) throw new Exception("Could not cast the DataContext to FoldersTreeViewModel!");
            _foldersTreeViewModel.LoadLogicalDrives();

            foreach (var directory in _foldersTreeViewModel.CurrentDirectoryContent)
            {
                var item = new TreeViewItem
                {
                    Header = directory.Name,
                    Tag = directory.Name,
                    FontWeight = FontWeights.Normal
                };
                item.Items.Add(_dummyItem);
                item.Expanded += TreeViewItem_Expanded;
                item.Selected += TreeViewItem_Selected;
                
                ((TreeView)sender).Items.Add(item);
            }
        }

        private void TreeViewItem_Expanded(object sender, RoutedEventArgs e)
        {
            var item = (TreeViewItem)sender;
            if (!item.IsExpanded) return; // we want to perform only with expanded one
            var path = item.Tag.ToString();
            if (item.Items.Count != 1 || item.Items[0] != _dummyItem) return;
            item.Items.Clear();

            _foldersTreeViewModel.LoadDirectoryFolders(path);

            //TODO Asynchronous check for subfolders content
            try
            {
                foreach (var directory in _foldersTreeViewModel.CurrentDirectoryContent)
                {
                    var subItem = new TreeViewItem
                    {
                        Header = directory.Name,
                        Tag = string.Concat(directory.Path, @"\", directory.Name),
                        FontWeight = FontWeights.Normal
                    };
                    if (directory.HasSubfolders)
                    {
                        subItem.Items.Add(_dummyItem);
                    }
                    subItem.Expanded += TreeViewItem_Expanded;
                    subItem.Selected += TreeViewItem_Selected;
                    item.Items.Add(subItem);
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        private void TreeViewItem_Selected(object sender, RoutedEventArgs routedEventArgs)
        {
            var item = (TreeViewItem)sender;
            if (!item.IsSelected) return; // we want to perform only with selected one

            var path = item.Tag.ToString();
            _foldersTreeViewModel.SetSelectedFolder(path);
            _foldersTreeViewModel.SetCurrentPath(path);
        }
    }
}
