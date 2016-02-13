using AntSimComplexAlgorithms;
using AntSimComplexAlgorithms.Utilities.DataStructures;
using AntSimComplexAlgorithms.Utilities.RouletteWheelSelector;
using NSubstitute;
using NUnit.Framework;
using System.Collections.Generic;

namespace AntSimComplexTests.Backend
{
  [TestFixture]
  internal class AntTests
  {
    [Test]
    public void TourLengthShouldBeZeroAfterInitialisation()
    {
      // arrange
      const int startNode = 5;

      var data = Substitute.For<IDataStructures>();
      data.NodeCount.Returns(10);
      var roulette = Substitute.For<IRouletteWheelSelector>();
      var ant = new Ant(data, roulette);

      // act
      ant.Initialise(startNode);

      // assert
      Assert.AreEqual(0.0, ant.TourLength);
    }

    [Test]
    public void TourShouldHaveOnlyStartNodeAfterInitialisation()
    {
      // arrange
      const int startNode = 5;

      var data = Substitute.For<IDataStructures>();
      data.NodeCount.Returns(10);
      var roulette = Substitute.For<IRouletteWheelSelector>();
      var ant = new Ant(data, roulette);

      // act
      ant.Initialise(startNode);

      // assert
      Assert.AreEqual(new List<int> { startNode }, ant.Tour);
    }

    [Test]
    public void MoveNextShouldBuildAccurateTourLength()
    {
      // arrange
      var data = Substitute.For<IDataStructures>();
      var roulette = Substitute.For<IRouletteWheelSelector>();

      data.NodeCount.Returns(10);

      data.NearestNeighbours(7).Returns(new[] { 3, 8, 2 });
      roulette.SelectNextNode(Arg.Any<int[]>(), 7).Returns(3);
      data.Distance(7, 3).Returns(1);

      data.NearestNeighbours(3).Returns(new[] { 7, 8, 2 });
      roulette.SelectNextNode(Arg.Any<int[]>(), 3).Returns(8);
      data.Distance(3, 8).Returns(2);

      data.NearestNeighbours(8).Returns(new[] { 7, 3, 2 });
      roulette.SelectNextNode(Arg.Any<int[]>(), 8).Returns(2);
      data.Distance(8, 2).Returns(5);

      var ant = new Ant(data, roulette);
      var expectedTour = new List<int> { 7, 3, 8, 2, 7 };

      // act
      ant.Initialise(7);
      ant.MoveNext();
      ant.MoveNext();
      ant.MoveNext();
      ant.MoveNext();

      // assert
      Assert.AreEqual(8, ant.TourLength);
      Assert.AreEqual(expectedTour, ant.Tour);
    }
  }
}