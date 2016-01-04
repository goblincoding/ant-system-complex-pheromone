using AntSimComplexUI.Utilities;
using System;
using System.Windows;

namespace AntSimComplexUI.UserControls
{
  /// <summary>
  /// Interaction logic for DirectoryBrowser.xaml
  /// </summary>
  public partial class DirectoryBrowserControl
  {
    public event EventHandler<DirPathEventArgs> DirectoryAccepted = delegate { };

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

    private void OkButtonClick(object sender, RoutedEventArgs e)
    {
      DirectoryAccepted(this, new DirPathEventArgs(DirectoryPath));
    }

    private void BrowseButtonClick(object sender, RoutedEventArgs e)
    {
      var dialog = new System.Windows.Forms.FolderBrowserDialog { SelectedPath = StartDirectory };
      var result = dialog.ShowDialog();
      if (result == System.Windows.Forms.DialogResult.OK)
      {
        DirectoryPath = dialog.SelectedPath;
      }
    }
  }
}