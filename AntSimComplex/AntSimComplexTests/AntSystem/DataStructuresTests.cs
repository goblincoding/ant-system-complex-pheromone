using AntSimComplexAS.Utilities;
using NUnit.Framework;
using System;
using System.Linq;

namespace AntSimComplexTests
{
    internal class DataStructuresTests
    {
        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestNullProblemDataStructuresConstructorFail()
        {
            var data = new DataStructures(null, 0);
        }

        [Test]
        public void TestDataStructuresConstructionSuccess()
        {
            var problem = new MockProblem();
            var data = new DataStructures(problem, 0);
            Assert.IsNotNull(data);
        }

        [Test]
        [ExpectedException(typeof(IndexOutOfRangeException))]
        public void TestDataStructuresDistanceIndexInvalid()
        {
            var problem = new MockProblem();
            var data = new DataStructures(problem, 0);
            var distance = data.Distance(0, MockConstants.NrNodes);
        }

        [Test]
        public void TestDataStructuresGetInterNodeDistanceSuccess()
        {
            var problem = new MockProblem();
            var data = new DataStructures(problem, 0);
            var nodes = problem.NodeProvider.GetNodes();
            for (int i = 0; i < nodes.Count - 1; i++)
            {
                var distance = data.Distance(i, i + 1);
                Assert.AreEqual(distance, problem.GetWeight(i, i + 1));
            }

            nodes.Reverse();
            for (int i = 0; i < nodes.Count - 1; i++)
            {
                var distance = data.Distance(i, i + 1);
                Assert.AreEqual(distance, problem.GetWeight(i, i + 1));
            }
        }

        [Test]
        [ExpectedException(typeof(IndexOutOfRangeException))]
        public void TestDataStructuresNearestNeighboursIndexInvalid()
        {
            var problem = new MockProblem();
            var data = new DataStructures(problem, 0);
            var neighbours = data.NearestNeighbours(MockConstants.NrNodes);
        }

        [Test]
        public void TestDataStructuresGetNearestNeighboursSuccess()
        {
            var problem = new MockProblem();
            var data = new DataStructures(problem, 0);
            var nodes = problem.NodeProvider.GetNodes();
            for (int i = 0; i < nodes.Count; i++)
            {
                var neighbours = data.NearestNeighbours(i);

                // -1 since nodes are not included in their own nearest neighbours lists.
                Assert.AreEqual(neighbours.Length, nodes.Count - 1);

                for (int j = 0; j < neighbours.Length - 1; j++)
                {
                    Assert.IsTrue(data.Distance(i, neighbours[j]) <= data.Distance(i, neighbours[j + 1]));
                }
            }
        }

        [Test]
        [ExpectedException(typeof(IndexOutOfRangeException))]
        public void TestDataStructuresPheromoneIndexInvalid()
        {
            var problem = new MockProblem();
            var data = new DataStructures(problem, 0);
            var density = data.PheromoneTrailDensity(0, MockConstants.NrNodes);
        }

        [Test]
        public void TestDataStructuresGetPheromoneSuccess()
        {
            const int pherDensity = 1;
            var problem = new MockProblem();
            var data = new DataStructures(problem, pherDensity);
            var nodes = problem.NodeProvider.GetNodes();
            for (int i = 0; i < nodes.Count - 1; i++)
            {
                var density = data.PheromoneTrailDensity(i, i + 1);
                Assert.AreEqual(density, pherDensity);
            }
        }

        [Test]
        [ExpectedException(typeof(IndexOutOfRangeException))]
        public void TestDataStructuresSetPheromoneFail()
        {
            var problem = new MockProblem();
            var data = new DataStructures(problem, 0);
            data.SetPheromoneTrailDensity(0, MockConstants.NrNodes, 0);
        }

        [Test]
        public void TestDataStructuresSetPheromoneSuccess()
        {
            const int pherDensity = 1;
            var problem = new MockProblem();
            var data = new DataStructures(problem, pherDensity);
            var nodes = problem.NodeProvider.GetNodes();
            for (int i = 0; i < nodes.Count - 1; i++)
            {
                const int newDensity = 2;
                data.SetPheromoneTrailDensity(i, i + 1, newDensity);
                Assert.AreEqual(data.PheromoneTrailDensity(i, i + 1), newDensity);
            }
        }

        [Test]
        [ExpectedException(typeof(IndexOutOfRangeException))]
        public void TestDataStructuresGetChoiceInfoIndexInvalid()
        {
            var problem = new MockProblem();
            var parameters = new Parameters(problem);
            var data = new DataStructures(problem, parameters.InitialPheromone);
            var density = data.ChoiceInfo(0, MockConstants.NrNodes);
        }

        [Test]
        public void TestDataStructuresGetChoiceInfoSuccess()
        {
            var problem = new MockProblem();
            var parameters = new Parameters(problem);
            var data = new DataStructures(problem, parameters.InitialPheromone);

            var random = new Random();
            var nodeCount = problem.NodeProvider.CountNodes();

            var i = random.Next(0, nodeCount);
            var j = random.Next(0, nodeCount);
            var heuristic = Math.Pow((1 / data.Distance(i, j)), Parameters.Beta);
            var choiceInfo = Math.Pow(data.PheromoneTrailDensity(i, j), Parameters.Alpha) * heuristic;

            var info = data.ChoiceInfo(i, j);
            Assert.IsTrue(info > 0.0);
            Assert.AreEqual(info, choiceInfo);
        }
    }
}