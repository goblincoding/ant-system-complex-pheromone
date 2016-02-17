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
  internal class ProblemData : IProblemData
  {
    public Random Random { get; }
    public int NodeCount { get; }

    /// <summary>
    /// The initial pheromone density with which to initialise the pheromone matrix values.
    /// </summary>
    private readonly double _initialPheromoneDensity;

    /// <summary>
    /// Represents the simple pheromone density trails between two nodes (graph arcs)
    /// for the "standard" Ant System implementation. Pheromone is frequently updated
    /// during the evaporation and deposit steps.
    /// </summary>
    private double[][] _pheromone;

    /// <summary>
    /// Represents ALL inter-city distances in a grid format, i.e. querying
    /// _distances[i][j] will return the distance from node i to node j.
    /// If i == j, the distance is set to double.MaxValue
    /// Once initialised, the values in this matrix do not change.
    /// </summary>
    private double[][] _distances;

    /// <summary>
    /// Represents the [n_ij]^B heuristic values for each edge [i][j] where
    /// n_ij = 1/d_ij and 'B' is the Beta parameter value <seealso cref="Parameters"/>
    /// Once initialised, the values in this matrix do not change.
    /// </summary>
    private double[][] _heuristic;

    /// <summary>
    /// Represents the [t_ij]^A [n_ij]^B heuristic values for each edge [i][j] where
    /// t_ij is the pheromone density, n_ij = 1/d_ij, and 'A' and 'B' are the Alpha
    /// and Beta parameter values <seealso cref="Parameters"/>
    /// </summary>
    private double[][] _choiceInfo;

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
    /// <param name="random">The global AntSystem Random object instance.</param>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when "initialPheromoneDensity" is out of range.</exception>
    public ProblemData(int nodeCount,
                       double initialPheromoneDensity,
                       IReadOnlyList<IReadOnlyList<double>> distances,
                       Random random)
    {
      if (initialPheromoneDensity <= 0.0)
      {
        throw new ArgumentOutOfRangeException(nameof(initialPheromoneDensity), "The initial pheromone density must be larger than zero.");
      }

      NodeCount = nodeCount;
      _initialPheromoneDensity = initialPheromoneDensity;
      Random = random;

      PopulateDataStructures(distances);
    }

    public void ResetPheromone()
    {
      var pheromone = _pheromone;
      foreach (var p in pheromone)
      {
        for (var j = 0; j < p.Length; j++)
        {
          p[j] = _initialPheromoneDensity;
        }
      }

      UpdateChoiceInfoMatrix();
    }

    public void EvaporatePheromone()
    {
      for (var i = 0; i < NodeCount; i++)
      {
        for (var j = i; j < NodeCount; j++)
        {
          var pher = _pheromone[i][j] * (1.0 - Parameters.EvaporationRate);
          _pheromone[i][j] = pher;  // matrix is symmetric
          _pheromone[j][i] = pher;
        }
      }
    }

    public void DepositPheromone(IEnumerable<int> tour, double deposit)
    {
      var tourArray = tour.ToArray();
      for (var i = 0; i < tourArray.Length - 1; i++)
      {
        var j = tourArray[i];
        var l = tourArray[i + 1];
        var pher = _pheromone[j][l] + deposit;
        _pheromone[j][l] = pher;  // matrix is symmetric
        _pheromone[l][j] = pher;
      }
    }

    public double Distance(int node1, int node2)
    {
      return _distances[node1][node2];
    }

    public IReadOnlyList<int> NearestNeighbours(int node)
    {
      return _nearest[node];
    }

    public double ChoiceInfo(int node1, int node2)
    {
      return _choiceInfo[node1][node2];
    }

    public void UpdateChoiceInfoMatrix()
    {
      // Matrix is symmetric.
      for (var i = 0; i < NodeCount; i++)
      {
        for (var j = i; j < NodeCount; j++)
        {
          var choice = CalculateChoiceInfo(i, j);
          _choiceInfo[i][j] = choice;
          _choiceInfo[j][i] = choice;
        }
      }
    }

    private double CalculateChoiceInfo(int i, int j)
    {
      return Math.Pow(_pheromone[i][j], Parameters.Alpha) * _heuristic[i][j];
    }

    /// <summary>
    /// Pheromone density matrix - p102
    /// Heuristics matrix - p102
    /// Choice info matrix - p102
    /// </summary>
    /// <param name="distances"></param>
    private void PopulateDataStructures(IReadOnlyList<IReadOnlyList<double>> distances)
    {
      // Initialise rows.
      _pheromone = new double[NodeCount][];
      _distances = new double[NodeCount][];
      _heuristic = new double[NodeCount][];
      _choiceInfo = new double[NodeCount][];
      _nearest = new int[NodeCount][];

      for (var i = 0; i < NodeCount; i++)
      {
        // Initialise columns.
        _pheromone[i] = new double[NodeCount];
        _distances[i] = new double[NodeCount];
        _heuristic[i] = new double[NodeCount];
        _choiceInfo[i] = new double[NodeCount];

        for (var j = 0; j < NodeCount; j++)
        {
          // Set the distance from a node to itself as sufficiently large that it
          // is HIGHLY unlikely to be selected.
          _distances[i][j] = i != j ? distances[i][j] : double.MaxValue;
          _heuristic[i][j] = Math.Pow(1.0 / _distances[i][j], Parameters.Beta);
          _pheromone[i][j] = _initialPheromoneDensity;
          _choiceInfo[i][j] = CalculateChoiceInfo(i, j);
        }

        PopulateNearestNeighboursList(i);
      }
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