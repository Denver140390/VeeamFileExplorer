using System;
using System.IO;
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
                ((TreeView)sender).Items.Add(item);
            }
        }

        private void TreeViewItem_Expanded(object sender, RoutedEventArgs e)
        {
            var item = (TreeViewItem)sender;
            if (item.Items.Count != 1 || item.Items[0] != _dummyItem) return;
            item.Items.Clear();

            _foldersTreeViewModel.LoadDirectoryContent(item.Tag.ToString());

            try
            {
                foreach (var directory in _foldersTreeViewModel.CurrentDirectoryContent)
                {
                    var subItem = new TreeViewItem
                    {
                        Header = directory.Name,
                        Tag = directory.Path,
                        FontWeight = FontWeights.Normal
                    };
                    subItem.Items.Add(_dummyItem);
                    subItem.Expanded += TreeViewItem_Expanded;
                    item.Items.Add(subItem);
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
