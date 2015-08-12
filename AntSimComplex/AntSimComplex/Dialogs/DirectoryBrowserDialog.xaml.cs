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
using System.Windows.Shapes;

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

        private void DirectoryAccepted(object sender, string dirPath)
        {
            DirectoryPath = dirPath;
            Close();
        }
    }
}