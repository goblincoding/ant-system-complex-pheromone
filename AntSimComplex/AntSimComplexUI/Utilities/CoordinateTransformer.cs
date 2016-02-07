using System;
using System.Windows;
using System.Windows.Media;

namespace AntSimComplexUI.Utilities
{
  /// <summary>
  /// Utility class to transform from world to canvas coordinates and back.
  /// </summary>
  internal class CoordinateTransformer
  {
    private Matrix _worldToCanvasMatrix;
    private Matrix _canvasToWorldMatrix;

    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="worldMinX">The mininum x coordinate of the "world"</param>
    /// <param name="worldMaxX">The maximum x coordinate of the "world"</param>
    /// <param name="worldMinY">The mininum y coordinate of the "world"</param>
    /// <param name="worldMaxY">The maximum y coordinate of the "world"</param>
    /// <param name="canvasMinX">The mininum x coordinate of the canvas</param>
    /// <param name="canvasMaxX">The maximum x coordinate of the canvas</param>
    /// <param name="canvasMinY">The mininum y coordinate of the canvas</param>
    /// <param name="canvasMaxY">The maximum y coordinate of the canvas</param>
    public CoordinateTransformer(double worldMinX, double worldMaxX, double worldMinY, double worldMaxY,
                                 double canvasMinX, double canvasMaxX, double canvasMinY, double canvasMaxY)
    {
      PrepareTransformationMatrices(worldMinX, worldMaxX, worldMinY, worldMaxY,
                                    canvasMinX, canvasMaxX, canvasMinY, canvasMaxY);
    }

    public Point TransformWorldToCanvas(Point point)
    {
      return _worldToCanvasMatrix.Transform(point);
    }

    public Point TransformCanvasToWorld(Point point)
    {
      return _canvasToWorldMatrix.Transform(point);
    }

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
  }
}