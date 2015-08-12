using AntSimComplex.Dialogs;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using TspLibNet;
using TspLibNet.Graph.Nodes;

namespace AntSimComplex
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private const int _circleWidth = 10;
        private const string _packageRelPath = @"..\..\..\packages";
        private const string _libPathRegistryKey = @"HKEY_CURRENT_USER\Software\AntSim\TSPLIB95Path";

        private string _tspLibPath;
        private TspLib95 _tspLib;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Initialise();
        }

        private void Initialise()
        {
            BrowseTSPLIBPath();
            CreateTspLib();
            PopulateComboBoxes();
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
            } while (String.IsNullOrWhiteSpace(_tspLibPath) || !tspPathExists);

            Registry.SetValue(_libPathRegistryKey, "", _tspLibPath);
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
        }

        private void TSPCombo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var ratio = canvas.ActualWidth / canvas.ActualHeight;
            var item = _tspLib.GetItemByName(TSPCombo.SelectedItem.ToString(), ProblemType.TSP);
            var nodes = item.Problem.NodeProvider.GetNodes();

            foreach (var node in nodes)
            {
                var node2D = node as Node2D;
                var x = node2D.X;
                var y = node2D.Y;
            }

            List<Point> points = new List<Point>() { };

            for (int j = 0; j < 10; j++)
            {
                for (int k = 0; k < 12; k++)
                {
                    points.Add(new Point(j * 10, k * 12));
                }
            }

            Ellipse[] ellipsePoints = new Ellipse[120];

            for (int j = 0; j < 120; j++)
            {
                ellipsePoints[j] = new Ellipse() { Width = _circleWidth, Height = _circleWidth, Fill = Brushes.Black };
                canvas.Children.Add(ellipsePoints[j]);
            }

            for (int i = 0; i < points.Count; i++)
            {
                Canvas.SetLeft(ellipsePoints[i], points[i].X - ellipsePoints[i].Width / 2);
                Canvas.SetTop(ellipsePoints[i], points[i].Y - ellipsePoints[i].Height / 2);
            }

            //var desiredCenterX = canvas.Width / 2;
            //var desiredCenterY = canvas.Height / 2;
            //var ellipse = CreateEllipse(40, 40, desiredCenterX, desiredCenterY);
            //var solidColorBrush = new SolidColorBrush();
            //solidColorBrush.Color = Color.FromArgb(255, 255, 255, 0);
            //ellipse.Fill = solidColorBrush;
            //ellipse.StrokeThickness = 2;
            //ellipse.Stroke = Brushes.Black;

            //Canvas.SetLeft(ellipse, desiredCenterX - (canvas.Width / 2));
            //Canvas.SetTop(ellipse, desiredCenterY - (canvas.Height / 2));
        }
    }
}