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
  internal class StandardProblemData : ProblemDataBase
  {
    /// <summary>
    /// Represents the simple pheromone density trails between two nodes (graph arcs)
    /// for the "standard" Ant System implementation. Pheromone is frequently updated
    /// during the evaporation and deposit steps.
    /// </summary>
    private double[][] _pheromone;

    /// <summary>
    /// Represents the [t_ij]^A [n_ij]^B heuristic values for each edge [i][j] where
    /// t_ij is the pheromone density, n_ij = 1/d_ij, and 'A' and 'B' are the Alpha
    /// and Beta parameter values <seealso cref="Parameters"/>
    /// </summary>
    private double[][] _choiceInfo;

    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="nodeCount">The nr of nodes in the TSP graph.</param>
    /// <param name="initialPheromoneDensity">Pheromone amount with which to initialise pheromone density</param>
    /// <param name="distances">The distance matrix containing node to node edge weights.</param>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when "initialPheromoneDensity" is out of range.</exception>
    public StandardProblemData(int nodeCount,
                               double initialPheromoneDensity,
                               IReadOnlyList<IReadOnlyList<double>> distances)
      : base(nodeCount, initialPheromoneDensity, distances)
    {
    }

    public override void ResetPheromone()
    {
      var pheromone = _pheromone;
      foreach (var p in pheromone)
      {
        for (var j = 0; j < p.Length; j++)
        {
          p[j] = InitialPheromoneDensity;
        }
      }

      UpdateChoiceInfoMatrix();
    }

    protected override void EvaporatePheromone()
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

    protected override void DepositPheromone(IEnumerable<int> tour, double deposit)
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

    public override IReadOnlyList<IReadOnlyList<double>> ChoiceInfo(IAnt ant = null)
    {
      return _choiceInfo;
    }

    public override void UpdatePheromoneTrails(IEnumerable<IAnt> ants)
    {
      EvaporatePheromone();

      foreach (var ant in ants)
      {
        var deposit = 1.0 / ant.TourLength;
        DepositPheromone(ant.Tour, deposit);
      }

      // Choice info matrix has to be updated AFTER pheromone changes.
      UpdateChoiceInfoMatrix();
    }

    protected override void UpdateChoiceInfoMatrix()
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

    protected override double CalculateChoiceInfo(int i, int j)
    {
      return Math.Pow(_pheromone[i][j], Parameters.Alpha) * Heuristic[i][j];
    }

    protected override void PopulatePheromoneChoiceStructures()
    {
      // Initialise rows.
      _pheromone = new double[NodeCount][];
      _choiceInfo = new double[NodeCount][];

      for (var i = 0; i < NodeCount; i++)
      {
        // Initialise columns.
        _pheromone[i] = new double[NodeCount];
        _choiceInfo[i] = new double[NodeCount];

        for (var j = 0; j < NodeCount; j++)
        {
          _pheromone[i][j] = InitialPheromoneDensity;
          _choiceInfo[i][j] = CalculateChoiceInfo(i, j);
        }
      }
    }
  }
}