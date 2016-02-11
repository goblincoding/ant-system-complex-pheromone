using AntSimComplexAlgorithms.Utilities.DataStructures;
using AntSimComplexAlgorithms.Utilities.RouletteWheelSelector;
using NSubstitute;
using NUnit.Framework;
using System;

namespace AntSimComplexTests.Backend.Utilities.RouletteWheelSelector
{
  [TestFixture]
  internal class RouletteWheelSelectorTests
  {
    [TestCase(0.00, 4)]  // probability of 5 - 4 is 0.20
    [TestCase(0.10, 4)]  // probability of 5 - 4 is 0.20
    [TestCase(0.20, 4)]  // probability of 5 - 4 is 0.20
    [TestCase(0.21, 2)]  // probability of 5 - 2 is 0.33
    [TestCase(0.32, 2)]  // probability of 5 - 2 is 0.33
    [TestCase(0.34, 1)]  // probability of 5 - 1 is 0.45
    [TestCase(0.38, 1)]  // probability of 5 - 1 is 0.45
    [TestCase(0.50, 1)]  // probability of 5 - 1 is 0.45
    [TestCase(1.00, 1)]  // probability of 5 - 1 is 0.45
    public void SelectNextNodeGivenMockProblemShouldReturnCorrectIndex(double next, int expectedIndex)
    {
      // arrange
      const int nodeCount = 10;
      const int probabilityScaleFactor = 1000000000;

      var random = Substitute.For<Random>();
      random.Next(probabilityScaleFactor).Returns((int)(next * probabilityScaleFactor));

      var problem = new MockProblem();
      var distances = problem.Distances;

      var data = new Data(nodeCount, 0.5, distances);
      var selector = new RouletteWheel(data, random);

      // act
      var result = selector.SelectNextNode(new[] { 4, 1, 2 }, 5);

      // assert
      Assert.AreEqual(expectedIndex, result);
    }
  }
}