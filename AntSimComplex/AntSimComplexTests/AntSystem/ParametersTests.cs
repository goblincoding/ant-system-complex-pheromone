using AntSimComplexAS.Utilities;
using NUnit.Framework;
using System;

namespace AntSimComplexTests
{
    public class ParametersTests
    {
        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestNullProblemParametersConstructor()
        {
            var parameters = new Parameters(null);
        }

        [Test]
        public void TestParametersConstructionSuccess()
        {
            var problem = new MockProblem();
            var parameters = new Parameters(problem);
            Assert.IsTrue(parameters.NumberOfAnts == problem.NodeProvider.CountNodes());
            Assert.IsTrue(parameters.InitialPheromone > 0.0);
        }
    }
}