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
    public void SelectNextNodeShouldReturnNearestNeighbourIndex()
    {
      // arrange
      const int expected = 4;
      var problemData = Substitute.For<IProblemData>();
      problemData.NearestNeighbours(5).Returns(new[] { 4, 3, 1, 2, 7, 8 });
      var selector = new NearestNeighbourSelector(problemData);

      // act
      var result = selector.SelectNextNode(new[] { 8, 3, 4, 2 }, 5);

      // assert
      Assert.AreEqual(expected, result);
    }
  }
}