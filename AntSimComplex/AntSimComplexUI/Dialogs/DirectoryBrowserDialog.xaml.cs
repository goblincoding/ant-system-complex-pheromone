using AntSimComplexUI.Utilities;

namespace AntSimComplexUI.Dialogs
{
  /// <summary>
  /// Interaction logic for DirectoryBrowserDialog.xaml
  /// </summary>
  public partial class DirectoryBrowserDialog
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