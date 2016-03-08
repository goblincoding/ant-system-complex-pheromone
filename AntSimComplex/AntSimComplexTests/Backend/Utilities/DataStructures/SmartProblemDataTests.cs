using AntSimComplexAlgorithms.Ants;
using AntSimComplexAlgorithms.Utilities;
using AntSimComplexAlgorithms.Utilities.DataStructures;
using NSubstitute;
using NUnit.Framework;
using System;
using System.Collections.Generic;

namespace AntSimComplexTests.Backend.Utilities.DataStructures
{
  [TestFixture]
  internal class SmartProblemDataTests
  {
    private readonly Random _random = new Random();
    private const double InitialPheromoneDensity = 0.5;
    private const int NodeCount = 10;

    [Test]
    public void CtorGivenNegativeInitialPheromoneShouldThrowArgumentOutOfRangeException()
    {
      // arrange
      var problem = new MockProblem();
      var distances = problem.Distances;

      // assert
      // ReSharper disable once ObjectCreationAsStatement
      Assert.Throws<ArgumentOutOfRangeException>(() => new SmartProblemData(problem.NodeProvider.CountNodes(), -1, distances));
    }

    [Test]
    public void DistanceIndexInvalidShouldThrowIndexOutOfRangeException()
    {
      // arrange
      var data = CreateSmartProblemDataFromMockProblem();

      // assert
      Assert.Throws<IndexOutOfRangeException>(() => data.Distance(0, MockConstants.NrNodes));
    }

    [TestCase(1, 2, 3)]
    [TestCase(2, 1, 3)]
    [TestCase(4, 7, 2)]
    public void DistanceGivenValidIndexShouldReturnCorrectWeight(int node1, int node2, int expectedWeight)
    {
      // arrange
      var data = CreateSmartProblemDataFromMockProblem();

      // act
      var result = data.Distance(node1, node2);

      // assert
      Assert.AreEqual(expectedWeight, result);
    }

    [TestCase(1, 1)]
    [TestCase(5, 5)]
    [TestCase(9, 9)]
    public void DistanceGivenValidIndexShouldReturnDoubleMaxIfToSameNode(int node1, int node2)
    {
      // arrange
      var data = CreateSmartProblemDataFromMockProblem();

      // act
      var result = data.Distance(node1, node2);

      // assert
      Assert.AreEqual(double.MaxValue, result);
    }

    [Test]
    public void NearestNeighboursIndexInvalidShouldThrowIndexOutOfRangeException()
    {
      // arrange
      var data = CreateSmartProblemDataFromMockProblem();

      // assert
      Assert.Throws<IndexOutOfRangeException>(() => data.NearestNeighbours(MockConstants.NrNodes));
    }

    [TestCase(0, new[] { 1, 3, 4, 5, 6, 2, 7, 8, 9 })]
    [TestCase(1, new[] { 9, 0, 2, 3, 4, 5, 6, 7, 8 })]
    [TestCase(4, new[] { 6, 7, 8, 0, 9, 1, 2, 3, 5 })]
    public void NearestNeighboursGivenValidIndexShouldReturnCorrectResults(int index, int[] nearestNeighbours)
    {
      // arrange
      var data = CreateSmartProblemDataFromMockProblem();

      // act
      var result = data.NearestNeighbours(index);

      // assert
      Assert.AreEqual(nearestNeighbours, result);
    }

    [Test]
    public void ChoiceInfoGivenValidIndicesShouldReturnCorrectValue()
    {
      // arrange
      const int node1 = 3;
      const int node2 = 8;

      var data = CreateSmartProblemDataFromMockProblem();
      var distance = data.Distance(node1, node2);
      var heuristic = Math.Pow(1 / distance, Parameters.Beta);
      var expected = Math.Pow(InitialPheromoneDensity, Parameters.Alpha) * heuristic;

      var ant = Substitute.For<IAnt>();
      ant.Id.Returns(_random.Next(0, NodeCount));

      // act
      var choiceInfo = data.ChoiceInfo(ant);
      var result = choiceInfo[node1][node2];

      // assert
      Assert.AreEqual(expected, result);
    }

