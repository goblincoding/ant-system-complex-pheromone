using AntSimComplexAlgorithms.Ants;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AntSimComplexAlgorithms.Utilities.DataStructures
{
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

    public StandardProblemData(int nodeCount,
                               double initialPheromoneDensity,
                               IReadOnlyList<IReadOnlyList<double>> distances)
      : base(nodeCount, initialPheromoneDensity, distances)
    {
    }

    public override void ResetPheromone()
    {
      for (var i = 0; i < NodeCount; i++)
      {
        for (var j = i; j < NodeCount; j++)
        {
          _pheromone[i][j] = InitialPheromoneDensity;  // matrix is symmetric
          _pheromone[j][i] = InitialPheromoneDensity;
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

    public override IReadOnlyList<IReadOnlyList<double>> ChoiceInfo(IAnt ant)
    {
      return _choiceInfo;
    }

    public override void UpdateGlobalPheromoneTrails(IEnumerable<IAnt> ants)
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

    public override void UpdateLocalPheromoneTrails(IEnumerable<IAnt> ants)
    {
      // No local pheromone updates for standard implementation.
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

    private double CalculateChoiceInfo(int i, int j)
    {
      return Math.Pow(_pheromone[i][j], Parameters.Alpha) * Heuristic[i][j];
    }
  }
}