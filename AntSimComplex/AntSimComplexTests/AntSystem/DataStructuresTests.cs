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
            var problem = Helpers.GetRandomTSPProblem();
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
            var problem = Helpers.GetRandomTSPProblem();
            var data = new DataStructures(problem, 0);
            var nodes = problem.NodeProvider.GetNodes();
            for (int i = 0; i < nodes.Count; i++)
            {
                var neighbours = data.NearestNeighbours(i);
                Assert.AreEqual(neighbours.Length, nodes.Count);

                for (int j = 0; j < nodes.Count; j++)
                {
                    Assert.IsTrue(neighbours.Contains(j));
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
            var problem = Helpers.GetRandomTSPProblem();
            var data = new DataStructures(problem, 1);
            var nodes = problem.NodeProvider.GetNodes();
            for (int i = 0; i < nodes.Count - 1; i++)
            {
                var density = data.PheromoneTrailDensity(i, i + 1);
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

            var density = data.PheromoneTrailDensity(random.Next(0, nodeCount), random.Next(0, nodeCount));
            Assert.AreEqual(density, parameters.InitialPheromone);
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
            var problem = Helpers.GetRandomTSPProblem();
            var data = new DataStructures(problem, 1);
            var nodes = problem.NodeProvider.GetNodes();
            for (int i = 0; i < nodes.Count - 1; i++)
            {
                data.SetPheromoneTrailDensity(i, i + 1, 1);
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
            var problem = Helpers.GetRandomTSPProblem();
            var parameters = new Parameters(problem);
            var data = new DataStructures(problem, parameters.InitialPheromone);

            var random = new Random();
            var nodeCount = problem.NodeProvider.CountNodes();

            var info = data.ChoiceInfo(random.Next(0, nodeCount), random.Next(0, nodeCount));
            Assert.IsTrue(info > 0.0);
        }
    }
}