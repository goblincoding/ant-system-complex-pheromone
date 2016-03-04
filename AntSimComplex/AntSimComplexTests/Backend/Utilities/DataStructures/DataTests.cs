using AntSimComplexAlgorithms;
using AntSimComplexAlgorithms.Utilities;
using AntSimComplexAlgorithms.Utilities.DataStructures;
using NSubstitute;
using NUnit.Framework;
using System;
using System.Collections.Generic;

namespace AntSimComplexTests.Backend.Utilities.DataStructures
{
  [TestFixture]
  internal class DataTests
  {
    private const double InitialPheromoneDensity = 0.5;

    [Test]
    public void CtorGivenNegativeInitialPheromoneShouldThrowArgumentOutOfRangeException()
    {
      // arrange
      var problem = new MockProblem();
      var distances = problem.Distances;

      // assert
      // ReSharper disable once ObjectCreationAsStatement
      Assert.Throws<ArgumentOutOfRangeException>(() => new StandardProblemData(problem.NodeProvider.CountNodes(), -1, distances));
    }

    [Test]
    public void DistanceIndexInvalidShouldThrowIndexOutOfRangeException()
    {
      // arrange
      var data = CreateDefaultDataStructuresFromMockProblem();

      // assert
      Assert.Throws<IndexOutOfRangeException>(() => data.Distance(0, MockConstants.NrNodes));
    }

    [TestCase(1, 2, 3)]
    [TestCase(2, 1, 3)]
    [TestCase(4, 7, 2)]
    public void DistanceGivenValidIndexShouldReturnMockProblemWeights(int node1, int node2, int expectedWeight)
    {
      // arrange
      var data = CreateDefaultDataStructuresFromMockProblem();

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
      var data = CreateDefaultDataStructuresFromMockProblem();

      // act
      var result = data.Distance(node1, node2);

      // assert
      Assert.AreEqual(double.MaxValue, result);
    }

    [Test]
    public void NearestNeighboursIndexInvalidShouldThrowIndexOutOfRangeException()
    {
      // arrange
      var data = CreateDefaultDataStructuresFromMockProblem();

      // assert
      Assert.Throws<IndexOutOfRangeException>(() => data.NearestNeighbours(MockConstants.NrNodes));
    }

    [TestCase(0, new[] { 1, 3, 4, 5, 6, 2, 7, 8, 9 })]
    [TestCase(1, new[] { 9, 0, 2, 3, 4, 5, 6, 7, 8 })]
    [TestCase(4, new[] { 6, 7, 8, 0, 9, 1, 2, 3, 5 })]
    public void NearestNeighboursGivenValidIndexShouldReturnCorrectMockProblemResults(int index, int[] nearestNeighbours)
    {
      // arrange
      var data = CreateDefaultDataStructuresFromMockProblem();

      // act
      var result = data.NearestNeighbours(index);

      // assert
      Assert.AreEqual(nearestNeighbours, result);
    }

    [Test]
    public void ChoiceInfoGivenValidIndicesShouldReturnCorrectMockProblemResults()
    {
      // arrange
      const int node1 = 3;
      const int node2 = 8;

      var data = CreateDefaultDataStructuresFromMockProblem();
      var distance = data.Distance(node1, node2);
      var heuristic = Math.Pow(1 / distance, Parameters.Beta);
      var expected = Math.Pow(InitialPheromoneDensity, Parameters.Alpha) * heuristic;

      var ant = Substitute.For<IAnt>();

      // act
      var choiceInfo = data.ChoiceInfo(ant);
      var result = choiceInfo[node1][node2];

      // assert
      Assert.AreEqual(expected, result);
    }

    [Test]
    public void DepositPheromoneUpdateChoiceInfoMatrixShouldUpdateCorrectly()
    {
      // arrange
      const int node1 = 4;
      const int node2 = 5;
      const double deposit = 0.1;

      var data = CreateDefaultDataStructuresFromMockProblem();
      var distance = data.Distance(node1, node2);
      var heuristic = Math.Pow(1 / distance, Parameters.Beta);
      var expected = Math.Pow(InitialPheromoneDensity * Parameters.EvaporationRate + deposit, Parameters.Alpha) * heuristic;

      var ant = Substitute.For<IAnt>();
      ant.Tour.Returns(new List<int> { node1, node2 });
      ant.TourLength.Returns(10);
      var ants = new List<IAnt> { ant };

      // act
      data.UpdatePheromoneTrails(ants);
      var choiceInfo = data.ChoiceInfo(ant);
      var result = choiceInfo[node1][node2];

      // assert
      Assert.AreEqual(expected, result);
    }

    [Test]
    public void EvaporatePheromoneUpdateChoiceInfoMatrixShouldUpdateCorrectly()
    {
      const int node1 = 1;
      const int node2 = 6;

      var data = CreateDefaultDataStructuresFromMockProblem();
      var distance = data.Distance(node1, node2);
      var heuristic = Math.Pow(1 / distance, Parameters.Beta);
      var expected = Math.Pow(InitialPheromoneDensity * Parameters.EvaporationRate, Parameters.Alpha) * heuristic;

      var ants = Substitute.For<IList<IAnt>>();
      var ant = Substitute.For<IAnt>();

      // act
      data.UpdatePheromoneTrails(ants);
      var choiceInfo = data.ChoiceInfo(ant);
      var result = choiceInfo[node1][node2];

      // assert
      Assert.AreEqual(expected, result);
    }

    [Test]
    public void ResetPheromoneUpdateChoiceInfoMatrixShouldUpdateCorrectly()
    {
      // arrange
      const int node1 = 0;
      const int node2 = 4;

      var data = CreateDefaultDataStructuresFromMockProblem();
      var distance = data.Distance(node1, node2);
      var heuristic = Math.Pow(1 / distance, Parameters.Beta);
      var expected = Math.Pow(InitialPheromoneDensity, Parameters.Alpha) * heuristic;

      var ant = Substitute.For<IAnt>();
      ant.Tour.Returns(new List<int> { node1, node2 });
      ant.TourLength.Returns(15);
      var ants = new List<IAnt> { ant };

      // act
      data.UpdatePheromoneTrails(ants);
      data.ResetPheromone();

      var choiceInfo = data.ChoiceInfo(ant);
      var result = choiceInfo[node1][node2];

      // assert
      Assert.AreEqual(expected, result);
    }

    private static StandardProblemData CreateDefaultDataStructuresFromMockProblem()
    {
      const int nodeCount = 10;

      var problem = new MockProblem();
      var distances = problem.Distances;

      var data = new StandardProblemData(nodeCount, InitialPheromoneDensity, distances);
      return data;
    }
  }
}