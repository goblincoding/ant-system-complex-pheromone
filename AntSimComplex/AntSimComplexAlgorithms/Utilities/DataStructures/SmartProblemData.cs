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
          if (ant.Visited[i]) continue;

          // Matrix is symmetric, the same SmartPheromone
          // object is referenced by [i][j] and [j][i]
          var currentNode = ant.CurrentNode;
          var stepCount = ant.StepCount;
          _pheromone[currentNode][i].Touch(ant);

          _choiceInfo[stepCount][currentNode][i] = CalculateChoiceInfo(stepCount, currentNode, i);
          _choiceInfo[stepCount][i][currentNode] = CalculateChoiceInfo(stepCount, i, currentNode);
        }
      }
    }

    public override IReadOnlyList<IReadOnlyList<double>> ChoiceInfo(IAnt ant)
    {
      return _choiceInfo[ant.StepCount];
    }

    protected override void UpdateChoiceInfoMatrix()
    {
      for (var i = 0; i < NodeCount; i++)
      {
        for (var j = 0; j < NodeCount; j++)
        {
          for (var k = 0; k < NodeCount; k++)
          {
            _choiceInfo[i][j][k] = CalculateChoiceInfo(i, j, k);
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
            smart = new SmartPheromoneConstant();
          }
          else
          {
            smart = new SmartPheromone(i, j, NodeCount, Distance(i, j), InitialPheromoneDensity);
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

    private double CalculateChoiceInfo(int stepCount, int i, int j)
    {
      return Math.Pow(_pheromone[i][j].Density(i, stepCount), Parameters.Alpha) * Heuristic[i][j];
    }
  }
}