using AntSimComplexUI.Utilities;
using NUnit.Framework;
using System;
using System.Windows;

namespace AntSimComplexTests.GUI
{
  [TestFixture]
  internal class CoordinateTransformerTests
  {
    [Test]
    public void TransformWorldToCanvasShouldReturnCorrectCanvasCoordinates()
    {
      // arrange
      var worldPoint = new Point(6734, 1453);
      var expectedCanvasPoint = new Point(684.322142672859, 501.698492462311);
      var transformer = CoordinateTransformer();

      // act
      var result = transformer.TransformWorldToCanvas(worldPoint);

      // assert
      Assert.AreEqual(Math.Round(expectedCanvasPoint.X, 10), Math.Round(result.X, 10));
      Assert.AreEqual(Math.Round(expectedCanvasPoint.Y, 10), Math.Round(result.Y, 10));
    }

    [Test]
    public void TransformCanvasToWorldShouldReturnCorrectWorldCoordinates()
    {
      // arrange
      var canvasPoint = new Point(684.322142672859, 501.698492462311);
      var expectedWorldPoint = new Point(6734, 1453);
      var transformer = CoordinateTransformer();

      // act
      var result = transformer.TransformCanvasToWorld(canvasPoint);

      // assert
      Assert.AreEqual(Math.Round(expectedWorldPoint.X, 10), Math.Round(result.X, 10));
      Assert.AreEqual(Math.Round(expectedWorldPoint.Y, 10), Math.Round(result.Y, 10));
    }

    [Test]
    public void TransformCanvasToWorldShouldMirrorTransformWorldToCanvas()
    {
      // arrange
      var canvasPoint = new Point(684.322142672859, 501.698492462311);
      var transformer = CoordinateTransformer();

      // act
      var worldPoint = transformer.TransformCanvasToWorld(canvasPoint);
      var result = transformer.TransformWorldToCanvas(worldPoint);

      // assert
      Assert.AreEqual(Math.Round(canvasPoint.X, 10), Math.Round(result.X, 10));
      Assert.AreEqual(Math.Round(canvasPoint.Y, 10), Math.Round(result.Y, 10));
    }

    [Test]
    public void TransformWorldToCanvasShouldMirrorTransformCanvasToWorld()
    {
      // arrange
      var worldPoint = new Point(6734, 1453);

      var transformer = CoordinateTransformer();

      // act
      var canvasPoint = transformer.TransformWorldToCanvas(worldPoint);
      var result = transformer.TransformCanvasToWorld(canvasPoint);

      // assert
      Assert.AreEqual(Math.Round(worldPoint.X, 10), Math.Round(result.X, 10));
      Assert.AreEqual(Math.Round(worldPoint.Y, 10), Math.Round(result.Y, 10));
    }

    private static CoordinateTransformer CoordinateTransformer()
    {
      const double canvasMaxX = 857.8125;
      const double canvasMaxY = 20;
      const double canvasMinX = 20;
      const double canvasMinY = 688;

      const double worldMaxX = 7762;
      const double worldMaxY = 5184;
      const double worldMinX = 10;
      const double worldMinY = 10;

      var transformer = new CoordinateTransformer(worldMinX, worldMaxX, worldMinY, worldMaxY,
                                                  canvasMinX, canvasMaxX, canvasMinY, canvasMaxY);
      return transformer;
    }
  }
}