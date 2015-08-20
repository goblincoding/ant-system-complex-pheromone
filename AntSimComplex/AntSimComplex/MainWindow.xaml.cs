using AntSimComplex.Dialogs;
using AntSimComplex.Utilities;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
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

        private bool _initialised;
        private bool _drawOptimal;

        private Matrix _worldToCanvasMatrix;
        private Matrix _canvasToWorldMatrix;

        private SymmetricTSPInfoProvider _tspLibProcessor;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Initialise();
            _initialised = true;
        }

        private void Initialise()
        {
            var tspLibPath = BrowseTSPLIBPath();

            // Load all symmetric TSP instances.
            _tspLibProcessor = new SymmetricTSPInfoProvider(tspLibPath);
            TSPCombo.ItemsSource = _tspLibProcessor.ProblemNames;
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
            var problemName = TSPCombo.SelectedItem?.ToString();
            var worldMinX = _tspLibProcessor.GetMinX(problemName);
            var worldMinY = _tspLibProcessor.GetMinY(problemName);
            var worldMaxX = _tspLibProcessor.GetMaxX(problemName);
            var worldMaxY = _tspLibProcessor.GetMaxY(problemName);

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
            DrawNodes(problemName);

            if (_drawOptimal)
            {
                DrawOptimalTour(problemName);
            }
        }

        private void DrawNodes(string problemName)
        {
            var nodes = _tspLibProcessor.GetGraphNodes(problemName);
            var points = from n in nodes
                         select new Point { X = n.X, Y = n.Y };

            foreach (var point in points)
            {
                var ellipse = new Ellipse() { Width = _circleWidth, Height = _circleWidth, Fill = Brushes.Black };
                ellipse.ToolTip = $"x: {point.X}, y: {point.Y}";
                canvas.Children.Add(ellipse);
                var transformed = TransformWorldToCanvas(point);
                Canvas.SetLeft(ellipse, transformed.X - ellipse.Width / 2);
                Canvas.SetTop(ellipse, transformed.Y - ellipse.Height / 2);
            }
        }

        private void DrawOptimalTour(string problemName)
        {
            var nodes = _tspLibProcessor.GetOptimalTourNodes(problemName);
            var optimalLength = _tspLibProcessor.GetOptimalTourLength(problemName);
            DrawTour(nodes, $"Optimal tour: {optimalLength}");
        }

        private void DrawTour(List<Node2D> nodes, string toolTip)
        {
            var points = from n in nodes
                         select new Point { X = n.X, Y = n.Y };

            for (var i = 0; i < points.Count() - 1; ++i)
            {
                var point1 = TransformWorldToCanvas(points.ElementAt(i));
                var point2 = TransformWorldToCanvas(points.ElementAt(i + 1));
                var line = new Line()
                {
                    X1 = point1.X,
                    Y1 = point1.Y,
                    X2 = point2.X,
                    Y2 = point2.Y,
                    StrokeThickness = 1,
                    Stroke = Brushes.Green,
                    ToolTip = toolTip
                };

                canvas.Children.Add(line);
            }
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

        private void TSPCombo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            DrawTspLibItem();
            var problemName = TSPCombo.SelectedItem?.ToString();
            var param = _tspLibProcessor.GetProblemParameters(problemName);
            DrawTour(param.NearestNeighbourTour, "");
            Console.WriteLine(param.InitialPheromone);
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
    }
}