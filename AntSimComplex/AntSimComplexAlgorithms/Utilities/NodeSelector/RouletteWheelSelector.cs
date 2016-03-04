using AntSimComplexAlgorithms.Utilities.DataStructures;
using System;
using System.Linq;

namespace AntSimComplexAlgorithms.Utilities.NodeSelector
{
  /// <summary>
  /// The probabilistic selection of the next node to be visited is done in accordance to the
  /// roulette wheel selection procedure: https://en.wikipedia.org/wiki/Fitness_proportionate_selection
  /// </summary>
  internal class RouletteWheelSelector : INodeSelector
  {
    // Comparing probability doubles is expensive, rather scale to a relatively big integer range.
    private const int ProbabilityScaleFactor = 1000000000;

    private readonly Random _random;
    private readonly IProblemData _problemData;

    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="problemData">The problem specific <seealso cref="StandardProblemData"/> object containing distance,
    /// pheromone, heuristic and choice info information.</param>
    /// <param name="random">The global random number generator object.</param>
    public RouletteWheelSelector(IProblemData problemData, Random random)
    {
      _problemData = problemData;
      _random = random;
    }

    /// <summary>
    /// Selects the index of the next node based on the "roulette wheel selection" principle.
    /// </summary>
    /// <param name="ant"></param>
    /// <returns>The index of the next node to visit.</returns>
    public int SelectNextNode(IAnt ant)
    {
      var selectedProbability = _random.Next(ProbabilityScaleFactor);
      var notVisited = ant.NotVisited;
      var probabilities = CalculateProbabilities(ant);

      var i = 0;
      var probabilitySum = probabilities[i];

      while (probabilitySum < selectedProbability &&
             i < probabilities.Length)
      {
        i++;
        probabilitySum += probabilities[i];
      }

      return notVisited[i];
    }

    /// <summary>
    /// Determines the probabilities of selection of the neighbour nodes based on the
    /// random proportional rule (ACO, Dorigo, 2004 p70).
    /// </summary>
    /// <returns>Probabilities of selection for each not visited node.</returns>
    private int[] CalculateProbabilities(IAnt ant)
    {
      // Denominator is the sum of the choice info values for the feasible neighbourhood.
      var choice = _problemData.ChoiceInfo(ant);
      var notVisited = ant.NotVisited;
      var denominator = notVisited.Sum(n => choice[ant.CurrentNode][n]);
      var probabilities = new int[notVisited.Count];

      // LINQ is not viable in this case due to the closure.  Performance
      // is increased significantly by using a basic for loop here instead.
      // ReSharper disable once LoopCanBeConvertedToQuery
      for (var i = 0; i < notVisited.Count; i++)
      {
        var numerator = choice[ant.CurrentNode][notVisited[i]];
        var probability = (int)(ProbabilityScaleFactor * (numerator / denominator));
        probabilities[i] = probability;
      }

      return probabilities;
    }
  }
}