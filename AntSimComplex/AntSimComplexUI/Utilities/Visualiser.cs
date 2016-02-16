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
    /// <param name="canvasSizeChanged">True if the window has been resized.</param>
    public void DrawTspLibItem(ListViewTourItem tourItem, bool drawOptimal, bool canvasSizeChanged)
    {
      if (canvasSizeChanged)
      {
        InitialiseTransformer();
      }

      _canvas.Children.Clear();

      foreach (var node in _currentTspItemManager.TspNodes)
      {
        DrawNode(node.Id, new Point { X = node.X, Y = node.Y }, Brushes.Black, false);
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
        DrawTour(tourItem.Nodes, tourItem.Length, Brushes.OrangeRed, Brushes.DodgerBlue);
      }
    }

    private void DrawTour(IEnumerable<TspNode> nodes, double tourLength, Brush startNodeBrush, Brush lineBrush)
    {
      var tspNodes = nodes as IList<TspNode> ?? nodes.ToList();
      var points = tspNodes.Select(n => _transformer.TransformWorldToCanvas(new Point { X = n.X, Y = n.Y })).ToList();

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

      // Draw the starting node over the poly lines.
      DrawNode(tspNodes.First().Id, _transformer.TransformCanvasToWorld(points.First()), startNodeBrush, true);
    }

    /// <summary>
    /// Draws a single problem node at its relative position within the world.
    /// </summary>
    /// <param name="id">The ID of the node being drawn.</param>
    /// <param name="point">The point at which the node is drawn.</param>
    /// <param name="brush">The colour of the node.</param>
    /// <param name="startNode">True if the node to be drawn is the start node.</param>
    private void DrawNode(int id, Point point, Brush brush, bool startNode)
    {
      // Draw the start node slightly larger for easier visual identification.
      var circleWidth = Properties.Settings.Default.NodeCircleWidth;
      circleWidth = startNode ? circleWidth * 2 : circleWidth;

      var ellipse = new Ellipse
      {
        Width = circleWidth,
        Height = circleWidth,
        Fill = brush,
        ToolTip = $"Node {id}: ({point.X}, {point.Y})"
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