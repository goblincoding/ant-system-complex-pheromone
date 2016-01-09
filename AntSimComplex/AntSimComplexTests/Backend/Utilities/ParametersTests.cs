using AntSimComplexAlgorithms;
using AntSimComplexAlgorithms.Utilities;
using NSubstitute;
using NUnit.Framework;
using System;

namespace AntSimComplexTests.Backend.Utilities
{
  [TestFixture]
  public class ParametersTests
  {
    [Test]
    public void CtorGivenNullProblemShouldThrowArgumentNullException()
    {
      // ReSharper disable once ObjectCreationAsStatement
      Assert.Throws<ArgumentNullException>(() => new Parameters(null, ProblemContext.Random));
    }

    [Test]
    public void InitialPheromoneGivenMockProblemAndStartNode7ShouldReturn0Point5()
    {
      // arrange
      var problem = new MockProblem();
      var random = Substitute.For<Random>();
      random.Next(0, 9).Returns(7); // force start node to be 7

      // act
      var result = new Parameters(problem, random);

      // assert
      Assert.AreEqual(0.5, result.InitialPheromone);
    }
  }
}