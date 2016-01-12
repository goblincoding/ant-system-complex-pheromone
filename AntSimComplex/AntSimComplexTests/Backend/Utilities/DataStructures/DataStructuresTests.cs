using AntSimComplexAlgorithms.Utilities;
using AntSimComplexAlgorithms.Utilities.DataStructures;
using NUnit.Framework;
using System;

namespace AntSimComplexTests.Backend.Utilities.DataStructures
{
  [TestFixture]
  internal class DataStructuresTests
  {
    private const double InitialPheromoneDensity = 0.5;

    [Test]
    public void CtorGivenNullProblemInstanceShouldThrowArgumentNullException()
    {
      // assert
      // ReSharper disable once ObjectCreationAsStatement
      Assert.Throws<ArgumentNullException>(() => { new Data(null, InitialPheromoneDensity); });
    }

    [TestCase(0.0)]
    [TestCase(-1)]
    public void CtorGivenInitialPheromoneNotGreaterThan0ShouldThrowArgumentOutOfRangeException(double initialPheromone)
    {
      // arrange
      var problem = new MockProblem();

      // assert
      // ReSharper disable once ObjectCreationAsStatement
      Assert.Throws<ArgumentOutOfRangeException>(() => new Data(problem, initialPheromone));
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
    public void ChoiceInfoGivenInvalidIndexShouldThrowIndexOutOfRangeException()
    {
      // arrange
      var data = CreateDefaultDataStructuresFromMockProblem();

      // assert
      Assert.Throws<IndexOutOfRangeException>(() => data.ChoiceInfo(0, MockConstants.NrNodes));
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

      // act
      var choiceInfo = data.ChoiceInfo(node1, node2);

      // assert
      Assert.AreEqual(expected, choiceInfo);
    }

    [Test]
    public void ChoiceInfoShouldUpdateCorrectlyAfterPheromoneDeposit()
    {
      // arrange
      const int node1 = 4;
      const int node2 = 5;
      const double deposit = 1.5;

      var data = CreateDefaultDataStructuresFromMockProblem();
      var distance = data.Distance(node1, node2);
      var heuristic = Math.Pow(1 / distance, Parameters.Beta);
      var expected = Math.Pow(InitialPheromoneDensity + deposit, Parameters.Alpha) * heuristic;

      // act
      data.DepositPheromone(new[] { node1, node2 }, deposit);
      data.UpdateChoiceInfoMatrix();
      var choiceInfo = data.ChoiceInfo(node1, node2);

      // assert
      Assert.AreEqual(expected, choiceInfo);
    }

    [Test]
    public void ChoiceInfoShouldUpdateCorrectlyAfterPheromoneEvaporation()
    {
      const int node1 = 1;
      const int node2 = 6;

      var data = CreateDefaultDataStructuresFromMockProblem();
      var distance = data.Distance(node1, node2);
      var heuristic = Math.Pow(1 / distance, Parameters.Beta);
      var expected = Math.Pow(InitialPheromoneDensity * Parameters.EvaporationRate, Parameters.Alpha) * heuristic;

      // act
      data.EvaporatePheromone();
      data.UpdateChoiceInfoMatrix();
      var choiceInfo = data.ChoiceInfo(node1, node2);

      // assert
      Assert.AreEqual(expected, choiceInfo);
    }

    [Test]
    public void ChoiceInfoShouldUpdateCorrectlyAfterPheromoneReset()
    {
      // arrange
      const int node1 = 0;
      const int node2 = 4;
      const double deposit = 1.5;

      var data = CreateDefaultDataStructuresFromMockProblem();
      var distance = data.Distance(node1, node2);
      var heuristic = Math.Pow(1 / distance, Parameters.Beta);
      var expected = Math.Pow(InitialPheromoneDensity, Parameters.Alpha) * heuristic;

      // act
      data.DepositPheromone(new[] { node1, node2 }, deposit);
      data.ResetPheromone();
      data.UpdateChoiceInfoMatrix();
      var choiceInfo = data.ChoiceInfo(node1, node2);

      // assert
      Assert.AreEqual(expected, choiceInfo);
    }

    private static Data CreateDefaultDataStructuresFromMockProblem()
    {
      var problem = new MockProblem();
      var data = new Data(problem, InitialPheromoneDensity);
      return data;
    }
  }
}