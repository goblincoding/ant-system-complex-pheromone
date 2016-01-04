using System;
using System.Collections.Generic;
using System.Linq;
using TspLibNet;

namespace AntSimComplexAlgorithms.Utilities
{
  // Note:  Might have to change distance matrix to integers: ACO p101

  /// <summary>
  /// This class represents the prepopulated (prior to algorithm run-time), consolidated,
  /// calculated values of different aspects of a particular TSP problem instance in data
  /// structures such as distance and nearest neighbour matrices.
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
  /// </summary>
  public class DataStructures
  {
    /// <summary>
    /// Nr of nodes is used everywhere as it determines the dimensions of the distance,
    /// nearest neighbour and pheromone density matrices.
    /// </summary>
    private readonly int _nodeCount;

    /// <summary>
    /// Represents the simple pheromone density trails between two nodes (graph arcs)
    /// for the "standard" Ant System implementation. Pheromone is frequently updated
    /// during the evaporation and deposit steps.
    /// </summary>
    public double[][] Pheromone;

    /// <summary>
    /// Represents ALL inter-city distances in a grid format, i.e. querying
    /// _distances[3][5] will return the distance from node 3 to node 5.
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
    /// <param name="problem">The TSP problem instance to which Ant System is to be applied.</param>
    /// <exception cref="ArgumentNullException">Thrown when "problem" is null.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when "initialPheromoneDensity" is out of range.</exception>
    public DataStructures(IProblem problem, double initialPheromoneDensity)
    {
      if (problem == null)
      {
        throw new ArgumentNullException(nameof(problem), $"The {nameof(DataStructures)} constructor needs a valid problem instance argument");
      }

      if (initialPheromoneDensity <= 0.0)
      {
        throw new ArgumentOutOfRangeException(nameof(initialPheromoneDensity), "The initial pheromone density must be larger than zero.");
      }

      _nodeCount = problem.NodeProvider.CountNodes();

      // Order is important!
      BuildInfoMatrices(problem, initialPheromoneDensity);
      BuildNearestNeighboursLists();
    }

    /// <summary>
    /// This method does not calculate the edge weight between two nodes, but references
    /// the weights obtained from the original problem with which the <seealso cref="DataStructures"/>
    /// object was constructed.
    /// </summary>
    /// <param name="node1">The index of the first node</param>
    /// <param name="node2">The index of the second node</param>
    /// <returns>Returns the distance (weight of the edge) between two nodes.</returns>
    /// <exception cref="IndexOutOfRangeException">Thrown when either of the two node indices fall outside the expected range.</exception>
    public double Distance(int node1, int node2)
    {
      return _distances[node1][node2];
    }

    /// <summary>
    /// This method does not create the nearest neighbours list, but references
    /// the lists obtained from the original problem with which the <seealso cref="DataStructures"/>
    /// object was constructed.
    /// </summary>
    /// <param name="node">The node index whose neighbours should be returned.</param>
    /// <returns>Returns an array of neighbouring node indices, ordered by ascending distance.</returns>
    /// <exception cref="IndexOutOfRangeException">Thrown when the node index falls outside the expected range.</exception>
    public int[] NearestNeighbours(int node)
    {
      return _nearest[node];
    }

    /// <summary>
    /// Represents the [t_ij]^A [n_ij]^B heuristic values for each edge [i][j] where t_ij is the
    /// pheromone density, n_ij = 1/d_ij, and 'A' and 'B' are the Alpha and Beta parameter values.
    /// This method does not calculate the choice info heuristics, but references values in a matrix
    /// of dimensions dependent on the original problem with which the <seealso cref="DataStructures"/>
    /// object was constructed.
    /// </summary>
    /// <param name="node1">The index of the first node</param>
    /// <param name="node2">The index of the second node</param>
    /// <returns>Returns the "choice info" heuristic for two nodes.</returns>
    /// <exception cref="IndexOutOfRangeException">Thrown when either of the two node indices fall outside the expected range.</exception>
    public double ChoiceInfo(int node1, int node2)
    {
      return _choiceInfo[node1][node2];
    }

    /// <summary>
    /// Updates the ChoiceInfo matrix with the latest pheromone values.  Should be called after the pheromone update
    /// process is completed.
    /// </summary>
    public void UpdateChoiceInfoMatrix()
    {
      // Matrix is symmetric.
      for (var i = 0; i < _nodeCount; i++)
      {
        for (var j = i; j < _nodeCount; j++)
        {
          UpdateChoiceInfo(i, j);
          UpdateChoiceInfo(j, i);
        }
      }
    }

    private void UpdateChoiceInfo(int i, int j)
    {
      _choiceInfo[i][j] = Math.Pow(Pheromone[i][j], Parameters.Alpha) * _heuristic[i][j];
    }

    /// <summary>
    /// Pheromone density matrix - p102
    /// Heuristics matrix - p102
    /// Choice info matrix - p102
    /// </summary>
    /// <param name="problem"></param>
    /// <param name="initialPheromoneDensity"></param>
    private void BuildInfoMatrices(IProblem problem, double initialPheromoneDensity)
    {
      // Initialise rows.
      Pheromone = new double[_nodeCount][];
      _distances = new double[_nodeCount][];
      _heuristic = new double[_nodeCount][];
      _choiceInfo = new double[_nodeCount][];

      // Ensure that the nodes are sorted by ID ascending
      // or else all matrix indices will be off.
      var nodes = problem.NodeProvider.GetNodes()
                                          .OrderBy(n => n.Id)
                                          .ToArray();

      var weightsProvider = problem.EdgeWeightsProvider;

      for (var i = 0; i < _nodeCount; i++)
      {
        // Initialise columns.
        Pheromone[i] = new double[_nodeCount];
        _distances[i] = new double[_nodeCount];
        _heuristic[i] = new double[_nodeCount];
        _choiceInfo[i] = new double[_nodeCount];

        for (var j = 0; j < _nodeCount; j++)
        {
          // Set the distance from a node to itself as sufficiently large that it
          // is HIGHLY unlikely to be selected.
          _distances[i][j] = (i != j) ? weightsProvider.GetWeight(nodes[i], nodes[j]) : int.MaxValue;
          _heuristic[i][j] = Math.Pow((1 / _distances[i][j]), Parameters.Beta);
          Pheromone[i][j] = initialPheromoneDensity;
          UpdateChoiceInfo(i, j);
        }
      }
    }

    /// <summary>
    /// From Ant Colony Optimization, Dorigo 2004 , p101.
    /// </summary>
    private void BuildNearestNeighboursLists()
    {
      _nearest = new int[_nodeCount][];

      for (var i = 0; i < _nodeCount; i++)
      {
        // -1 since nodes aren't included in their own nearest neighbours lists.
        _nearest[i] = new int[_nodeCount - 1];

        // Select all the distance/index pairs for all nodes OTHER than
        // the current (i) (we need this so that we do not lose the position
        // of the index once we sort by distance).  In this case, "key" is
        // distance.
        var pairs = _distances[i]
                          .Select((distance, index) => new KeyValuePair<double, int>(distance, index))
                          .OrderBy(pair => pair.Key).ToList();

        // Remove nodes from their own nearest neighbour lists.
        var nearestIndices = pairs.Where(p => p.Value != i).Select(p => p.Value).ToArray();
        nearestIndices.CopyTo(_nearest[i], 0);
      }
    }
  }
}