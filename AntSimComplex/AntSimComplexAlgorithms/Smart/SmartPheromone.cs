using AntSimComplexAlgorithms.Ants;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AntSimComplexAlgorithms.Smart
{
  internal class SmartPheromone : ISmartPheromone
  {
    // Key = node, Value = density representation
    private readonly Dictionary<int, double[]> _densities;

    private readonly double _arcWeight;
    private readonly double _initialPheromoneDensity;
    private readonly int _node1;
    private readonly int _node2;
    private readonly int _nodeCount;

    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="node1">The index of one of the vertices of the pheromone's arc</param>
    /// <param name="node2">The index of one of the vertices of the pheromone's arc</param>
    /// <param name="nodeCount"></param>
    /// <param name="arcWeight">The weight of the arc (distance between the nodes) that the pheromone is on</param>
    /// <param name="initialPheromoneDensity">Pheromone amount with which to initialise pheromone density</param>
    /// <exception cref="ArgumentOutOfRangeException">Thrown if two node ID's are the same.</exception>
    public SmartPheromone(int node1, int node2, int nodeCount, double arcWeight, double initialPheromoneDensity)
    {
      if (node1 == node2)
      {
        throw new ArgumentOutOfRangeException($"{nameof(node1)} and {nameof(node2)} cannot be the same.");
      }

      _node1 = node1;
      _node2 = node2;
      _nodeCount = nodeCount;
      _arcWeight = arcWeight;
      _initialPheromoneDensity = initialPheromoneDensity;

      _densities = new Dictionary<int, double[]>
      {
        { _node1, new double[nodeCount] },
        { _node2, new double[nodeCount] }
      };

      for (var i = 0; i < nodeCount; i++)
      {
        _densities[_node1][i] = _initialPheromoneDensity;
        _densities[_node2][i] = _initialPheromoneDensity;
      }
    }

    /// <returns>The adapted density representation dependent on the direction of travel
    /// across the pheromone edge's vertices.</returns>
    /// <param name="fromNode">The node (vertex) for which the density is requested</param>
    /// <param name="stepCount">Current global "step"</param>
    /// <exception cref="KeyNotFoundException">Thrown if fromNode is invalid</exception>
    public double Density(int fromNode, int stepCount)
    {
      return _densities[fromNode].ElementAt(stepCount);
    }

    /// <summary>
    /// Resets densities to initial pheromone density.
    /// </summary>
    public void Reset()
    {
      for (var i = 0; i < _nodeCount; i++)
      {
        _densities[_node1][i] = _initialPheromoneDensity;
        _densities[_node2][i] = _initialPheromoneDensity;
      }
    }

    /// <summary>
    /// Evaporates pheromone by the provided rate.
    /// </summary>
    public void Evaporate(double evaporationRate)
    {
      for (var i = 0; i < _nodeCount; i++)
      {
        _densities[_node1][i] *= 1.0 - evaporationRate;
        _densities[_node2][i] *= 1.0 - evaporationRate;
      }
    }

    /// <summary>
    /// Deposit pheromone.
    /// </summary>
    public void Deposit(double amount)
    {
      for (var i = 0; i < _nodeCount; i++)
      {
        _densities[_node1][i] += amount;
        _densities[_node2][i] += amount;
      }
    }

    /// <summary>
    /// Updates the density that will be presented to "ant" if it should consider
    /// stepping to one of the pheromone's vertices from the other.
    /// </summary>
    /// <param name="ant">The ant currently on one of the pheromone's vertices</param>
    /// <exception cref="ArgumentOutOfRangeException">Thrown if the ant is not occupying one of the vertices
    /// for this pheromone's arc</exception>
    /// <exception cref="KeyNotFoundException">Thrown if ant is unknown</exception>
    public void Touch(IAnt ant)
    {
      if (!_densities.ContainsKey(ant.CurrentNode))
      {
        throw new ArgumentOutOfRangeException(nameof(ant), "The provided ant is not occupying a vertex on this pheromone arc");
      }

      // This is an arbitrary calculation that might have to be revisited.
      var adjustment = 1.0 / (ant.TourLength + _arcWeight);
      _densities[ant.CurrentNode][ant.StepCount] += adjustment;
    }
  }
}