    [TestCase(1, 5, 11.9)]
    [TestCase(4, 5, 12.5)]
    [TestCase(0, 9, 0.5)]
    [TestCase(3, 7, 0.67)]
    [TestCase(1, 1, 9.0)]
    [TestCase(0, 9, 7.9)]
    public void UpdateGlobalPheromoneTrailsShouldWorkCorrectly(int node1, int node2, double tourLength)
    {
      // arrange
      var deposit = 1.0 / tourLength;

      var data = CreateSmartProblemDataFromMockProblem();
      var distance = data.Distance(node1, node2);
      var heuristic = Math.Pow(1.0 / distance, Parameters.Beta);

      var ant = Substitute.For<IAnt>();
      ant.Id.Returns(_random.Next(0, NodeCount));
      ant.CurrentNode.Returns(node1);
      ant.Tour.Returns(new List<int> { node1, node2 });
      ant.TourLength.Returns(tourLength);

      var ants = new List<IAnt> { ant };

      var evaporatedDensity = InitialPheromoneDensity * (1.0 - Parameters.EvaporationRate);
      var depositedDensity = evaporatedDensity + deposit;
      var expected = Math.Pow(depositedDensity, Parameters.Alpha) * heuristic;

      // act
      data.UpdateGlobalPheromoneTrails(ants);
      var choiceInfo = data.ChoiceInfo(ant);
      var result = choiceInfo[node1][node2];

      // assert
      Assert.AreEqual(expected, result);
    }

    [TestCase(1, 5, 11.9)]
    [TestCase(4, 5, 12.5)]
    [TestCase(0, 9, 0.5)]
    [TestCase(3, 7, 0.67)]
    [TestCase(1, 1, 9.0)]
    [TestCase(0, 9, 7.9)]
    public void UpdateLocalPheromoneTrailsShouldWorkCorrectly(int node1, int node2, double tourLength)
    {
      // arrange
      var data = CreateSmartProblemDataFromMockProblem();
      var distance = data.Distance(node1, node2);
      var heuristic = Math.Pow(1.0 / distance, Parameters.Beta);
      var touch = 1.0 / (tourLength / distance);

      var ant = Substitute.For<IAnt>();
      ant.Id.Returns(_random.Next(0, NodeCount));
      ant.CurrentNode.Returns(node1);
      ant.Tour.Returns(new List<int> { node1, node2 });
      ant.TourLength.Returns(tourLength);

      var ants = new List<IAnt> { ant };

      var touchedDensity = InitialPheromoneDensity * touch;
      var expected = Math.Pow(touchedDensity, Parameters.Alpha) * heuristic;

      // act
      data.UpdateLocalPheromoneTrails(ants);
      var choiceInfo = data.ChoiceInfo(ant);
      var result = choiceInfo[node1][node2];

      // assert
      Assert.AreEqual(expected, result);
    }

    [TestCase(1, 5, 11.9)]
    [TestCase(4, 5, 12.5)]
    [TestCase(0, 9, 0.5)]
    [TestCase(1, 1, 9.0)]
    public void ResetPheromoneShouldUpdateChoiceInfoCorrectly(int node1, int node2, double tourLength)
    {
      // arrange
      var data = CreateSmartProblemDataFromMockProblem();
      var distance = data.Distance(node1, node2);
      var heuristic = Math.Pow(1 / distance, Parameters.Beta);
      var expected = Math.Pow(InitialPheromoneDensity, Parameters.Alpha) * heuristic;

      var ant = Substitute.For<IAnt>();
      ant.Id.Returns(_random.Next(0, NodeCount));
      ant.Tour.Returns(new List<int> { node1, node2 });
      ant.TourLength.Returns(tourLength);

      var ants = new List<IAnt> { ant };

      // act
      data.UpdateGlobalPheromoneTrails(ants);
      data.ResetPheromone();

      var choiceInfo = data.ChoiceInfo(ant);
      var result = choiceInfo[node1][node2];

      // assert
      Assert.AreEqual(expected, result);
    }

    private static SmartProblemData CreateSmartProblemDataFromMockProblem()
    {
      var problem = new MockProblem();
      var distances = problem.Distances;
      return new SmartProblemData(NodeCount, InitialPheromoneDensity, distances);
    }
  }
}