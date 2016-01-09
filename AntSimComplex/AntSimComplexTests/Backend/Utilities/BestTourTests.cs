using AntSimComplexAlgorithms.Utilities;
using NUnit.Framework;

namespace AntSimComplexTests.Backend.Utilities
{
  [TestFixture]
  public class BestTourTests
  {
    [TestCase(5.0, 4.0, 1)]
    [TestCase(2, 4.0, -1)]
    [TestCase(1.0, 1.0, 0)]
    public void CompareToShouldCompareOnTourLength(double tourLength1, double tourLength2, int compareResult)
    {
      // arrange
      var tour1 = new BestTour { TourLength = tourLength1, Tour = new[] { 1, 2, 3, 4 } };
      var tour2 = new BestTour { TourLength = tourLength2, Tour = new[] { 1, 2, 3, 4 } };

      // act
      var result = tour1.CompareTo(tour2);

      // assert
      Assert.AreEqual(compareResult, result);
    }
  }
}