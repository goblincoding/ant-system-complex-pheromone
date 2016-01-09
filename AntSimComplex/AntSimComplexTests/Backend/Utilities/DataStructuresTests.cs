using AntSimComplexAlgorithms;
using AntSimComplexAlgorithms.Utilities;
using NUnit.Framework;
using System;

namespace AntSimComplexTests.Backend.Utilities
{
  [TestFixture]
  public class DataStructuresTests
  {
    private const double InitialPheromoneDensity = 0.5;

    [Test]
    public void CtorGivenNullProblemInstanceShouldThrowArgumentNullException()
    {
      // assert
      // ReSharper disable once ObjectCreationAsStatement
      Assert.Throws<ArgumentNullException>(() => { new DataStructures(null, InitialPheromoneDensity); });
    }

    [TestCase(0.0)]
    [TestCase(-1)]
    public void CtorGivenInitialPheromoneNotGreaterThan0ShouldThrowArgumentOutOfRangeException(double initialPheromone)
    {
      // arrange
      var problem = new MockProblem();

      // assert
      // ReSharper disable once ObjectCreationAsStatement
      Assert.Throws<ArgumentOutOfRangeException>(() => new DataStructures(problem, initialPheromone));
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

    private DataStructures CreateDefaultDataStructuresFromMockProblem()
    {
      var problem = new MockProblem();
      var parameters = new Parameters(problem, ProblemContext.Random);
      var data = new DataStructures(problem, parameters.InitialPheromone);
      return data;
    }
  }
}