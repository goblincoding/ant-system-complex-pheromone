using AntSimComplexAlgorithms.Ants;

namespace AntSimComplexAlgorithms.Smart
{
  internal class SmartPheromone : ISmartPheromone
  {
    // Provides a different density representation dependant
    // on the pheromone's position within the tour's construction.
    // In other words, it attempts to present a preferred position
    // within the graph based on the different densities.
    private readonly double[] _densities;

    private readonly double _initialPheromoneDensity;
    private readonly int _nodeCount;

    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="nodeCount"></param>
    /// <param name="initialPheromoneDensity">Pheromone amount with which to initialise pheromone density</param>
    public SmartPheromone(int nodeCount, double initialPheromoneDensity)
    {
      _nodeCount = nodeCount;
      _initialPheromoneDensity = initialPheromoneDensity;

      _densities = new double[nodeCount];

      for (var i = 0; i < nodeCount; i++)
      {
        _densities[i] = _initialPheromoneDensity;
      }
    }

    /// <param name="stepCount">Current global "step"</param>
    public double Density(int stepCount)
    {
      return _densities[stepCount];
    }

    /// <summary>
    /// Resets densities to initial pheromone density.
    /// </summary>
    public void Reset()
    {
      for (var i = 0; i < _nodeCount; i++)
      {
        _densities[i] = _initialPheromoneDensity;
      }
    }

    /// <summary>
    /// Evaporates pheromone by the provided rate.
    /// </summary>
    public void Evaporate(double evaporationRate)
    {
      for (var i = 0; i < _nodeCount; i++)
      {
        _densities[i] *= 1.0 - evaporationRate;
      }
    }

    /// <summary>
    /// Deposit pheromone.
    /// </summary>
    public void Deposit(double amount)
    {
      for (var i = 0; i < _nodeCount; i++)
      {
        _densities[i] += amount;
      }
    }

    /// <summary>
    /// Updates the density that will be presented to "ant" if it should consider
    /// stepping to one of the pheromone's vertices from the other.
    /// </summary>
    /// <param name="ant">The ant currently on one of the pheromone's vertices</param>
    public void Touch(IAnt ant)
    {
      // This is an arbitrary calculation that might have to be revisited.
      var adjustment = 1.0 / ant.TourLength;
      _densities[ant.StepCount] += adjustment;
    }
  }
}