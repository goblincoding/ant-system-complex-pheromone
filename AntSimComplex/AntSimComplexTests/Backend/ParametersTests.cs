using AntSimComplexAlgorithms.Utilities;
using NUnit.Framework;
using System;

namespace AntSimComplexTests.Backend
{
  public class ParametersTests
  {
    [Test]
    public void TestNullProblemParametersConstructor()
    {
      Assert.Throws<ArgumentNullException>(() => new Parameters(null));
    }

    [Test]
    public void TestParametersConstructionSuccess()
    {
      var problem = new MockProblem();
      var parameters = new Parameters(problem);
      Assert.IsTrue(parameters.InitialPheromone > 0.0);
    }
  }
}