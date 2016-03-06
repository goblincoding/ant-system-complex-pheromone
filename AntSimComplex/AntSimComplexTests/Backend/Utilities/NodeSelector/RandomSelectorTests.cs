using AntSimComplexAlgorithms.Ants;
using AntSimComplexAlgorithms.Utilities.NodeSelector;
using NSubstitute;
using NUnit.Framework;
using System;

namespace AntSimComplexTests.Backend.Utilities.NodeSelector
{
  [TestFixture]
  internal class RandomSelectorTests
  {
    [Test]
    public void SelectNextNodeShouldReturnRandomUnvisitedNodeIndex()
    {
      // arrange
      const int expected = 3;

      var ant = Substitute.For<IAnt>();
      ant.Visited.Returns(new[] { false, true, false, false });

      var random = Substitute.For<Random>();
      random.Next(0, 3).Returns(2);

      var selector = new RandomSelector(random);

      // act
      var result = selector.SelectNextNode(ant);

      // assert
      Assert.AreEqual(expected, result);
    }
  }
}