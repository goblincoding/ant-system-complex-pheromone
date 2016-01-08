using System;
using TspLibNet;

namespace AntSimComplexAlgorithms.Utilities
{
  /// <summary>
  /// These parameters are for the random proportional rule (probability of selection, formula given on p70 of
  /// Ant Colony Optimisation - Dorigo and Stutzle) and are determined as per Box 3.1 "Good settings for ACO
  /// without local search applied." on p71.
  ///
  /// Initial pheromone is calculated as t0 = m / C^nn (nr of ants / nearest neighbour heuristic) as suggested on p70.
  /// </summary>
  public class Parameters
  {
    /// <summary>
    /// Determines the relative influence of the pheromone trail in the random proportional rule.
    /// </summary>
    public static int Alpha { get; set; } = 1;

    /// <summary>
    /// Good values are 2 - 5, determines the relative influence of the heuristic information
    /// in the random proportional rule.
    /// </summary>
    public static int Beta { get; set; } = 2;

    /// <summary>
    /// The pheromone evaporation rate for the pheromone update cycle (rho).
    /// </summary>
    public static double EvaporationRate { get; set; } = 0.5;

    /// <summary>
    /// The pheromone density initialisation value (tau zero or t0 = m / C^nn).
    /// </summary>
    public double InitialPheromone { get; }

    /// <param name="problem">A TSPLib.Net problem instance</param>
    /// <param name="random">Random number generator instance</param>
    /// <exception cref="ArgumentNullException">Thrown if an null problem instance was provided.</exception>
    public Parameters(IProblem problem, Random random)
    {
      if (problem == null)
      {
        throw new ArgumentNullException(nameof(problem), $"The {nameof(Parameters)} constructor needs a valid problem instance argument");
      }

      var numberOfAnts = problem.NodeProvider.CountNodes();
      InitialPheromone = numberOfAnts / problem.GetNearestNeighbourTourLength(random);
    }
  }
}