using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using VeeamFileExplorer.Helpers;
using VeeamFileExplorer.ViewModels;

namespace VeeamFileExplorer.Views
{
    public partial class FoldersTreeView : UserControl
    {
        private readonly FoldersTreeViewModel _foldersTreeViewModel;
        private TreeViewItem _currentTreeViewItem;
        private readonly object _dummyItem = new object();

        public FoldersTreeView()
        {
            InitializeComponent();

            _foldersTreeViewModel = (FoldersTreeViewModel) DataContext;
            _foldersTreeViewModel.CurrentDirectoryContent.CollectionChanged += CurrentDirectoryContent_CollectionChanged;
        }

        private void TreeView_OnLoaded(object sender, RoutedEventArgs e)
        {
            CurrentPathViewModel.Instance.OnPathChangedEvent += OnPathChanged;
            _foldersTreeViewModel.LoadLogicalDrives();

            foreach (var directory in _foldersTreeViewModel.CurrentDirectoryContent)
            {
                var item = new TreeViewItem
                {
                    Header = directory.Name,
                    Tag = directory.FullPath
                };
                if (directory.IsAccessible && directory.HasSubfolders)
                {
                    item.Items.Add(_dummyItem);
                }
                item.Expanded += TreeViewItem_Expanded;
                item.Selected += TreeViewItem_Selected;
                
                ((TreeView)sender).Items.Add(item);
            }
        }

        private void OnPathChanged(object sender, EventArgs e)
        {
            string newPath = CurrentPathViewModel.Instance.Value;

            var pathParts = newPath.Split('\\').ToList();
            if (pathParts[pathParts.Count - 1].Equals(String.Empty))
            {
                pathParts.RemoveAt(pathParts.Count - 1);
            }

            OpenFolder(pathParts);
        }

        private void OpenFolder(List<string> pathParts)
        {
            int level = 0;
            TreeViewItem lastItem = null;
            while (level < pathParts.Count)
            {
                foreach (var item in this.FindTreeViewItems())
                {
                    string header = item.Header.ToString();
                    string pathPart = pathParts[level];
                    //BUG There might be few folders with equal names. In that case only the first one will be opened
                    if (header.Equals(pathPart) || header.Equals(String.Concat(pathPart, "\\")))
                    {
                        item.IsExpanded = true;
                        lastItem = item;
                        Application.Current.DoEvents(); // wait the item to expand
                        Application.Current.DoEvents(); // TODO Calling DoEvents() twice does the job for some reason... Or not...
                        break;
                    }
                }
                level++;
            }
            if (lastItem != null)
                lastItem.IsSelected = true;
        }

        private async void TreeViewItem_Expanded(object sender, RoutedEventArgs e)
        {
            //TODO Unload items if Expanded == false
            var item = (TreeViewItem)sender;
            if (!item.IsExpanded) return; // we want to perform only with expanded one
            var path = item.Tag.ToString();
            if (item.Items.Count != 1 || item.Items[0] != _dummyItem) return;
            item.Items.Clear();
            _currentTreeViewItem = item;

            var loadingTask = _foldersTreeViewModel.LoadDirectoryFoldersAsync(path);
            await loadingTask;

            //BUG Large collections still cause lags...
        }

        private void CurrentDirectoryContent_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            int lastAddedIndex = _foldersTreeViewModel.CurrentDirectoryContent.Count - 1;
            if (_currentTreeViewItem == null || lastAddedIndex < 0) return;

            var folder = _foldersTreeViewModel.CurrentDirectoryContent[lastAddedIndex];
            var subItem = new TreeViewItem
            {
                Header = folder.Name,
                Tag = folder.FullPath
            };
            if (folder.IsAccessible && folder.HasSubfolders)
            {
                subItem.Items.Add(_dummyItem);
            }
            subItem.Expanded += TreeViewItem_Expanded;
            subItem.Selected += TreeViewItem_Selected;

            _currentTreeViewItem.Items.Add(subItem);
        }

        private void TreeViewItem_Selected(object sender, RoutedEventArgs routedEventArgs)
        {
            var item = (TreeViewItem)sender;
            if (!item.IsSelected) return; // we want to perform only with selected one

            var path = item.Tag.ToString();
            _foldersTreeViewModel.SetSelectedFolder(path);
            _foldersTreeViewModel.SetCurrentPath(path);
        }

        private void TreeView_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            TreeView.ContextMenu = Resources["FolderContext"] as ContextMenu;
        }

        private void TreeView_PreviewMouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            var treeViewItem = this.VisualUpwardSearch(e.OriginalSource as DependencyObject);

            if (treeViewItem == null) return;
            treeViewItem.Focus();
            e.Handled = true;
        }

        private void ContextMenuItem_Click(object sender, RoutedEventArgs e)
        {
            CurrentPathViewModel.Instance.OpenInWindowsExplorer();
        }
    }
}