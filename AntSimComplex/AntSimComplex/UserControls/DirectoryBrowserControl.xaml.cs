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

namespace AntSimComplex.UserControls
{
    /// <summary>
    /// Interaction logic for DirectoryBrowser.xaml
    /// </summary>
    public partial class DirectoryBrowserControl : UserControl
    {
        public event EventHandler<string> DirectoryAccepted = delegate { };

        public string DirectoryPath
        {
            get
            {
                return BrowseTextBox.Text;
            }

            internal set
            {
                BrowseTextBox.Text = value;
            }
        }

        public string StartDirectory { get; internal set; }

        public DirectoryBrowserControl()
        {
            InitializeComponent();
        }

        private void OKButton_Click(object sender, RoutedEventArgs e)
        {
            DirectoryAccepted(this, DirectoryPath);
        }

        private void BrowseButton_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new System.Windows.Forms.FolderBrowserDialog();
            dialog.SelectedPath = StartDirectory;

            var result = dialog.ShowDialog();
            if (result == System.Windows.Forms.DialogResult.OK)
            {
                DirectoryPath = dialog.SelectedPath;
            }
        }
    }
}