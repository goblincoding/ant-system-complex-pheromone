using AntSimComplexAlgorithms;
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
    public void SelectNextNodeShouldReturnRandomNodeIndex()
    {
      // arrange
      const int expected = 1;
      var random = Substitute.For<Random>();
      random.Next(0, 4).Returns(2);

      var selector = new RandomSelector(random);
      var ant = Substitute.For<IAnt>();
      ant.NotVisited.Returns(new[] { 4, 2, 1, 3 });

      // act
      var result = selector.SelectNextNode(ant);

      // assert
      Assert.AreEqual(expected, result);
    }
  }
}