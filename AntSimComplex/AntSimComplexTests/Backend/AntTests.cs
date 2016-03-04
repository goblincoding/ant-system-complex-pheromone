using AntSimComplexAlgorithms;
using AntSimComplexAlgorithms.Utilities.DataStructures;
using AntSimComplexAlgorithms.Utilities.NodeSelector;
using NSubstitute;
using NUnit.Framework;
using System.Linq;

namespace AntSimComplexTests.Backend
{
  [TestFixture]
  internal class AntTests
  {
    [Test]
    public void TourLengthShouldBeZeroAfterInitialisation()
    {
      // arrange
      const int startNode = 4;

      var data = ProblemData();
      var roulette = Substitute.For<INodeSelector>();
      var ant = new Ant(0, data, roulette);

      // act
      ant.Initialise(startNode);

      // assert
      Assert.AreEqual(0.0, ant.TourLength);
    }

    [Test]
    public void TourShouldHaveOnlyStartNodeAfterInitialisation()
    {
      // arrange
      const int startNode = 2;

      var data = ProblemData();
      var roulette = Substitute.For<INodeSelector>();
      var ant = new Ant(0, data, roulette);

      // act
      ant.Initialise(startNode);

      // assert
      Assert.AreEqual(new[] { startNode }, ant.Tour);
    }

    [Test]
    public void TourMustNotContainInvalidNodeIndicesAfterInitialisation()
    {
      // arrange
      var data = ProblemData();
      var roulette = Substitute.For<INodeSelector>();
      var ant = new Ant(0, data, roulette);

      // act
      ant.Initialise(2);

      // assert
      Assert.IsFalse(ant.Tour.Any(n => n == -1));
    }

    [Test]
    public void CurrentNodeShouldReturnCorrectIndex()
    {
      // arrange
      const int currentNode = 3;

      var data = ProblemData();
      var roulette = Substitute.For<INodeSelector>();
      var ant = new Ant(0, data, roulette);

      roulette.SelectNextNode(ant).Returns(currentNode);

      // act
      ant.Initialise(7);
      ant.Step(1);

      // assert
      Assert.AreEqual(currentNode, ant.CurrentNode);
    }

    [Test]
    public void NotVisitedShouldReturnCorrectSubsetOfAllNodes()
    {
      var notVisited = new[] { 0, 1, 2, 4, 5, 6, 9 };
      var tourSoFar = new[] { 7, 3, 8 };

      var roulette = Substitute.For<INodeSelector>();
      var data = ProblemData();
      var ant = new Ant(0, data, roulette);

      // act
      ant.Initialise(7);
      for (var i = 1; i < tourSoFar.Length; i++)
      {
        roulette.SelectNextNode(ant).Returns(tourSoFar[i]);
        ant.Step(i);
      }

      Assert.AreEqual(notVisited, ant.NotVisited);
    }

    [Test]
    public void StepShouldBuildCorrectTourLength()
    {
      // arrange
      var expectedTour = new[] { 7, 3, 8, 2, 7 };
      var roulette = Substitute.For<INodeSelector>();
      var data = ProblemData();
      var ant = new Ant(0, data, roulette);

      // act
      ant.Initialise(7);
      for (var i = 1; i < expectedTour.Length; i++)
      {
        roulette.SelectNextNode(ant).Returns(expectedTour[i]);
        ant.Step(i);
      }

      // assert
      Assert.AreEqual(8, ant.TourLength);
      Assert.AreEqual(expectedTour, ant.Tour);
    }

    private static IProblemData ProblemData()
    {
      var data = Substitute.For<IProblemData>();

      data.NodeCount.Returns(10);

      data.NearestNeighbours(7).Returns(new[] { 3, 8, 2 });
      data.Distance(7, 3).Returns(1);

      data.NearestNeighbours(3).Returns(new[] { 7, 8, 2 });
      data.Distance(3, 8).Returns(2);

      data.NearestNeighbours(8).Returns(new[] { 7, 3, 2 });
      data.Distance(8, 2).Returns(5);
      return data;
    }
  }
}