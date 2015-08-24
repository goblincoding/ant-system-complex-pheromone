using AntSimComplexAS;
using NUnit.Framework;
using System;

namespace AntSimComplexTests
{
    public class UtilitiesTests
    {
        #region Parameters

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestNullProblemParametersConstructor()
        {
            var parameters = new Parameters(null);
        }

        [Test]
        public void TestParametersConstructionSuccess()
        {
            var problem = Helpers.GetRandomTSPProblem();
            var parameters = new Parameters(problem);
            Assert.IsTrue(parameters.NumberOfAnts == problem.NodeProvider.GetNodes().Count);
            Assert.IsTrue(parameters.InitialPheromone > 0.0);
        }

        #endregion Parameters

        #region DataStructures

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestNullProblemDataStructuresConstructor()
        {
            var parameters = new DataStructures(null);
        }

        [Test]
        public void TestDataStructuresConstructionSuccess()
        {
            var problem = Helpers.GetRandomTSPProblem();
            var data = new DataStructures(problem);
            Assert.IsNotNull(data);
        }

        #endregion DataStructures
    }
}