using AntSimComplexAlgorithms.Ants;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AntSimComplexAlgorithms.Utilities.DataStructures
{
  /// <summary>
  /// This class represents the prepopulated (prior to algorithm run-time), consolidated,
  /// calculated values of different aspects of a particular TSP problem instance in data
  /// structures such as distance and nearest neighbour matrices.
  ///
  /// All the data structures are created and populated as per "Ant Colony Optimisation"
  /// Dorigo and Stutzle (2004), Ch3.8, p99 which is aimed at obtaining an efficient
  /// Ant System implementation.
  ///
  /// All matrices are n^2.  From Ant Colony Optimization, Dorigo 2004, p100:
  ///
  /// "In fact, although for symmetric TSPs we only need to store n(n-1)/2 distinct
  /// distances, it is more efficient to use an n^2 matrix to avoid performing
  /// additional operations to check whether, when accessing a generic distance
  /// d(i,j), entry (i,j) or entry (j,i) of the matrix should be used."
  ///
  /// The values of only two matrices get updated after creation, those of the pheromone
  /// matrix and the choice info matrix (since the choice info heuristic is directly
  /// dependent on pheromone density).
  /// </summary>
  internal abstract class ProblemDataBase : IProblemData
  {
    public int NodeCount { get; }

    /// <summary>
    /// The initial pheromone density with which to initialise the pheromone matrix values.
    /// </summary>
    protected double InitialPheromoneDensity;

    /// <summary>
    /// Represents the [n_ij]^B heuristic values for each edge [i][j] where
    /// n_ij = 1/d_ij and 'B' is the Beta parameter value <seealso cref="Parameters"/>
    /// Once initialised, the values in this matrix do not change.
    /// </summary>
    protected double[][] Heuristic;

    /// <summary>
    /// Represents ALL inter-city distances in a grid format, i.e. querying
    /// _distances[i][j] will return the distance from node i to node j.
    /// If i == j, the distance is set to double.MaxValue
    /// Once initialised, the values in this matrix do not change.
    /// </summary>
    private double[][] _distances;

    /// <summary>
    /// Represents the nearest neighbour lists for all nodes where _nearest[i]
    /// returns an array of the (adjusted from TSPLIB node ID's) node ids sorted
    /// by increasing distance from node i.
    /// Once initialised, the values in this matrix do not change.
    /// </summary>
    private int[][] _nearest;

    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="nodeCount">The nr of nodes in the TSP graph.</param>
    /// <param name="initialPheromoneDensity">Pheromone amount with which to initialise pheromone density</param>
    /// <param name="distances">The distance matrix containing node to node edge weights.</param>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when "initialPheromoneDensity" is out of range.</exception>
    protected ProblemDataBase(int nodeCount, double initialPheromoneDensity, IReadOnlyList<IReadOnlyList<double>> distances)
    {
      if (initialPheromoneDensity <= 0.0)
      {
        throw new ArgumentOutOfRangeException(nameof(initialPheromoneDensity), "The initial pheromone density must be larger than zero.");
      }

      NodeCount = nodeCount;
      InitialPheromoneDensity = initialPheromoneDensity;

      PopulateDataStructures(distances);
    }

    public double Distance(int node1, int node2)
    {
      return _distances[node1][node2];
    }

    public IReadOnlyList<int> NearestNeighbours(int node)
    {
      return _nearest[node];
    }

    public abstract void ResetPheromone();

    public abstract void UpdatePheromoneTrails(IEnumerable<IAnt> ants);

    public abstract IReadOnlyList<IReadOnlyList<double>> ChoiceInfo(IAnt ant);

    protected abstract void EvaporatePheromone();

    protected abstract void DepositPheromone(IEnumerable<int> tour, double deposit);

    protected abstract void UpdateChoiceInfoMatrix();

    protected abstract void PopulatePheromoneChoiceStructures();

    /// <summary>
    /// Pheromone density matrix - p102
    /// Heuristics matrix - p102
    /// Choice info matrix - p102
    /// </summary>
    /// <param name="distances"></param>
    private void PopulateDataStructures(IReadOnlyList<IReadOnlyList<double>> distances)
    {
      // Initialise rows.
      _distances = new double[NodeCount][];
      Heuristic = new double[NodeCount][];
      _nearest = new int[NodeCount][];

      for (var i = 0; i < NodeCount; i++)
      {
        // Initialise columns.
        _distances[i] = new double[NodeCount];
        Heuristic[i] = new double[NodeCount];

        for (var j = 0; j < NodeCount; j++)
        {
          // Set the distance from a node to itself as sufficiently large that it
          // is HIGHLY unlikely to be selected.
          _distances[i][j] = i != j ? distances[i][j] : double.MaxValue;
          Heuristic[i][j] = Math.Pow(1.0 / _distances[i][j], Parameters.Beta);
        }

        PopulateNearestNeighboursList(i);
      }

      PopulatePheromoneChoiceStructures();
    }

    /// <summary>
    /// From Ant Colony Optimization, Dorigo 2004 , p101.
    /// </summary>
    private void PopulateNearestNeighboursList(int currentNodeIndex)
    {
      // Select distance/index pairs for all nodes so that we may
      // know which index matched which distance after sorting.
      var pairs = _distances[currentNodeIndex]
        .Select((distance, index) => new { Distance = distance, Index = index })
        .OrderBy(pair => pair.Distance);

      // Remove nodes from their own nearest neighbour lists.
      var nearestIndices = pairs.Where(p => p.Index != currentNodeIndex).Select(p => p.Index).ToArray();

      // -1 since nodes aren't included in their own nearest neighbours lists.
      _nearest[currentNodeIndex] = new int[NodeCount - 1];
      nearestIndices.CopyTo(_nearest[currentNodeIndex], 0);
    }
  }
}