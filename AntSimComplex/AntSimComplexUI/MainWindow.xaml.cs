using AntSimComplexAlgorithms;
using AntSimComplexAlgorithms.Utilities;
using AntSimComplexUI.Dialogs;
using AntSimComplexUI.Utilities;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Timers;
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
  public partial class MainWindow
  {
    private const int CircleWidth = 6;

    private const string PackageRelPath = @"..\..\..\packages";
    private const string LibPathRegistryKey = @"HKEY_CURRENT_USER\Software\AntSim\TSPLIB95Path";

    private bool _initialised;
    private bool _drawOptimal;
    private static bool _timerRunning;

    private Matrix _worldToCanvasMatrix;
    private Matrix _canvasToWorldMatrix;

    private readonly ObservableCollection<ListViewTourItem> _tourItems;

    private SymmetricTspItemLoader _tspItemLoader;
    private SymmetricTspItemInfoProvider _currentTspItemInfoProvider;
    private AntSystem _antSystem;

    public MainWindow()
    {
      InitializeComponent();
      _tourItems = new ObservableCollection<ListViewTourItem>();

      // Automatically sort tour items by length when adding to the ListView
      TourListView.Items.SortDescriptions.Add(new SortDescription("Length", ListSortDirection.Ascending));
      TourListView.ItemsSource = _tourItems;
    }

    /// <summary>
    /// Called as soon as the main window is loaded.
    /// </summary>
    private void Initialise()
    {
      var tspLibPath = BrowseTsplibPath();

      // Load all symmetric TSP instances.
      _tspItemLoader = new SymmetricTspItemLoader(tspLibPath);
      TspCombo.ItemsSource = _tspItemLoader.ProblemNames;
    }

    /// <summary>
    /// Runs the algorithm against the selected problem and for the chosen number of iterations
    /// or seconds.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ExecuteButtonClick(object sender, RoutedEventArgs e)
    {
      // Clear previous results.
      _antSystem.Reset();

      // Fire up a background worker so that we may execute the AS algorithm
      // while blocking the UI from receiving any user input.
      var worker = new BackgroundWorker();

      // This method gets called as soon as work has completed. UI interaction is re-enabled.
      worker.RunWorkerCompleted += OnExecutionBackgroundWorkerCompleted;

      // Are we executing by nr of iterations or time?
      if (RunCount.IsChecked == true)
      {
        var runCount = RunCountInt.Value;

        // No direct interaction with the UI is allowed from this method.
        worker.DoWork += (o, ea) =>
        {
          for (var i = 0; i < runCount; i++)
          {
            _antSystem.Execute();
          }
        };
      }
      else
      {
        // ReSharper disable once PossibleInvalidOperationException
        // Will never be NULL (*1000 for ms)
        var timeOut = (double)RunTimeInt.Value * 1000;
        _timerRunning = true;

        // No direct interaction with the UI is allowed from this method.
        worker.DoWork += (o, ea) =>
        {
          using (var timer = new Timer(timeOut))
          {
            timer.Elapsed += OnTimerElapsed;
            timer.Start();
            while (_timerRunning)
            {
              _antSystem.Execute();
            }
          }
        };
      }

      // Set IsBusy before we start the thread.
      BusyIndicator.IsBusy = true;
      worker.RunWorkerAsync();
    }

    /// <summary>
    /// Browse to the TSPLIB95 directory.
    /// </summary>
    /// <returns></returns>
    private string BrowseTsplibPath()
    {
      var tspLibPath = (string)Registry.GetValue(LibPathRegistryKey, "", "");
      var startDirectory = System.IO.Path.GetFullPath(PackageRelPath);
      bool tspPathExists;

      do
      {
        var dialog = new DirectoryBrowserDialog(tspLibPath, startDirectory) { Owner = this };
        dialog.ShowDialog();

        tspLibPath = dialog.DirectoryPath;
        tspPathExists = Directory.Exists(tspLibPath);

        if (!tspPathExists)
        {
          MessageBox.Show(this, "Path to TSPLIB95 is invalid.", "Error!");
        }
      } while (string.IsNullOrWhiteSpace(tspLibPath) || !tspPathExists);

      Registry.SetValue(LibPathRegistryKey, "", tspLibPath);
      return tspLibPath;
    }

    private void AddOptimalTourToListView()
    {
      if (_currentTspItemInfoProvider.HasOptimalTour)
      {
        _tourItems.Add(new ListViewTourItem(_currentTspItemInfoProvider.OptimalTour,
                                            _currentTspItemInfoProvider.OptimalTourLength,
                                            $"Current Problem (Optimal): {_currentTspItemInfoProvider.ProblemName}"));
      }
    }

    #region drawing

    /// <summary>
    /// Draw everything we know about the selected TSP problem item.
    /// </summary>
    private void DrawTspLibItem()
    {
      var worldMinX = _currentTspItemInfoProvider.GetMinX();
      var worldMinY = _currentTspItemInfoProvider.GetMinY();
      var worldMaxX = _currentTspItemInfoProvider.GetMaxX();
      var worldMaxY = _currentTspItemInfoProvider.GetMaxY();

      const double margin = 20;
      const double canvasMinX = margin;
      const double canvasMinY = margin;
      var canvasMaxX = Canvas.ActualWidth - margin;
      var canvasMaxY = Canvas.ActualHeight - margin;

      // Order of canvas Y min and max arguments are swapped due to canvas coordinate
      // system (top-left is 0,0).  This "flips" the coordinate system along the Y
      // axis by making the Y scale value negative so that we have bottom-left at 0,0.
      PrepareTransformationMatrices(worldMinX, worldMaxX, worldMinY, worldMaxY,
                                    canvasMinX, canvasMaxX, canvasMaxY, canvasMinY);

      Canvas.Children.Clear();

      var points = _currentTspItemInfoProvider.GetPoints();
      foreach (var point in points)
      {
        DrawNode(point, Brushes.Black);
      }

      DrawSelectedTour();

      if (_drawOptimal)
      {
        DrawOptimalTour();
      }
    }

    /// <summary>
    /// Draws the optimal tour for the selected problem (if it is known).
    /// </summary>
    private void DrawOptimalTour()
    {
      var nodes = _currentTspItemInfoProvider.OptimalTour;
      if (!nodes.Any())
      {
        return;
      }

      var optimalLength = _currentTspItemInfoProvider.OptimalTourLength;
      DrawTour(nodes, optimalLength, Brushes.Red, Brushes.Green);
    }

    /// <summary>
    /// Draws the tour currently selected in the ListView.
    /// </summary>
    private void DrawSelectedTour()
    {
      var item = TourListView.SelectedItem as ListViewTourItem;
      if (item != null)
      {
        DrawTour(item.Nodes, item.Length, Brushes.CornflowerBlue, Brushes.DodgerBlue);
      }
    }

    private void DrawTour(IEnumerable<Node2D> nodes, double tourLength, Brush startNodeBrush, Brush lineBrush)
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

      Canvas.Children.Add(poly);
    }

    /// <summary>
    /// Draws a single problem node at its relative position within the world.
    /// </summary>
    /// <param name="point"></param>
    /// <param name="brush"></param>
    private void DrawNode(Point point, Brush brush)
    {
      var ellipse = new Ellipse
      {
        Width = CircleWidth,
        Height = CircleWidth,
        Fill = brush,
        ToolTip = $"x: {point.X}, y: {point.Y}"
      };
      Canvas.Children.Add(ellipse);
      var transformed = TransformWorldToCanvas(point);
      Canvas.SetLeft(ellipse, transformed.X - ellipse.Width / 2);
      Canvas.SetTop(ellipse, transformed.Y - ellipse.Height / 2);
    }

    #endregion drawing

    #region matrixtransformations

    /// <summary>
    /// Sets up the transformation matrices so that we can transform from world to canvas coordinates and back.
    /// </summary>
    /// <param name="worldMinX">The mininum x coordinate of the "world"</param>
    /// <param name="worldMaxX">The maximum x coordinate of the "world"</param>
    /// <param name="worldMinY">The mininum y coordinate of the "world"</param>
    /// <param name="worldMaxY">The maximum y coordinate of the "world"</param>
    /// <param name="canvasMinX">The mininum x coordinate of the canvas</param>
    /// <param name="canvasMaxX">The maximum x coordinate of the canvas</param>
    /// <param name="canvasMinY">The mininum y coordinate of the canvas</param>
    /// <param name="canvasMaxY">The maximum y coordinate of the canvas</param>
    private void PrepareTransformationMatrices(double worldMinX, double worldMaxX, double worldMinY, double worldMaxY,
                                               double canvasMinX, double canvasMaxX, double canvasMinY, double canvasMaxY)
    {
      _worldToCanvasMatrix = Matrix.Identity;
      _worldToCanvasMatrix.Translate(-worldMinX, -worldMinY);

      var canvasXRange = Math.Abs(canvasMaxX - canvasMinX);
      var canvasYRange = Math.Abs(canvasMaxY - canvasMinY);
      var canvasXDiff = 0.0;
      var canvasYDiff = 0.0;

      double canvasRange;
      if (canvasYRange > canvasXRange)
      {
        canvasRange = canvasXRange;
        canvasYDiff = (canvasYRange - canvasXRange) * 0.5;
      }
      else
      {
        canvasRange = canvasYRange;
        canvasXDiff = (canvasXRange - canvasYRange) * 0.5;
      }

      var xscale = canvasRange / (worldMaxX - worldMinX);
      var yscale = -canvasRange / (worldMaxY - worldMinY);
      _worldToCanvasMatrix.Scale(xscale, yscale);

      _worldToCanvasMatrix.Translate(canvasMinX + canvasXDiff, canvasMinY - canvasYDiff);

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

    #endregion matrixtransformations

    #region eventhandlers

    /// <summary>
    /// Loads the TspLib95 item for the selected problem and instantiates a corresponding
    /// AntSystem object for it.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void TspComboSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
      var problemName = TspCombo.SelectedItem?.ToString();
      var item = _tspItemLoader.GetItem(problemName);
      _currentTspItemInfoProvider = new SymmetricTspItemInfoProvider(item);
      DrawTspLibItem();

      _antSystem = new AntSystem(item.Problem);
      _tourItems.Clear();
      AddOptimalTourToListView();
    }

    /// <summary>
    /// Called once the AS algorithm has finished executing. Loads the best toure of each iteration into the ListView.
    /// </summary>
    /// <param name="o"></param>
    /// <param name="ea"></param>
    private void OnExecutionBackgroundWorkerCompleted(object o, RunWorkerCompletedEventArgs ea)
    {
      BusyIndicator.IsBusy = false;

      _tourItems.Clear();
      AddOptimalTourToListView();

      var count = 1;
      foreach (var tour in _antSystem.BestTours)
      {
        var nodeTour = _currentTspItemInfoProvider.BuildNode2DTourFromZeroBasedIndices(tour.Tour);
        _tourItems.Add(new ListViewTourItem(nodeTour, tour.TourLength, $"Best Tour for Iteration {count}"));
        count++;
      }
    }

    private static void OnTimerElapsed(object sender, ElapsedEventArgs e)
    {
      _timerRunning = false;
    }

    private void WindowLoaded(object sender, RoutedEventArgs e)
    {
      Initialise();
      _initialised = true;
    }

    private void WindowSizeChanged(object sender, SizeChangedEventArgs e)
    {
      if (_initialised)
      {
        DrawTspLibItem();
      }
    }

    private void ShowOptimalChecked(object sender, RoutedEventArgs e)
    {
      _drawOptimal = true;

      if (_initialised)
      {
        DrawTspLibItem();
      }
    }

    private void ShowOptimalUnchecked(object sender, RoutedEventArgs e)
    {
      _drawOptimal = false;

      if (_initialised)
      {
        DrawTspLibItem();
      }
    }

    private void TourListViewSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
      if (_initialised)
      {
        DrawTspLibItem();
      }
    }

    private void AlphaIntValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
    {
      Parameters.Alpha = (int)e.NewValue;
    }

    private void BetaIntValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
    {
      Parameters.Beta = (int)e.NewValue;
    }

    private void EvapDoubleValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
    {
      Parameters.EvaporationRate = (double)e.NewValue;
    }

    #endregion eventhandlers
  }
}