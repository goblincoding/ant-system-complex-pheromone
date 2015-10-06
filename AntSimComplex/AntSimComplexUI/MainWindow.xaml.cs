using AntSimComplex.Utilities;
using AntSimComplexAlgorithms;
using AntSimComplexUI.Dialogs;
using AntSimComplexUI.Utilities;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using TspLibNet.Graph.Nodes;

namespace AntSimComplexUI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private const int _circleWidth = 10;
        private const string _packageRelPath = @"..\..\..\packages";
        private const string _libPathRegistryKey = @"HKEY_CURRENT_USER\Software\AntSim\TSPLIB95Path";

        private bool _initialised;
        private bool _drawOptimal;

        private Matrix _worldToCanvasMatrix;
        private Matrix _canvasToWorldMatrix;

        private ObservableCollection<ListViewTourItem> _tourItems;

        private SymmetricTSPItemSelector _tspProblemSelector;
        private SymmetricTSPInfoProvider _tspInfoProvider;
        private AntSystem _antSystem;

        public MainWindow()
        {
            InitializeComponent();
            _tourItems = new ObservableCollection<ListViewTourItem>();
            TourListView.ItemsSource = _tourItems;
        }

        private void Initialise()
        {
            var tspLibPath = BrowseTSPLIBPath();

            // Load all symmetric TSP instances.
            _tspProblemSelector = new SymmetricTSPItemSelector(tspLibPath, 100, typeof(Node2D));
            TSPCombo.ItemsSource = _tspProblemSelector.ProblemNames;
        }

        private string BrowseTSPLIBPath()
        {
            var tspLibPath = (string)Registry.GetValue(_libPathRegistryKey, "", "");
            var tspPathExists = Directory.Exists(tspLibPath);
            var startDirectory = System.IO.Path.GetFullPath(_packageRelPath);

            do
            {
                var dialog = new DirectoryBrowserDialog(tspLibPath, startDirectory);
                dialog.Owner = this;
                dialog.ShowDialog();

                tspLibPath = dialog.DirectoryPath;
                tspPathExists = Directory.Exists(tspLibPath);

                if (!tspPathExists)
                {
                    MessageBox.Show(this, "Path to TSPLIB95 is invalid.", "Error!");
                }
            } while (String.IsNullOrWhiteSpace(tspLibPath) || !tspPathExists);

            Registry.SetValue(_libPathRegistryKey, "", tspLibPath);
            return tspLibPath;
        }

        private void DrawTspLibItem()
        {
            var worldMinX = _tspInfoProvider.GetMinX();
            var worldMinY = _tspInfoProvider.GetMinY();
            var worldMaxX = _tspInfoProvider.GetMaxX();
            var worldMaxY = _tspInfoProvider.GetMaxY();

            const double margin = 20;
            const double canvasMinX = margin;
            const double canvasMinY = margin;
            var canvasMaxX = canvas.ActualWidth - margin;
            var canvasMaxY = canvas.ActualHeight - margin;

            // Order of canvas Y min and max arguments are swapped due to canvas coordinate
            // system (top-left is 0,0).  This "flips" the coordinate system along the Y
            // axis by making the Y scale value negative so that we have bottom-left at 0,0.
            PrepareTransformationMatrices(worldMinX, worldMaxX, worldMinY, worldMaxY,
                                          canvasMinX, canvasMaxX, canvasMaxY, canvasMinY);

            canvas.Children.Clear();
            DrawNodes();
            DrawSelectedTour();

            if (_drawOptimal)
            {
                DrawOptimalTour();
            }
        }

        private void DrawOptimalTour()
        {
            var nodes = _tspInfoProvider.OptimalTourNodes2D;
            if (!nodes.Any())
            {
                return;
            }

            var optimalLength = _tspInfoProvider.OptimalTourLength;
            DrawTour(nodes, optimalLength, Brushes.Red, Brushes.Green);
        }

        private void DrawSelectedTour()
        {
            var item = TourListView.SelectedItem as ListViewTourItem;
            if (item != null)
            {
                DrawTour(item.Nodes, item.Length, Brushes.CornflowerBlue, Brushes.DodgerBlue);
            }
        }

        private void DrawTour(List<Node2D> nodes, double tourLength, Brush startNodeBrush, Brush lineBrush)
        {
            var points = (from n in nodes
                          select TransformWorldToCanvas(new Point { X = n.X, Y = n.Y })).ToList();

            // Draw the starting node in red for easier identification.
            DrawNode(TransformCanvasToWorld(points.First()), startNodeBrush);

            // Return to starting point.
            points.Add(points.First());

            var poly = new Polyline
            {
                Points = new PointCollection(points),
                Stroke = lineBrush,
                StrokeThickness = 1,
                ToolTip = $"Tour length: {tourLength}"
            };

            canvas.Children.Add(poly);
        }

        private void DrawNodes()
        {
            var points = from n in _tspInfoProvider.Nodes2D
                         select new Point { X = n.X, Y = n.Y };

            foreach (var point in points)
            {
                DrawNode(point, Brushes.Black);
            }
        }

        private void DrawNode(Point point, Brush brush)
        {
            var ellipse = new Ellipse { Width = _circleWidth, Height = _circleWidth, Fill = brush };
            ellipse.ToolTip = $"x: {point.X}, y: {point.Y}";
            canvas.Children.Add(ellipse);
            var transformed = TransformWorldToCanvas(point);
            Canvas.SetLeft(ellipse, transformed.X - ellipse.Width / 2);
            Canvas.SetTop(ellipse, transformed.Y - ellipse.Height / 2);
        }

        private void PrepareTransformationMatrices(double worldMinX, double worldMaxX, double worldMinY, double worldMaxY,
                                                   double canvasMinX, double canvasMaxX, double canvasMinY, double canvasMaxY)
        {
            _worldToCanvasMatrix = Matrix.Identity;
            _worldToCanvasMatrix.Translate(-worldMinX, -worldMinY);

            double xscale = (canvasMaxX - canvasMinX) / (worldMaxX - worldMinX);
            double yscale = (canvasMaxY - canvasMinY) / (worldMaxY - worldMinY);
            _worldToCanvasMatrix.Scale(xscale, yscale);

            _worldToCanvasMatrix.Translate(canvasMinX, canvasMinY);

            _canvasToWorldMatrix = _worldToCanvasMatrix;
            _canvasToWorldMatrix.Invert();
        }

        private Point TransformWorldToCanvas(Point point)
        {
            return _worldToCanvasMatrix.Transform(point);
        }

        private Point TransformCanvasToWorld(Point point)
        {
            return _canvasToWorldMatrix.Transform(point);
        }

        #region eventhandlers

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Initialise();
            _initialised = true;
        }

        private void TSPCombo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var problemName = TSPCombo.SelectedItem?.ToString();
            var item = _tspProblemSelector.GetItem(problemName);
            _tspInfoProvider = new SymmetricTSPInfoProvider(item);
            DrawTspLibItem();

            _antSystem = new AntSystem(item.Problem);
            _tourItems.Clear();

            if (_tspInfoProvider.HasOptimalTour)
            {
                var ids = from n in _tspInfoProvider.OptimalTourNodes2D
                          select n.Id.ToString();
                _tourItems.Add(new ListViewTourItem(_tspInfoProvider.OptimalTourNodes2D,
                                                    _tspInfoProvider.OptimalTourLength,
                                                    $"Current Problem (Optimal): {problemName}"));
            }
        }

        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (_initialised)
            {
                DrawTspLibItem();
            }
        }

        private void ShowOptimal_Checked(object sender, RoutedEventArgs e)
        {
            _drawOptimal = true;

            if (_initialised)
            {
                DrawTspLibItem();
            }
        }

        private void ShowOptimal_Unchecked(object sender, RoutedEventArgs e)
        {
            _drawOptimal = false;

            if (_initialised)
            {
                DrawTspLibItem();
            }
        }

        private void ExecuteButton_Click(object sender, RoutedEventArgs e)
        {
            _antSystem.Execute();

            _tourItems.Clear();
            var count = 1;
            foreach (var ant in _antSystem.Ants)
            {
                var nodeTour = _tspInfoProvider.BuildNode2DTourFromZeroBasedIndices(ant.Tour);
                _tourItems.Add(new ListViewTourItem(nodeTour, ant.TourLength, $"Ant {count}"));
                count++;
            }
        }

        private void TourListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_initialised)
            {
                DrawTspLibItem();
            }
        }

        #endregion eventhandlers
    }
}