using AntSimComplexAlgorithms.Utilities;
using AntSimComplexAlgorithms.Utilities.DataStructures;
using AntSimComplexAlgorithms.Utilities.RouletteWheelSelector;
using System;
using TspLibNet;

namespace AntSimComplexAlgorithms.ProblemContext
{
  /// <summary>
  /// Wraps all of the Utility classes containing information related to a specific TSP problem.
  /// </summary>
  internal class Context : IProblemContext
  {
    private readonly IDataStructures _dataStructures;
    private readonly IRouletteWheelSelector _rouletteWheelSelector;

    /// <summary>
    /// The global AntSystem Random object instance.
    /// </summary>
    public Random Random { get; }

    /// <summary>
    /// Nr of nodes is used everywhere as it determines the dimensions of the distance,
    /// nearest neighbour and pheromone density matrices.
    /// </summary>
    public int NodeCount { get; }

    public double[][] Pheromone => _dataStructures.Pheromone;

    public int[] NearestNeighbours(int node)
    {
      return _dataStructures.NearestNeighbours(node);
    }

    public double Distance(int node1, int node2)
    {
      return _dataStructures.Distance(node1, node2);
    }

    public double ChoiceInfo(int node1, int node2)
    {
      return _dataStructures.ChoiceInfo(node1, node2);
    }

    public void UpdateChoiceInfoMatrix()
    {
      _dataStructures.UpdateChoiceInfoMatrix();
    }

    public void ResetPheromone()
    {
      _dataStructures.ResetPheromone();
    }

    public int SelectNextNode(int[] notVisited, int currentNode)
    {
      return _rouletteWheelSelector.SelectNextNode(notVisited, currentNode);
    }

    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="problem">The TSP problem instance to which Ant System is to be applied.</param>
    /// <param name="random">The application global random object instance.</param>
    public Context(IProblem problem, Random random)
    {
      Random = random;
      NodeCount = problem.NodeProvider.CountNodes();

      var parameters = new Parameters(problem, Random);
      _dataStructures = new Data(problem, parameters.InitialPheromone);
      _rouletteWheelSelector = new RouletteWheel(_dataStructures, Random);
    }
  }
}