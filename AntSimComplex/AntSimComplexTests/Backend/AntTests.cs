using AntSimComplexAlgorithms.ProblemContext;
using NSubstitute;
using NUnit.Framework;
using System;

namespace AntSimComplexTests.Backend
{
  [TestFixture]
  internal class AntTests
  {
    [Test]
    public void InitialiseWithMockProblemNode7ShouldReturnCorrectTour()
    {
      // arrange
      var problem = new MockProblem();
      var random = Substitute.For<Random>();

      var context = new Context(problem, random);
      // act

      // assert
    }
  }
}