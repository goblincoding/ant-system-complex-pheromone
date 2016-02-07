using AntSimComplexAlgorithms.Utilities;
using AntSimComplexTspLibItemManager.Utilities;
using NSubstitute;
using NUnit.Framework;
using System;

namespace AntSimComplexTests.Backend.Utilities
{
  [TestFixture]
  internal class ParametersTests
  {
    [Test]
    public void InitialPheromoneGivenMockProblemAndStartNode7ShouldReturn0Point5()
    {
      // arrange
      var problem = new MockProblem();
      var random = Substitute.For<Random>();
      random.Next(0, 9).Returns(7); // force start node to be 7

      // act
      var result = new Parameters(problem.NodeProvider.CountNodes(), problem.GetNearestNeighbourTourLength(random));

      // assert
      Assert.AreEqual(0.5, result.InitialPheromone);
    }
  }
}