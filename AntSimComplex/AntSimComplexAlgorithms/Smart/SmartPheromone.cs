using AntSimComplexAlgorithms.Ants;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AntSimComplexAlgorithms.Smart
{
  internal class SmartPheromone
  {
    // Key = node, Value = density representation
    private readonly Dictionary<int, double> _densities;

    private readonly double _arcWeight;
    private readonly double _initialPheromoneDensity;

    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="node1">The index of one of the vertices of the pheromone's arc</param>
    /// <param name="node2">The index of one of the vertices of the pheromone's arc</param>
    /// <param name="arcWeight">The weight of the arc (distance between the nodes) that the pheromone is on</param>
    /// <param name="initialPheromoneDensity">Pheromone amount with which to initialise pheromone density</param>
    public SmartPheromone(int node1, int node2, double arcWeight, double initialPheromoneDensity)
    {
      _densities = new Dictionary<int, double>
      {
        { node1, initialPheromoneDensity },
        { node2, initialPheromoneDensity }
      };

      _arcWeight = arcWeight;
      _initialPheromoneDensity = initialPheromoneDensity;
    }

    /// <returns>The adapted density representation dependent on the direction of travel
    /// across the pheromone edge's vertices.</returns>
    /// <param name="fromNode">The node (vertex) for which the density is requested</param>
    /// <exception cref="KeyNotFoundException">Thrown if fromNode is invalid</exception>
    public double Density(int fromNode)
    {
      return _densities[fromNode];
    }

    /// <summary>
    /// Resets densities to initial pheromone density.
    /// </summary>
    public void Reset()
    {
      var ants = _densities.Keys.ToArray();
      foreach (var ant in ants)
      {
        _densities[ant] = _initialPheromoneDensity;
      }
    }

    /// <summary>
    /// Evaporates pheromone by the provided rate.
    /// </summary>
    public void Evaporate(double evaporationRate)
    {
      var ants = _densities.Keys.ToArray();
      foreach (var ant in ants)
      {
        _densities[ant] *= (1.0 - evaporationRate);
      }
    }

    /// <summary>
    /// Deposit pheromone.
    /// </summary>
    public void Deposit(double amount)
    {
      var ants = _densities.Keys.ToArray();
      foreach (var ant in ants)
      {
        _densities[ant] += amount;
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
      var adjustment = 1.0 / (ant.TourLength / _arcWeight);
      _densities[ant.CurrentNode] *= adjustment;
    }
  }
}