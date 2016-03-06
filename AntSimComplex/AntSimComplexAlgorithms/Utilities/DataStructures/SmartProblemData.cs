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
    /// Key = ant id, Value = choice info for the ant in question.
    /// </summary>
    private Dictionary<int, double[][]> _choiceInfo;

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
          _pheromone[i][j].Reset();
        }
      }

      UpdateChoiceInfoMatrix();
    }

    public override void UpdatePheromoneTrails(IEnumerable<IAnt> ants)
    {
      EvaporatePheromone();
      var enumerable = ants as IAnt[] ?? ants.ToArray();
      DepositPheromone(enumerable);
      TouchPheromone(enumerable);

      // Choice info matrix has to be updated AFTER pheromone changes.
      UpdateChoiceInfoMatrix();
    }

    public override IReadOnlyList<IReadOnlyList<double>> ChoiceInfo(IAnt ant)
    {
      return _choiceInfo[ant.Id];
    }

    protected override void UpdateChoiceInfoMatrix()
    {
      foreach (var antId in _choiceInfo.Keys)
      {
        for (var i = 0; i < NodeCount; i++)
        {
          for (var j = i; j < NodeCount; j++)
          {
            var choice = CalculateChoiceInfo(antId, i, j);
            _choiceInfo[antId][i][j] = choice;
            _choiceInfo[antId][j][i] = choice;
          }
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
        _pheromone[j][l].Deposit(deposit);
      }
    }

    protected override void PopulatePheromoneChoiceStructures()
    {
      PopulateSmartPheromoneDataStructure();
      PopulateChoiceInfoDictionary();
    }

    private void PopulateSmartPheromoneDataStructure()
    {
      // Initialise rows.
      _pheromone = new SmartPheromone[NodeCount][];

      for (var i = 0; i < NodeCount; i++)
      {
        // Initialise columns.
        _pheromone[i] = new SmartPheromone[NodeCount];

        for (var j = 0; j < NodeCount; j++)
        {
          _pheromone[i][j] = new SmartPheromone(i, j, Distance(i, j), NodeCount, InitialPheromoneDensity);
        }
      }
    }

    private void PopulateChoiceInfoDictionary()
    {
      _choiceInfo = new Dictionary<int, double[][]>();

      // Initialise arrays
      for (var i = 0; i < NodeCount; i++)
      {
        // Initialise rows.
        _choiceInfo[i] = new double[NodeCount][];

        for (var j = 0; j < NodeCount; j++)
        {
          // Initialise columns.
          _choiceInfo[i][j] = new double[NodeCount];

          for (var k = 0; k < NodeCount; k++)
          {
            _choiceInfo[i][j][k] = CalculateChoiceInfo(i, j, k);
          }
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

    private void TouchPheromone(IAnt[] ants)
    {
      for (var i = 0; i < NodeCount; i++)
      {
        // Local variable is required to eliminate capture
        // from closure in LINQ Where below
        var i1 = i;
        for (var j = i; j < NodeCount; j++)
        {
          var j1 = j;
          // Identify the arcs between the ant's current node
          // and those it has not yet visited and inform the
          // SmartPheromone object that it will be evaluated
          // during the next step decision.
          var candidates = ants.Where(a => a.CurrentNode == i1 && !a.Visited[j1]);
          foreach (var candidate in candidates)
          {
            // Matrix is symmetric, the same SmartPheromone
            // object is referenced by [i][j] and [j][i]
            _pheromone[i][j].Touch(candidate);
          }
        }
      }
    }

    private double CalculateChoiceInfo(int antId, int i, int j)
    {
      return Math.Pow(_pheromone[i][j].Density(antId), Parameters.Alpha) * Heuristic[i][j];
    }
  }
}