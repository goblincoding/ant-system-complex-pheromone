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

        [Test]
        public void TestDataStructuresGetInterNodeDistanceSuccess()
        {
            var problem = Helpers.GetRandomTSPProblem();
            var data = new DataStructures(problem);
            var nodes = problem.NodeProvider.GetNodes();
            for (int i = 0; i < nodes.Count - 1; i++)
            {
                var distance = data.GetInterNodeDistance(nodes[i], nodes[i + 1]);
                Assert.IsTrue(distance >= 0);
            }

            nodes.Reverse();
            for (int i = 0; i < nodes.Count - 1; i++)
            {
                var distance = data.GetInterNodeDistance(nodes[i], nodes[i + 1]);
                Assert.IsTrue(distance >= 0);
            }
        }

        [Test]
        [ExpectedException(typeof(IndexOutOfRangeException))]
        public void TestDataStructuresGetInterNodeDistanceFail()
        {
            var problem = Helpers.GetTSPProblemByName("ulysses16.tsp");
            var data = new DataStructures(problem);

            var otherProblem = Helpers.GetTSPProblemByName("eil51");
            var nodes = otherProblem.NodeProvider.GetNodes();
            for (int i = 0; i < nodes.Count - 1; i++)
            {
                var distance = data.GetInterNodeDistance(nodes[i], nodes[i + 1]);
            }
        }

        #endregion DataStructures
    }
}