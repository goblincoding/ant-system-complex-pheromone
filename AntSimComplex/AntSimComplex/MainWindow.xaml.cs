using AntSimComplexAS;
using AntSimComplexUI.Dialogs;
using AntSimComplexUI.Utilities;
using Microsoft.Win32;
using System;
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

        private SymmetricTSPItemSelector _tspProblemSelector;
        private SymmetricTSPInfoProvider _tspInfoProvider;
        private AntSystem _antSystem;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void TSPCombo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var problemName = TSPCombo.SelectedItem?.ToString();
            var item = _tspProblemSelector.GetItem(problemName);
            _tspInfoProvider = new SymmetricTSPInfoProvider(item);
            DrawTspLibItem();

            _antSystem = new AntSystem(item.Problem);
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

            if (_drawOptimal)
            {
                DrawOptimalTour();
            }
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
            var ellipse = new Ellipse() { Width = _circleWidth, Height = _circleWidth, Fill = brush };
            ellipse.ToolTip = $"x: {point.X}, y: {point.Y}";
            canvas.Children.Add(ellipse);
            var transformed = TransformWorldToCanvas(point);
            Canvas.SetLeft(ellipse, transformed.X - ellipse.Width / 2);
            Canvas.SetTop(ellipse, transformed.Y - ellipse.Height / 2);
        }

        private void DrawOptimalTour()
        {
            var nodes = _tspInfoProvider.OptimalTourNodes2D;
            if (!nodes.Any())
            {
                return;
            }

            var optimalLength = _tspInfoProvider.OptimalTourLength;
            var points = (from n in nodes
                          select TransformWorldToCanvas(new Point { X = n.X, Y = n.Y })).ToList();

            // Draw the starting node in red for easier identification.
            DrawNode(TransformCanvasToWorld(points.First()), Brushes.Red);

            // Return to starting point.
            points.Add(points.First());

            var poly = new Polyline()
            {
                Points = new PointCollection(points),
                Stroke = Brushes.Green,
                StrokeThickness = 1,
                ToolTip = $"Optimal tour: {optimalLength}"
            };

            canvas.Children.Add(poly);
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