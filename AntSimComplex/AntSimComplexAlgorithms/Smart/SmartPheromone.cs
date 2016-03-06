using AntSimComplexAlgorithms.Ants;
using System;
using System.Collections.Generic;

namespace AntSimComplexAlgorithms.Smart
{
  internal class SmartPheromone
  {
    // Key = antId, Value = density representation
    private readonly Dictionary<int, double> _densities;

    private readonly int _node1;
    private readonly int _node2;
    private readonly double _arcWeight;

    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="node1">The index of one of the vertices of the pheromone's arc</param>
    /// <param name="node2">The index of one of the vertices of the pheromone's arc</param>
    /// <param name="initialPheromoneDensity">Pheromone amount with which to initialise pheromone density</param>
    /// <param name="arcWeight">The weight of the arc (distance between the nodes) that the pheromone is on</param>
    /// <param name="ants">A list of ants active for the current problem</param>
    public SmartPheromone(int node1, int node2, double arcWeight, double initialPheromoneDensity, IEnumerable<IAnt> ants)
    {
      _densities = new Dictionary<int, double>();

      foreach (var ant in ants)
      {
        _densities.Add(ant.Id, initialPheromoneDensity);
      }

      _node1 = node1;
      _node2 = node2;
      _arcWeight = arcWeight;
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
      if (ant.CurrentNode != _node1 && ant.CurrentNode != _node2)
      {
        throw new ArgumentOutOfRangeException(nameof(ant), "The provided ant is not occupying a vertex on this pheromone arc");
      }

      // This is an arbitrary calculation that might have to be revisited.
      var adjustment = 1 / (ant.TourLength + _arcWeight);
      _densities[ant.Id] *= adjustment;
    }

    /// <returns>The adapted density representation for a specific ant considering
    /// stepping onto either of the vertices of the arc this pheromone represents</returns>
    /// <param name="antId">The id of the Ant for which the density is requested</param>
    /// <exception cref="KeyNotFoundException">Thrown if antId is invalid</exception>
    public double Density(int antId)
    {
      return _densities[antId];
    }
  }
}