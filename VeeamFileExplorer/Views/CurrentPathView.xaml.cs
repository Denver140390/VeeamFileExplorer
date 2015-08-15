using System.Windows.Controls;
using System.Windows.Input;
using VeeamFileExplorer.ViewModels;

namespace VeeamFileExplorer.Views
{
    public partial class CurrentPathView : UserControl
    {
        public CurrentPathView()
        {
            InitializeComponent();
        }

        private void TextBox_KeyDown(object sender, KeyEventArgs e)
        {
            var textBox = (TextBox) sender;
            if (e.Key == Key.Enter)
            {
                CurrentPathViewModel.Instance.LoadNewPath(textBox.Text);
            }
        }
    }
}
