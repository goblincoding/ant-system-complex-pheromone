using AntSimComplexAlgorithms.ProblemContext;
using AntSimComplexAlgorithms.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AntSimComplexAlgorithms
{
  /// <summary>
  /// This class is the entry point for the basic ("standard") Ant System implementation.
  /// </summary>
  public class AntSystem
  {
    /// <summary>
    /// A list of ALL the best tours per solution iteration.
    /// </summary>
    public List<BestTour> BestTours { get; } = new List<BestTour>();

    public List<IterationStatsItem> IterationStats => _statsAggregator.IterationStats;

    private Ant[] Ants { get; set; }

    private readonly StatsAggregator _statsAggregator;
    private readonly IProblemContext _problemContext;
    private int _currentIteration;

    /// <summary>
    /// Use a single, static random variable so that we do not end up with roughly
    /// the same number generation sequences with fast clock cycles.
    /// </summary>
    private static readonly Random Random = new Random(Guid.NewGuid().GetHashCode());

    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="nodeCount">The nr of nodes in the TSP graph.</param>
    /// <param name="nearestNeighbourTourLength">The tour length constructed through the Nearest Neighbour Heuristic.</param>
    /// <param name="distances">The distance matrix containing node to node edge weights.</param>
    public AntSystem(int nodeCount, int nearestNeighbourTourLength, IReadOnlyList<IReadOnlyList<int>> distances)
    {
      _problemContext = new Context(nodeCount, nearestNeighbourTourLength, distances, Random);
      _statsAggregator = new StatsAggregator();
      CreateAnts();
    }

    /// <summary>
    /// Resets the AntSystem internals.
    /// </summary>
    public void Reset()
    {
      BestTours.Clear();
      _problemContext.ResetPheromone();
      _statsAggregator.ClearStats();
      _currentIteration = 0;
    }

    /// <summary>
    /// 1. Initialise ants.
    /// 2. Construct solutions.
    /// 3. Update pheromone trails.
    /// </summary>
    public void Execute()
    {
      _statsAggregator.StartIteration(_currentIteration++);
      InitialiseAnts();
      ConstructSolutions();
      UpdatePheromoneTrails();
      _statsAggregator.StopIteration(Ants.Select(a => a.TourLength));

      var bestAnt = Ants.Min();
      BestTours.Add(new BestTour { TourLength = bestAnt.TourLength, Tour = bestAnt.Tour });
    }

    private void CreateAnts()
    {
      var nodeCount = _problemContext.NodeCount;
      Ants = new Ant[nodeCount];
      for (var i = 0; i < nodeCount; i++)
      {
        var ant = new Ant(_problemContext);
        Ants[i] = ant;
      }
    }

    private void InitialiseAnts()
    {
      // Initialise the ants at random start nodes.
      foreach (var ant in Ants)
      {
        var startNode = Random.Next(0, _problemContext.NodeCount);
        ant.Initialise(startNode);
      }
    }

    private void ConstructSolutions()
    {
      // Construct solutions (iterate through nr of nodes since
      // each ant has to visit each node once).
      for (var i = 0; i < _problemContext.NodeCount; i++)
      {
        foreach (var ant in Ants)
        {
          ant.MoveNext();
        }
      }
    }

    private void UpdatePheromoneTrails()
    {
      // Update pheromone trails.
      _problemContext.EvaporatePheromone();

      // Deposit new pheromone.
      foreach (var ant in Ants)
      {
        var deposit = 1 / ant.TourLength;
        _problemContext.DepositPheromone(ant.Tour, deposit);
      }

      // Choice info matrix has to be updated after pheromone changes.
      _problemContext.UpdateChoiceInfoMatrix();
    }
  }
}