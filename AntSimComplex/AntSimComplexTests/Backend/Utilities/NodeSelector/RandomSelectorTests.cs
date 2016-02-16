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

      // act
      var result = selector.SelectNextNode(new[] { 4, 3, 1, 2 }, 5);

      // assert
      Assert.AreEqual(expected, result);
    }
  }
}