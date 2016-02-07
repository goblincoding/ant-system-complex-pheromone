using AntSimComplexAlgorithms.Utilities;
using AntSimComplexAlgorithms.Utilities.DataStructures;
using AntSimComplexAlgorithms.Utilities.RouletteWheelSelector;
using System;
using System.Collections.Generic;

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

    public void EvaporatePheromone()
    {
      _dataStructures.EvaporatePheromone();
    }

    public void DepositPheromone(IEnumerable<int> tour, double deposit)
    {
      _dataStructures.DepositPheromone(tour, deposit);
    }

    public int SelectNextNode(int[] notVisited, int currentNode)
    {
      return _rouletteWheelSelector.SelectNextNode(notVisited, currentNode);
    }

    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="nodeCount">The nr of nodes in the TSP graph.</param>
    /// <param name="nearestNeighbourTourLength">The tour length constructed through the Nearest Neighbour Heuristic.</param>
    /// <param name="distances">The distance matrix containing node to node edge weights.</param>
    /// <param name="random">The application global random object instance.</param>
    public Context(int nodeCount, double nearestNeighbourTourLength, IReadOnlyList<IReadOnlyList<double>> distances, Random random)
    {
      Random = random;
      NodeCount = nodeCount;

      var parameters = new Parameters(NodeCount, nearestNeighbourTourLength);
      _dataStructures = new Data(NodeCount, parameters.InitialPheromone, distances);
      _rouletteWheelSelector = new RouletteWheel(_dataStructures, Random);
    }
  }
}