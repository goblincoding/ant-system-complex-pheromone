using AntSimComplexAS;
using NUnit.Framework;
using System;
using System.Linq;

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
            Assert.IsTrue(parameters.NumberOfAnts == problem.NodeProvider.CountNodes());
            Assert.IsTrue(parameters.InitialPheromone > 0.0);
            Assert.IsTrue(parameters.EvaporationRate >= 0.0);
        }

        #endregion Parameters

        #region DataStructures

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestNullProblemDataStructuresConstructor()
        {
            var parameters = new DataStructures(null, 0);
        }

        [Test]
        public void TestDataStructuresConstructionSuccess()
        {
            var problem = Helpers.GetRandomTSPProblem();
            var data = new DataStructures(problem, 0);
            Assert.IsNotNull(data);
        }

        [Test]
        [ExpectedException(typeof(IndexOutOfRangeException))]
        public void TestDataStructuresGetInterNodeDistanceFail()
        {
            var problem = Helpers.GetTSPProblemByName("ulysses16.tsp");
            var data = new DataStructures(problem, 0);

            var otherProblem = Helpers.GetTSPProblemByName("eil51");
            var nodes = otherProblem.NodeProvider.GetNodes();
            for (int i = 0; i < nodes.Count - 1; i++)
            {
                var distance = data.GetInterNodeDistance(nodes[i], nodes[i + 1]);
            }
        }

        [Test]
        public void TestDataStructuresGetInterNodeDistanceSuccess()
        {
            var problem = Helpers.GetRandomTSPProblem();
            var data = new DataStructures(problem, 0);
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
        public void TestDataStructuresGetNearestNeighboursFail()
        {
            var problem = Helpers.GetTSPProblemByName("ulysses16.tsp");
            var data = new DataStructures(problem, 0);

            var otherProblem = Helpers.GetTSPProblemByName("eil51");
            var nodes = otherProblem.NodeProvider.GetNodes();
            for (int i = 0; i < nodes.Count; i++)
            {
                var neighbours = data.GetNearestNeighbourIDs(nodes[i]);
            }
        }

        [Test]
        public void TestDataStructuresGetNearestNeighboursSuccess()
        {
            var problem = Helpers.GetRandomTSPProblem();
            var data = new DataStructures(problem, 0);
            var nodes = problem.NodeProvider.GetNodes();
            for (int i = 0; i < nodes.Count; i++)
            {
                var neighbours = data.GetNearestNeighbourIDs(nodes[i]);
                Assert.AreEqual(neighbours.Length, nodes.Count);

                foreach (var node in nodes)
                {
                    Assert.IsTrue(neighbours.Contains(node.Id));
                }
            }
        }

        [Test]
        [ExpectedException(typeof(IndexOutOfRangeException))]
        public void TestDataStructuresGetPheromoneFail()
        {
            var problem = Helpers.GetTSPProblemByName("ulysses16.tsp");
            var parameters = new Parameters(problem);
            var data = new DataStructures(problem, parameters.InitialPheromone);

            var otherProblem = Helpers.GetTSPProblemByName("eil51");
            var nodes = otherProblem.NodeProvider.GetNodes();
            for (int i = 0; i < nodes.Count - 1; i++)
            {
                var density = data.GetPheromoneTrailDensity(nodes[i], nodes[i + 1]);
            }
        }

        [Test]
        public void TestDataStructuresGetPheromoneSuccess()
        {
            var problem = Helpers.GetRandomTSPProblem();
            var data = new DataStructures(problem, 1);
            var nodes = problem.NodeProvider.GetNodes();
            for (int i = 0; i < nodes.Count - 1; i++)
            {
                var density = data.GetPheromoneTrailDensity(nodes[i], nodes[i + 1]);
            }
        }

        [Test]
        public void TestDataStructuresGetInitialPheromoneSuccess()
        {
            var problem = Helpers.GetRandomTSPProblem();
            var parameters = new Parameters(problem);
            var data = new DataStructures(problem, parameters.InitialPheromone);

            var random = new Random();
            var nodeCount = problem.NodeProvider.CountNodes();
            var node1 = problem.NodeProvider.GetNodes().ElementAt(random.Next(0, nodeCount));
            var node2 = problem.NodeProvider.GetNodes().ElementAt(random.Next(0, nodeCount));

            var density = data.GetPheromoneTrailDensity(node1, node2);
            Assert.AreEqual(density, parameters.InitialPheromone);
        }

        [Test]
        [ExpectedException(typeof(IndexOutOfRangeException))]
        public void TestDataStructuresSetPheromoneFail()
        {
            var problem = Helpers.GetTSPProblemByName("ulysses16.tsp");
            var data = new DataStructures(problem, 1);

            var otherProblem = Helpers.GetTSPProblemByName("eil51");
            var nodes = otherProblem.NodeProvider.GetNodes();
            for (int i = 0; i < nodes.Count - 1; i++)
            {
                data.SetPheromoneTrailDensity(nodes[i], nodes[i + 1], 0);
            }
        }

        [Test]
        public void TestDataStructuresSetPheromoneSuccess()
        {
            var problem = Helpers.GetRandomTSPProblem();
            var data = new DataStructures(problem, 1);
            var nodes = problem.NodeProvider.GetNodes();
            for (int i = 0; i < nodes.Count - 1; i++)
            {
                data.SetPheromoneTrailDensity(nodes[i], nodes[i + 1], 1);
            }
        }

        #endregion DataStructures
    }
}