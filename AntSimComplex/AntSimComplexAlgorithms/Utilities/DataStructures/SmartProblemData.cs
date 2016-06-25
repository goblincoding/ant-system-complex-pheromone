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
    private ISmartPheromone[][] _pheromone;

    /// <summary>
    /// Represents the [t_ij]^A [n_ij]^B heuristic values for each edge [i][j] where
    /// t_ij is the pheromone density, n_ij = 1/d_ij, and 'A' and 'B' are the Alpha
    /// and Beta parameter values <seealso cref="Parameters"/>
    /// Matrix is NOT symmetric.
    /// </summary>
    private Dictionary<int, double[][]> _choiceInfo;

    public SmartProblemData(int nodeCount,
                            double initialPheromoneDensity,
                            IReadOnlyList<IReadOnlyList<double>> distances)
      : base(nodeCount, initialPheromoneDensity, distances)
    {
    }

    public override IReadOnlyList<IReadOnlyList<double>> ChoiceInfo(IAnt ant)
    {
      return _choiceInfo[ant.Id];
    }

    public override void UpdateGlobalPheromoneTrails(IEnumerable<IAnt> ants)
    {
      EvaporatePheromone();
      DepositPheromone(ants);
      DoGlobalUpdate();

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
          if (ant.Visited[i]) continue;

          // Matrix is symmetric, the same SmartPheromone
          // object is referenced by [i][j] and [j][i]
          var currentNode = ant.CurrentNode;
          var antId = ant.Id;
          _pheromone[currentNode][i].UpdatePresentedDensity(ant);

          _choiceInfo[antId][currentNode][i] = CalculateChoiceInfo(antId, currentNode, i);
          _choiceInfo[antId][i][currentNode] = _choiceInfo[antId][currentNode][i];
        }
      }
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

    protected override void UpdateChoiceInfoMatrix()
    {
      // Ant ID
      for (var antId = 0; antId < NodeCount; antId++)
      {
        // Matrix is symmetric
        for (var j = 0; j < NodeCount; j++)
        {
          for (var k = j; k < NodeCount; k++)
          {
            var choice = CalculateChoiceInfo(antId, j, k);
            _choiceInfo[antId][j][k] = choice;
            _choiceInfo[antId][k][j] = choice;
          }
        }
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
      _choiceInfo = new Dictionary<int, double[][]>();

      for (var i = 0; i < NodeCount; i++)
      {
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

    private void PopulatePheromoneStructures()
    {
      // Initialise rows.
      _pheromone = new ISmartPheromone[NodeCount][];

      for (var i = 0; i < NodeCount; i++)
      {
        // Initialise columns.
        _pheromone[i] = new ISmartPheromone[NodeCount];
      }

      for (var i = 0; i < NodeCount; i++)
      {
        for (var j = i; j < NodeCount; j++)
        {
          ISmartPheromone smart;

          // Ensure that we have smart pheromones on the diagonal
          // that always return max density.
          if (i == j)
          {
            smart = new SmartPheromoneConstant(i, j);
          }
          else
          {
            smart = new SmartPheromone(i, j, NodeCount, InitialPheromoneDensity);
            SmartPheromone.AllSmartPheromones.Add(smart);
          }

          // Matrix is symmetric, the same SmartPheromone
          // object is referenced by [i][j] and [j][i]
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

    private void DoGlobalUpdate()
    {
      for (var i = 0; i < NodeCount; i++)
      {
        for (var j = i; j < NodeCount; j++)
        {
          // Matrix is symmetric, the same SmartPheromone
          // object is referenced by [i][j] and [j][i]
          _pheromone[i][j].UpdatePheromoneGraphSnapshot();
        }
      }
    }

    private double CalculateChoiceInfo(int antId, int i, int j)
    {
      return Math.Pow(_pheromone[i][j].PresentedDensity(antId), Parameters.Alpha) * Heuristic[i][j];
    }
  }
}