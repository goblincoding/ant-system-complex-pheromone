using AntSimComplex.Dialogs;
using AntSimComplex.UserControls;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
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
using TspLibNet;

namespace AntSimComplex
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private const string _packageRelPath = @"..\..\..\packages";
        private const string _libPathRegistryKey = @"HKEY_CURRENT_USER\Software\AntSim\TSPLIB95Path";

        private string _tspLibPath;
        private TspLib95 _tspLib;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void MenuOpen_Click(object sender, RoutedEventArgs e)
        {
        }

        private void CreateTspLib()
        {
            // Load all symmetric TSP instances.
            _tspLib = new TspLib95(_tspLibPath);
            _tspLib.LoadAllTSP();
        }

        private void PopulateComboBoxes()
        {
            TSPCombo.ItemsSource = from p in _tspLib.TSPItems()
                                   where p.Problem.NodeProvider.CountNodes() <= 100
                                   select p.Problem.Name;

            TSPCombo.SelectedIndex = 0;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Initialise();
        }

        private void BrowseTSPLIBPath()
        {
            _tspLibPath = (string)Registry.GetValue(_libPathRegistryKey, "", "");

            var tspPathExists = Directory.Exists(_tspLibPath);
            var startDirectory = System.IO.Path.GetFullPath(_packageRelPath);

            do
            {
                var dialog = new DirectoryBrowserDialog(_tspLibPath, startDirectory);
                dialog.Owner = this;
                dialog.ShowDialog();

                _tspLibPath = dialog.DirectoryPath;
                tspPathExists = Directory.Exists(_tspLibPath);

                if (!tspPathExists)
                {
                    MessageBox.Show(this, "Path to TSPLIB95 is invalid.", "Error!");
                }
            } while (String.IsNullOrWhiteSpace(_tspLibPath) ||
                   !tspPathExists);

            Registry.SetValue(_libPathRegistryKey, "", _tspLibPath);
        }

        private void Initialise()
        {
            BrowseTSPLIBPath();
            CreateTspLib();
            PopulateComboBoxes();
        }

        private void Window_Closed(object sender, EventArgs e)
        {
        }
    }
}