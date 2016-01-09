using AntSimComplexAlgorithms.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using TspLibNet;

namespace AntSimComplexAlgorithms
{
  /// <summary>
  /// This class is the entry point for the basic ("standard") Ant System implementation.
  /// </summary>
  public class AntSystem
  {
    /// <summary>
    /// Emitted when ants have to move to the next node.
    /// </summary>
    public event EventHandler MoveNext = delegate { };

    public Ant[] Ants { get; private set; }

    /// <summary>
    /// A list of ALL the best tours per solution iteration.
    /// </summary>
    public List<BestTour> BestTours { get; } = new List<BestTour>();

    private readonly int _nodeCount;
    private readonly ProblemContext _problemContext;

    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="problem">The TSP problem instance to which Ant System is to be applied.</param>
    /// <exception cref="ArgumentNullException">Thrown when "problem" is null.</exception>
    public AntSystem(IProblem problem)
    {
      if (problem == null)
      {
        throw new ArgumentNullException(nameof(problem), $"The {nameof(AntSystem)} constructor needs a valid problem instance argument");
      }

      _problemContext = new ProblemContext(problem);
      _nodeCount = _problemContext.NodeCount;

      Ants = new Ant[_nodeCount];
      for (var i = 0; i < _nodeCount; i++)
      {
        var ant = new Ant(_problemContext);
        MoveNext += ant.MoveNext;
        Ants[i] = ant;
      }
    }

    /// <summary>
    /// 1. Initialise ants.
    /// 2. Construct solutions.
    /// 3. Update pheromone trails.
    /// </summary>
    public void Execute()
    {
      // Initialise the ants at random start nodes.
      foreach (var ant in Ants)
      {
        var startNode = ProblemContext.Random.Next(0, _nodeCount);
        ant.Initialise(startNode);
      }

      // Construct solutions (iterate through nr of nodes since
      // each ant has to visit each node).
      for (var i = 0; i < _nodeCount; i++)
      {
        MoveNext(this, EventArgs.Empty);
      }

      // Update pheromone trails.
      EvaporatePheromone();
      DepositPheromone();

      // Choice info matrix has to be updated after pheromone changes.
      _problemContext.DataStructures.UpdateChoiceInfoMatrix();

      var best = Ants.Min();
      BestTours.Add(new BestTour { TourLength = best.TourLength, Tour = best.Tour });
    }

    /// <summary>
    /// Resets the AntSystem internals.
    /// </summary>
    public void Reset()
    {
      BestTours.Clear();
      _problemContext.ResetPheromone();
    }

    private void EvaporatePheromone()
    {
      for (var i = 0; i < _nodeCount; i++)
      {
        for (var j = i; j < _nodeCount; j++)
        {
          // Matrix is symmetric.
          var pher = _problemContext.DataStructures.Pheromone[i][j] * Parameters.EvaporationRate;
          _problemContext.DataStructures.Pheromone[i][j] = pher;
          _problemContext.DataStructures.Pheromone[i][j] = pher;
        }
      }
    }

    private void DepositPheromone()
    {
      foreach (var ant in Ants)
      {
        var deposit = 1 / ant.TourLength;
        for (var i = 0; i < _nodeCount; i++)
        {
          var j = ant.Tour[i];
          var l = ant.Tour[i + 1]; // stays within array bounds since Tour = n + 1 (returns to starting node)
          var pher = _problemContext.DataStructures.Pheromone[i][j] + deposit;
          _problemContext.DataStructures.Pheromone[j][l] = pher;  // matrix is symmetric
          _problemContext.DataStructures.Pheromone[l][j] = pher;
        }
      }
    }
  }
}