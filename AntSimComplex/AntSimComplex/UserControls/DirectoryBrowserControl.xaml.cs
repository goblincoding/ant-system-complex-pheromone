using AntSimComplexUI.Utilities;
using System;
using System.Windows;
using System.Windows.Controls;

namespace AntSimComplexUI.UserControls
{
    /// <summary>
    /// Interaction logic for DirectoryBrowser.xaml
    /// </summary>
    public partial class DirectoryBrowserControl : UserControl
    {
        public event EventHandler<StringEventArgs> DirectoryAccepted = delegate { };

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
            DirectoryAccepted(this, new StringEventArgs(DirectoryPath));
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