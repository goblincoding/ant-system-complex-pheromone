using AntSimComplexAlgorithms.Ants;
using System.Collections.Generic;
using System.Linq;

namespace AntSimComplexAlgorithms.Smart
{
  internal class SmartPheromone : ISmartPheromone
  {
    public static List<ISmartPheromone> AllSmartPheromones = new List<ISmartPheromone>();

    public int Node1 { get; }
    public int Node2 { get; }

    private List<ISmartPheromone> _orderedNeighbours;
    private double _graphDensity;

    private readonly double[] _presentedDensities;
    private readonly double _initialPheromoneDensity;

    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="node1"></param>
    /// <param name="node2"></param>
    /// <param name="nodeCount"></param>
    /// <param name="initialPheromoneDensity">Pheromone amount with which to initialise pheromone density</param>
    public SmartPheromone(int node1, int node2, int nodeCount, double initialPheromoneDensity)
    {
      Node1 = node1;
      Node2 = node2;

      _initialPheromoneDensity = initialPheromoneDensity;
      _graphDensity = _initialPheromoneDensity;
      _presentedDensities = new double[nodeCount];

      for (var i = 0; i < nodeCount; i++)
      {
        _presentedDensities[i] = _initialPheromoneDensity;
      }

      _orderedNeighbours = new List<ISmartPheromone>();
    }

    public void UpdatePheromoneGraphSnapshot()
    {
      if (!_orderedNeighbours.Any())
      {
        foreach (var smartPheromone in AllSmartPheromones)
        {
          if (smartPheromone != this &&
              (smartPheromone.Node1 == Node1 ||
               smartPheromone.Node1 == Node2 ||
               smartPheromone.Node2 == Node1 ||
               smartPheromone.Node2 == Node2))
          {
            _orderedNeighbours.Add(smartPheromone);
          }
        }
      }

      _orderedNeighbours = _orderedNeighbours.OrderByDescending(s => s.GraphDensity()).ToList();
    }

    /// <summary>
    /// </summary>
    /// <param name="ant">The ant currently on one of the pheromone's vertices</param>
    public void UpdatePresentedDensity(IAnt ant)
    {
      foreach (var orderedNeighbour in _orderedNeighbours)
      {
        if (!ant.Visited[orderedNeighbour.Node1] ||
            !ant.Visited[orderedNeighbour.Node2])
        {
          _presentedDensities[ant.Id] = _graphDensity + orderedNeighbour.GraphDensity();
          break;
        }
      }
    }

    /// <param name="antId"></param>
    public double PresentedDensity(int antId)
    {
      return _presentedDensities[antId];
    }

    public double GraphDensity()
    {
      return _graphDensity;
    }

    /// <summary>
    /// Resets densities to initial pheromone density.
    /// </summary>
    public void Reset()
    {
      _graphDensity = _initialPheromoneDensity;
    }

    /// <summary>
    /// Evaporates pheromone by the provided rate.
    /// </summary>
    public void Evaporate(double evaporationRate)
    {
      _graphDensity *= 1.0 - evaporationRate;
    }

    /// <summary>
    /// Deposit pheromone.
    /// </summary>
    public void Deposit(double amount)
    {
      _graphDensity += amount;
    }
  }
}