using AntSimComplexAlgorithms.Ants;
using AntSimComplexAlgorithms.Smart;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AntSimComplexAlgorithms.Utilities.DataStructures
{
  internal class SmartProblemData : ProblemDataBase
  {
    /// <summary>
    /// Represents the SmartPheromone density trails between two nodes (graph arcs)
    /// for the adapted Ant System implementation. Pheromone is frequently updated
    /// during the evaporation and deposit steps.
    /// </summary>
    private SmartPheromone[][] _pheromone;

    /// <summary>
    /// Represents the [t_ij]^A [n_ij]^B heuristic values for each edge [i][j] where
    /// t_ij is the pheromone density, n_ij = 1/d_ij, and 'A' and 'B' are the Alpha
    /// and Beta parameter values <seealso cref="Parameters"/>
    /// Matrix is NOT symmetric.
    /// </summary>
    private double[][] _choiceInfo;

    public SmartProblemData(int nodeCount, double initialPheromoneDensity, IReadOnlyList<IReadOnlyList<double>> distances)
      : base(nodeCount, initialPheromoneDensity, distances)
    {
    }

    public override void ResetPheromone()
    {
      for (var i = 0; i < NodeCount; i++)
      {
        for (var j = i; j < NodeCount; j++)
        {
          // Matrix is symmetric, the same SmartPheromone
          // object is referenced by [i][j] and [j][i]
          if (j == i) continue;
          _pheromone[i][j].Reset();
        }
      }

      UpdateChoiceInfoMatrix();
    }

    public override void UpdateGlobalPheromoneTrails(IEnumerable<IAnt> ants)
    {
      EvaporatePheromone();
      DepositPheromone(ants);

      // Choice info matrix has to be updated AFTER pheromone changes.
      UpdateChoiceInfoMatrix();
    }

    public override void UpdateLocalPheromoneTrails(IEnumerable<IAnt> ants)
    {
      foreach (var ant in ants)
      {
        for (var i = 0; i < NodeCount; i++)
        {
          // Identify the arcs between the ant's current node
          // and those it has not yet visited and inform the
          // SmartPheromone object that it will be evaluated
          // during the next step decision.
          if (ant.CurrentNode == i || ant.Visited[i]) continue;

          // Matrix is symmetric, the same SmartPheromone
          // object is referenced by [i][j] and [j][i]
          _pheromone[ant.CurrentNode][i].Touch(ant);

          _choiceInfo[ant.CurrentNode][i] = CalculateChoiceInfo(ant.CurrentNode, i);
          _choiceInfo[i][ant.CurrentNode] = CalculateChoiceInfo(i, ant.CurrentNode);
        }
      }
    }

    public override IReadOnlyList<IReadOnlyList<double>> ChoiceInfo(IAnt ant)
    {
      return _choiceInfo;
    }

    protected override void UpdateChoiceInfoMatrix()
    {
      for (var i = 0; i < NodeCount; i++)
      {
        for (var j = 0; j < NodeCount; j++)
        {
          if (j == i) continue;
          _choiceInfo[i][j] = CalculateChoiceInfo(i, j);
        }
      }
    }

    protected override void EvaporatePheromone()
    {
      for (var i = 0; i < NodeCount; i++)
      {
        for (var j = i; j < NodeCount; j++)
        {
          // Matrix is symmetric, the same SmartPheromone
          // object is referenced by [i][j] and [j][i]
          if (j == i) continue;
          _pheromone[i][j].Evaporate(Parameters.EvaporationRate);
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
        // Matrix is symmetric, the same SmartPheromone
        // object is referenced by [i][j] and [j][i]
        if (j == l) continue;
        _pheromone[j][l].Deposit(deposit);
      }
    }

    protected override void PopulatePheromoneChoiceStructures()
    {
      PopulatePheromoneStructures();
      PopulateChoiceStructures();
    }

    private void PopulateChoiceStructures()
    {
      // Initialise rows.
      _choiceInfo = new double[NodeCount][];

      for (var i = 0; i < NodeCount; i++)
      {
        // Initialise columns.
        _choiceInfo[i] = new double[NodeCount];

        for (var j = 0; j < NodeCount; j++)
        {
          if (i == j) continue;
          _choiceInfo[i][j] = CalculateChoiceInfo(i, j);
        }
      }
    }

    private void PopulatePheromoneStructures()
    {
      // Initialise rows.
      _pheromone = new SmartPheromone[NodeCount][];

      for (var i = 0; i < NodeCount; i++)
      {
        // Initialise columns.
        _pheromone[i] = new SmartPheromone[NodeCount];
      }

      for (var i = 0; i < NodeCount; i++)
      {
        for (var j = i; j < NodeCount; j++)
        {
          // Matrix is symmetric, the same SmartPheromone
          // object is referenced by [i][j] and [j][i]
          if (i == j) continue;
          var smart = new SmartPheromone(i, j, Distance(i, j), InitialPheromoneDensity);
          _pheromone[i][j] = smart;
          _pheromone[j][i] = smart;
        }
      }
    }

    private void DepositPheromone(IEnumerable<IAnt> ants)
    {
      foreach (var ant in ants)
      {
        var deposit = 1.0 / ant.TourLength;
        DepositPheromone(ant.Tour, deposit);
      }
    }

    private double CalculateChoiceInfo(int i, int j)
    {
      return Math.Pow(_pheromone[i][j].Density(i), Parameters.Alpha) * Heuristic[i][j];
    }
  }
}