using AntSimComplexAlgorithms;
using AntSimComplexAlgorithms.Utilities.DataStructures;
using AntSimComplexAlgorithms.Utilities.NodeSelector;
using NSubstitute;
using NUnit.Framework;

namespace AntSimComplexTests.Backend.Utilities.NodeSelector
{
  [TestFixture]
  public class NearestNeighbourSelectorTests
  {
    [Test]
    public void SelectNextNodeShouldReturnNearestUnvisitedNeighbourIndex()
    {
      // arrange
      const int expected = 3;
      const int currentNode = 5;

      var neighbours = new[] { 4, 3, 1, 2, 7, 8 };
      var visited = new[] { false, true, false, true, true, false };
      var problemData = Substitute.For<IProblemData>();
      problemData.NearestNeighbours(currentNode).Returns(neighbours);

      var selector = new NearestNeighbourSelector(problemData);

      var ant = Substitute.For<IAnt>();
      ant.CurrentNode.Returns(currentNode);
      ant.Visited.Returns(visited);

      // act
      var result = selector.SelectNextNode(ant);

      // assert
      Assert.AreEqual(expected, result);
    }
  }
}