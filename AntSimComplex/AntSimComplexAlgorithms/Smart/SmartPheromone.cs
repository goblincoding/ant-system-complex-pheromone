using AntSimComplexAlgorithms.Ants;
using System.Collections.Generic;
using System.Linq;

namespace AntSimComplexAlgorithms.Smart
{
  internal class SmartPheromone : ISmartPheromone
  {
    private readonly int _node1;
    private readonly int _node2;
    private readonly ISmartPheromone[][] _smartPheromones;
    private readonly double[] _densities;
    private readonly double _initialPheromoneDensity;
    private double _graphDensity;

    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="node1"></param>
    /// <param name="node2"></param>
    /// <param name="nodeCount"></param>
    /// <param name="initialPheromoneDensity">Pheromone amount with which to initialise pheromone density</param>
    /// <param name="smartPheromones"></param>
    public SmartPheromone(int node1, int node2,
      int nodeCount,
      double initialPheromoneDensity,
      ISmartPheromone[][] smartPheromones)
    {
      _node1 = node1;
      _node2 = node2;
      _smartPheromones = smartPheromones;
      _initialPheromoneDensity = initialPheromoneDensity;
      _graphDensity = _initialPheromoneDensity;

      _densities = new double[nodeCount];

      for (var i = 0; i < nodeCount; i++)
      {
        _densities[i] = _initialPheromoneDensity;
      }
    }

    /// <param name="antId"></param>
    public double PresentedDensity(int antId)
    {
      return _densities[antId];
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

    /// <summary>
    /// </summary>
    /// <param name="ant">The ant currently on one of the pheromone's vertices</param>
    public void Touch(IAnt ant)
    {
      var directionNode = ant.CurrentNode == _node1 ? _node1 : _node2;
      var pherLookAhead = new List<ISmartPheromone>();
      for (var i = 0; i < ant.Visited.Count; i++)
      {
        if (ant.Visited[i]) continue;
        pherLookAhead.Add(_smartPheromones[directionNode][i]);
      }

      var greatestNeighbourDensity = pherLookAhead.Select(p => p.GraphDensity()).Max();
      _densities[ant.Id] = _graphDensity + greatestNeighbourDensity;
    }
  }
}