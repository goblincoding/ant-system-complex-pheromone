using AntSimComplex.Utilities;
using System.Windows;

namespace AntSimComplex.Dialogs
{
    /// <summary>
    /// Interaction logic for DirectoryBrowserDialog.xaml
    /// </summary>
    public partial class DirectoryBrowserDialog : Window
    {
        public string DirectoryPath { get; internal set; }

        public DirectoryBrowserDialog(string savedPath, string startDirectory)
        {
            InitializeComponent();

            dirBrowser.DirectoryAccepted += DirectoryAccepted;
            dirBrowser.DirectoryPath = savedPath;
            dirBrowser.StartDirectory = startDirectory;
        }

        private void DirectoryAccepted(object sender, StringEventArgs stringArgs)
        {
            DirectoryPath = stringArgs.DirPath;
            Close();
        }
    }
}