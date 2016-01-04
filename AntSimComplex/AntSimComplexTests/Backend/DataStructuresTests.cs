using AntSimComplexAlgorithms.Utilities;
using NUnit.Framework;
using System;

namespace AntSimComplexTests.Backend
{
  internal class DataStructuresTests
  {
    private const double _initialPheromoneDensity = 0.5;

    [Test]
    public void TestNullProblemDataStructuresConstructorFail()
    {
      Assert.Throws<ArgumentNullException>(() => new DataStructures(null, _initialPheromoneDensity));
    }

    [TestCase(0.0)]
    [TestCase(-1)]
    public void TestInitialPheromoneOutOfRangeDataStructuresConstructorFail(double initialPheromone)
    {
      var problem = new MockProblem();
      Assert.Throws<ArgumentOutOfRangeException>(() => new DataStructures(problem, initialPheromone));
    }

    [Test]
    public void TestDataStructuresConstructionSuccess()
    {
      var problem = new MockProblem();
      var data = new DataStructures(problem, _initialPheromoneDensity);
      Assert.IsNotNull(data);
    }

    [Test]
    public void TestDataStructuresDistanceIndexInvalid()
    {
      var problem = new MockProblem();
      var data = new DataStructures(problem, _initialPheromoneDensity);
      Assert.Throws<IndexOutOfRangeException>(() => data.Distance(0, MockConstants.NrNodes));
    }

    [Test]
    public void TestDataStructuresGetInterNodeDistanceSuccess()
    {
      var problem = new MockProblem();
      var data = new DataStructures(problem, _initialPheromoneDensity);
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
    public void TestDataStructuresNearestNeighboursIndexInvalid()
    {
      var problem = new MockProblem();
      var data = new DataStructures(problem, _initialPheromoneDensity);
      Assert.Throws<IndexOutOfRangeException>(() => data.NearestNeighbours(MockConstants.NrNodes));
    }

    [Test]
    public void TestDataStructuresGetNearestNeighboursSuccess()
    {
      var problem = new MockProblem();
      var data = new DataStructures(problem, _initialPheromoneDensity);
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
    public void TestDataStructuresGetChoiceInfoIndexInvalid()
    {
      var problem = new MockProblem();
      var parameters = new Parameters(problem);
      var data = new DataStructures(problem, parameters.InitialPheromone);
      Assert.Throws<IndexOutOfRangeException>(() => data.ChoiceInfo(0, MockConstants.NrNodes));
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
      var choiceInfo = Math.Pow(data.Pheromone[i][j], Parameters.Alpha) * heuristic;

      var info = data.ChoiceInfo(i, j);
      Assert.IsTrue(info > 0.0);
      Assert.AreEqual(info, choiceInfo);
    }
  }
}