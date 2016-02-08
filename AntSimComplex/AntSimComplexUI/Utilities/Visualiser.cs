using AntSimComplexTspLibItemManager;
using AntSimComplexTspLibItemManager.Utilities;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace AntSimComplexUI.Utilities
{
  internal class Visualiser
  {
    private readonly Canvas _canvas;
    private readonly TspLibItemManager _currentTspItemManager;
    private CoordinateTransformer _transformer;

    public Visualiser(Canvas canvas, TspLibItemManager currentTspItemManager)
    {
      _canvas = canvas;
      _currentTspItemManager = currentTspItemManager;

      InitialiseTransformer();
    }

    /// <summary>
    /// Draw the tour item and, optionally, the optimal tour.
    /// </summary>
    /// <param name="tourItem">The tour item to draw.</param>
    /// <param name="drawOptimal">Draw the optimal tour as well (if available).</param>
    /// <param name="canvasSizeChanged">True if the GUI window's canvas size has changed.</param>
    public void DrawTspLibItem(ListViewTourItem tourItem, bool drawOptimal, bool canvasSizeChanged)
    {
      if (canvasSizeChanged)
      {
        InitialiseTransformer();
      }

      _canvas.Children.Clear();

      var points = _currentTspItemManager.NodeCoordinatesAsPoints;
      foreach (var point in points)
      {
        DrawNode(point, Brushes.Black);
      }

      DrawSelectedTour(tourItem);

      if (drawOptimal)
      {
        DrawOptimalTour();
      }
    }

    /// <summary>
    /// Draws the optimal tour for the selected problem (if it is known).
    /// </summary>
    private void DrawOptimalTour()
    {
      var nodes = _currentTspItemManager.OptimalTour;
      if (!nodes.Any())
      {
        return;
      }

      var optimalLength = _currentTspItemManager.OptimalTourLength;
      DrawTour(nodes, optimalLength, Brushes.Red, Brushes.Green);
    }

    /// <summary>
    /// Draws the tour currently selected in the ListView.
    /// </summary>
    private void DrawSelectedTour(ListViewTourItem tourItem)
    {
      if (tourItem != null)
      {
        DrawTour(tourItem.Nodes, tourItem.Length, Brushes.CornflowerBlue, Brushes.DodgerBlue);
      }
    }

    private void DrawTour(IEnumerable<TspNode> nodes, double tourLength, Brush startNodeBrush, Brush lineBrush)
    {
      var points = (from n in nodes
                    select _transformer.TransformWorldToCanvas(new Point { X = n.X, Y = n.Y })).ToList();

      // Draw the starting node in red for easier identification.
      DrawNode(_transformer.TransformCanvasToWorld(points.First()), startNodeBrush);

      // Return to starting point.
      points.Add(points.First());

      var poly = new Polyline
      {
        Points = new PointCollection(points),
        Stroke = lineBrush,
        StrokeThickness = 1,
        ToolTip = $"Tour length: {tourLength}"
      };

      _canvas.Children.Add(poly);
    }

    /// <summary>
    /// Draws a single problem node at its relative position within the world.
    /// </summary>
    /// <param name="point"></param>
    /// <param name="brush"></param>
    private void DrawNode(Point point, Brush brush)
    {
      var circleWidth = Properties.Settings.Default.NodeCircleWidth;
      var ellipse = new Ellipse
      {
        Width = circleWidth,
        Height = circleWidth,
        Fill = brush,
        ToolTip = $"x: {point.X}, y: {point.Y}"
      };
      _canvas.Children.Add(ellipse);
      var transformed = _transformer.TransformWorldToCanvas(point);
      Canvas.SetLeft(ellipse, transformed.X - ellipse.Width / 2);
      Canvas.SetTop(ellipse, transformed.Y - ellipse.Height / 2);
    }

    private void InitialiseTransformer()
    {
      var worldMinX = _currentTspItemManager.MinXCoordinate;
      var worldMinY = _currentTspItemManager.MinYCoordinate;
      var worldMaxX = _currentTspItemManager.MaxXCoordinate;
      var worldMaxY = _currentTspItemManager.MaxYCoordinate;

      const double margin = 20;
      const double canvasMinX = margin;
      const double canvasMinY = margin;
      var canvasMaxX = _canvas.ActualWidth - margin;
      var canvasMaxY = _canvas.ActualHeight - margin;

      // Order of canvas Y min and max arguments are swapped due to canvas coordinate
      // system (top-left is 0,0).  This "flips" the coordinate system along the Y
      // axis by making the Y scale value negative so that we have bottom-left at 0,0.
      _transformer = new CoordinateTransformer(worldMinX, worldMaxX, worldMinY, worldMaxY,
        canvasMinX, canvasMaxX, canvasMaxY, canvasMinY);
    }
  }
}