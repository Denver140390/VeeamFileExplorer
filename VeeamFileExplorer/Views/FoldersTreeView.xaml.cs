using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using VeeamFileExplorer.Models;
using VeeamFileExplorer.ViewModels;

namespace VeeamFileExplorer.Views
{
    /// <summary>
    /// Interaction logic for FoldersTreeView.xaml
    /// </summary>
    public partial class FoldersTreeView : UserControl
    {
        public FoldersTreeView()
        {
            InitializeComponent();
        }

        private void TreeView_OnLoaded(object sender, RoutedEventArgs e)
        {
            var dummy = new object();

            //ToDo Check DataContext cast to FoldersTreeViewModel
            foreach (var directory in ((FoldersTreeViewModel)DataContext).Directories)
            {
                var item = new TreeViewItem
                {
                    Header = directory.Name,
                    Tag = directory.Name,
                    FontWeight = FontWeights.Normal
                };
                item.Items.Add(dummy);
                //item.Expanded += new RoutedEventHandler(folder_Expanded);
                ((TreeView)sender).Items.Add(item);
            }
        }
    }
}
