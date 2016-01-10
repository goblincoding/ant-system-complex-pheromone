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

    [Test]
    public void NearestNeighboursIndexInvalidShouldThrowIndexOutOfRangeException()
    {
      // arrange
      var data = CreateDefaultDataStructuresFromMockProblem();

      // assert
      Assert.Throws<IndexOutOfRangeException>(() => data.NearestNeighbours(MockConstants.NrNodes));
    }

    [Test]
    public void ChoiceInfoIndexInvalidShouldThrowIndexOutOfRangeException()
    {
      // arrange
      var data = CreateDefaultDataStructuresFromMockProblem();

      // assert
      Assert.Throws<IndexOutOfRangeException>(() => data.ChoiceInfo(0, MockConstants.NrNodes));
    }

    private static Data CreateDefaultDataStructuresFromMockProblem()
    {
      var problem = new MockProblem();
      var parameters = new Parameters(problem, new Random());
      var data = new Data(problem, parameters.InitialPheromone);
      return data;
    }
  }
}