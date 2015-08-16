using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Security.Permissions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using VeeamFileExplorer.Helpers;
using VeeamFileExplorer.ViewModels;

namespace VeeamFileExplorer.Views
{
    public partial class FoldersTreeView : UserControl
    {
        private FoldersTreeViewModel _foldersTreeViewModel;
        private TreeViewItem _currentTreeViewItem;
        private readonly object _dummyItem = new object();
        private Dispatcher _mainDispatcher;

        public FoldersTreeView()
        {
            InitializeComponent();
        }

        private void TreeView_OnLoaded(object sender, RoutedEventArgs e)
        {
            _mainDispatcher = Dispatcher.CurrentDispatcher;

            _foldersTreeViewModel = DataContext as FoldersTreeViewModel;
            if (_foldersTreeViewModel == null) throw new Exception("Could not cast the DataContext to FoldersTreeViewModel!");
            _foldersTreeViewModel.CurrentDirectoryContent.CollectionChanged += CurrentDirectoryContent_CollectionChanged;
            CurrentPathViewModel.Instance.OnPathChangedEvent += OnPathChanged;
            _foldersTreeViewModel.LoadLogicalDrives();

            foreach (var directory in _foldersTreeViewModel.CurrentDirectoryContent)
            {
                var item = new TreeViewItem
                {
                    Header = directory.Name,
                    Tag = directory.Name,
                    FontWeight = FontWeights.Normal
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

        private async void OnPathChanged(object sender, EventArgs e)
        {
            string newPath = CurrentPathViewModel.Instance.Value;

            var pathParts = newPath.Split('\\').ToList();
            if (pathParts[pathParts.Count - 1].Equals(String.Empty))
            {
                pathParts.RemoveAt(pathParts.Count - 1);
            }

            int level = 0;
            var oldItems = new List<TreeViewItem>();
            TreeViewItem lastItem = null;
            while (level < pathParts.Count)
            {
                var items = this.FindTreeViewItems();
//                await Task.Run(() =>
//                {
//                    while (items.Count == oldItems.Count)
//                    {
//                        _mainDispatcher.Invoke(() => items = this.FindTreeViewItems());
//                    }
//                });
//                oldItems = items;
                List<string> headers = items.Select(item => item.Header.ToString()).ToList();
//                var matches = from item in items
//                              where headers.All(header => header.Contains(pathParts[level]))
//                              select item;
//                var match = matches.FirstOrDefault();
//                match.IsExpanded = true;
//                lastItem = match;

                foreach (var item in this.FindTreeViewItems())
                {
                    string header = item.Header.ToString();
                    string pathPart = pathParts[level];
                    if (header.Equals(pathPart) || header.Equals(String.Concat(pathPart, "\\")))
                    {
                        item.IsExpanded = true;
                        lastItem = item;
                        DoEvents();

//                        item.UpdateLayout();
//                        this.UpdateLayout();
//                        await Task.Delay(50);

                        break;
                    }
                }
                level++;
            }
            if (lastItem != null)
                lastItem.IsSelected = true;
        }

        [SecurityPermission(SecurityAction.Demand, Flags = SecurityPermissionFlag.UnmanagedCode)]
        public void DoEvents()
        {
            DispatcherFrame frame = new DispatcherFrame();
            Dispatcher.CurrentDispatcher.BeginInvoke(DispatcherPriority.Background,
                new DispatcherOperationCallback(ExitFrame), frame);
            Dispatcher.PushFrame(frame);
        }

        public object ExitFrame(object f)
        {
            ((DispatcherFrame)f).Continue = false;

            return null;
        }

        private async void TreeViewItem_Expanded(object sender, RoutedEventArgs e)
        {
            var item = (TreeViewItem)sender;
            if (!item.IsExpanded) return; // we want to perform only with expanded one
            var path = item.Tag.ToString();
            if (item.Items.Count != 1 || item.Items[0] != _dummyItem) return;
            item.Items.Clear();
            _currentTreeViewItem = item;

            var loadingTask = _foldersTreeViewModel.LoadDirectoryFoldersAsync(path);
            await loadingTask;

            //TODO !!! Large collections still cause lags...
//            try
//            {
//                foreach (var folder in _foldersTreeViewModel.CurrentDirectoryContent)
//                {
//                    var subItem = new TreeViewItem
//                    {
//                        Header = folder.Name,
//                        //Tag = string.Concat(directory.Path, @"\", directory.Name),
//                        Tag = folder.FullPath,
//                        FontWeight = FontWeights.Normal
//                    };
//                    if (folder.IsAccessible && folder.HasSubfolders)
//                    {
//                        subItem.Items.Add(_dummyItem);
//                    }
//                    subItem.Expanded += TreeViewItem_Expanded;
//                    subItem.Selected += TreeViewItem_Selected;
//                    item.Items.Add(subItem);
//                }
//            }
//            catch (Exception ex)
//            {
//                throw new Exception(ex.Message);
//            }
        }

        private void CurrentDirectoryContent_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            int lastAddedIndex = _foldersTreeViewModel.CurrentDirectoryContent.Count - 1;
            if (_currentTreeViewItem == null || lastAddedIndex < 0) return;

            var folder = _foldersTreeViewModel.CurrentDirectoryContent[lastAddedIndex];
            var subItem = new TreeViewItem
            {
                Header = folder.Name,
                //Tag = string.Concat(directory.Path, @"\", directory.Name),
                Tag = folder.FullPath,
                FontWeight = FontWeights.Normal
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
            var treeViewItem = VisualUpwardSearch(e.OriginalSource as DependencyObject);

            if (treeViewItem != null)
            {
                treeViewItem.Focus();
                e.Handled = true;
            }
        }

        private static TreeViewItem VisualUpwardSearch(DependencyObject source)
        {
            while (source != null && !(source is TreeViewItem))
                source = VisualTreeHelper.GetParent(source);

            return source as TreeViewItem;
        }

        private void ContextMenuItem_Click(object sender, RoutedEventArgs e)
        {
            CurrentPathViewModel.Instance.OpenInWindowsExplorer();
        }
    }
}