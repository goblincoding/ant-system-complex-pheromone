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
        private const string _libPath = @"..\..\..\packages\TSPLib.Net.1.1.0\lib\TSPLIB95";
        private TspLib95 _tspLib;

        public MainWindow()
        {
            CreateTspLib();
            InitializeComponent();
            PopulateComboBoxes();
        }

        private void MenuOpen_Click(object sender, RoutedEventArgs e)
        {
        }

        private void CreateTspLib()
        {
            // Load all symmetric TSP instances.
            _tspLib = new TspLib95(_libPath);
            _tspLib.LoadAllTSP();
        }

        private void PopulateComboBoxes()
        {
            this.TSPCombo.ItemsSource = from p in _tspLib.TSPItems()
                                        where p.Problem.NodeProvider.CountNodes() <= 100
                                        select p.Problem.Name;

            //this.TSPCombo.ItemsSource = _tspLib.TSPItems().Select(i => i.Problem.Name);
        }
    }
}