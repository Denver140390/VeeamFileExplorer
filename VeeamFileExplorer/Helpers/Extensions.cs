using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace VeeamFileExplorer.Helpers
{
    static class Extensions
    {
        public static List<TreeViewItem> FindTreeViewItems(this Visual @this)
        {
            if (@this == null)
                return null;

            var result = new List<TreeViewItem>();

            var frameworkElement = @this as FrameworkElement;
            frameworkElement?.ApplyTemplate();

            for (int i = 0, count = VisualTreeHelper.GetChildrenCount(@this); i < count; i++)
            {
                var child = VisualTreeHelper.GetChild(@this, i) as Visual;

                var treeViewItem = child as TreeViewItem;
                if (treeViewItem != null)
                {
                    result.Add(treeViewItem);
//                    if (!treeViewItem.IsExpanded)
//                    {
//                        treeViewItem.IsExpanded = true;
//                        treeViewItem.UpdateLayout();
//                    }
                }
                foreach (var childTreeViewItem in FindTreeViewItems(child))
                {
                    result.Add(childTreeViewItem);
                }
            }
            return result;
        }
    }
}
