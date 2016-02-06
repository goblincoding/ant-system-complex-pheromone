using AntSimComplexAlgorithms.ProblemContext;
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
    /// A list of ALL the best tours per solution iteration.
    /// </summary>
    public List<BestTour> BestTours { get; } = new List<BestTour>();

    public StatsAggregator StatsAggregator { get; }

    private Ant[] Ants { get; set; }
    private readonly IProblemContext _problemContext;
    private int _currentIteration;

    /// <summary>
    /// Use a single, static random variable so that we do not end up with roughly
    /// the same number generation sequences with fast clock cycles.
    /// </summary>
    private static readonly Random Random = new Random();

    /// <summary>
    /// Resets the AntSystem internals.
    /// </summary>
    public void Reset()
    {
      BestTours.Clear();
      _problemContext.ResetPheromone();
      StatsAggregator.ClearStats();
      _currentIteration = 0;
    }

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

      _problemContext = new Context(problem, Random);
      StatsAggregator = new StatsAggregator();
      CreateAnts();
    }

    /// <summary>
    /// 1. Initialise ants.
    /// 2. Construct solutions.
    /// 3. Update pheromone trails.
    /// </summary>
    public void Execute()
    {
      StatsAggregator.StartIteration(_currentIteration++);
      InitialiseAnts();
      ConstructSolutions();
      UpdatePheromoneTrails();
      StatsAggregator.StopIteration(Ants.Select(a => a.TourLength));

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