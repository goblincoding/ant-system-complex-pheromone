using AntSimComplexUI.Utilities;
using System.Windows;

namespace AntSimComplexUI.Dialogs
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

      DirBrowser.DirectoryAccepted += DirectoryAccepted;
      DirBrowser.DirectoryPath = savedPath;
      DirBrowser.StartDirectory = startDirectory;
    }

    private void DirectoryAccepted(object sender, DirPathEventArgs stringArgs)
    {
      DirectoryPath = stringArgs.DirPath;
      Close();
    }
  }
}