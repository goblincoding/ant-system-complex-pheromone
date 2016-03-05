using AntSimComplexAlgorithms;
using AntSimComplexAlgorithms.Utilities.DataStructures;
using AntSimComplexAlgorithms.Utilities.NodeSelector;
using NSubstitute;
using NUnit.Framework;
using System;

namespace AntSimComplexTests.Backend.Utilities.NodeSelector
{
  [TestFixture]
  internal class RouletteWheelSelectorTests
  {
    [TestCase(0.0, 4)]    // probability of 5 - 4 is 16.3
    [TestCase(0.105, 4)]  // probability of 5 - 4 is 16.3
    [TestCase(0.204, 1)]  // probability of 5 - 1 is 56.9 (1.6 + 56.9 = 73.2)
    [TestCase(0.331, 1)]  // probability of 5 - 1 is 56.9 (1.6 + 56.9 = 73.2)
    [TestCase(0.532, 1)]  // probability of 5 - 1 is 56.9 (1.6 + 56.9 = 73.2)
    [TestCase(0.732, 1)]  // probability of 5 - 1 is 56.9 (1.6 + 56.9 = 73.2)
    [TestCase(0.825, 2)]  // probability of 5 - 2 is 22   (1.6 + 56.9 + 22 = 95.2)
    [TestCase(0.911, 2)]  // probability of 5 - 2 is 22   (1.6 + 56.9 + 22 = 95.2)
    [TestCase(0.943, 2)]  // probability of 5 - 2 is 22   (1.6 + 56.9 + 22 = 95.2)
    [TestCase(0.953, 3)]  // probability of 5 - 3 is 4.8  (1.6 + 56.9 + 22 + 4.8 = 100)
    [TestCase(0.989, 3)]  // probability of 5 - 3 is 4.8  (1.6 + 56.9 + 22 + 4.8 = 100)
    [TestCase(1.0, 3)]    // probability of 5 - 3 is 4.8  (1.6 + 56.9 + 22 + 4.8 = 100)
    public void SelectNextNodeShouldReturnCorrectIndex(double nextRandom, int expectedIndex)
    {
      // arrange
      const int probabilityScaleFactor = 1000000000;
      const int currentNode = 5;

      var ant = Substitute.For<IAnt>();
      ant.CurrentNode.Returns(currentNode);
      ant.NotVisited.Returns(new[] { 4, 1, 2, 3 });

      var random = Substitute.For<Random>();
      random.Next(probabilityScaleFactor).Returns((int)(nextRandom * probabilityScaleFactor));

      var data = Substitute.For<IProblemData>();
      var choice = new[]
      {
        new[] { 0.0, 0.0, 0.0, 0.0, 0.0, 0.0 },
        new[] { 0.0, 0.0, 0.0, 0.0, 0.0, 0.0 },
        new[] { 0.0, 0.0, 0.0, 0.0, 0.0, 0.0 },
        new[] { 0.0, 0.0, 0.0, 0.0, 0.0, 0.0 },
        new[] { 0.0, 0.0, 0.0, 0.0, 0.0, 0.0 },
        new[] { 0.0, 56.9, 22.0, 4.8, 16.3, 0.0 }
      };

      data.ChoiceInfo(ant).Returns(choice);

      var selector = new RouletteWheelSelector(data, random);

      // act
      var result = selector.SelectNextNode(ant);

      // assert
      Assert.AreEqual(expectedIndex, result);
    }
  }
}