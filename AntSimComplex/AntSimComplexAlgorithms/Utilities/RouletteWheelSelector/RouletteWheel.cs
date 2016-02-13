using AntSimComplexAlgorithms.Utilities.DataStructures;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AntSimComplexAlgorithms.Utilities.RouletteWheelSelector
{
  /// <summary>
  /// The probabilistic selection of the next node to be visited is done in accordance to the
  /// roulette wheel selection procedure: https://en.wikipedia.org/wiki/Fitness_proportionate_selection
  /// (ACO p107)
  /// </summary>
  internal class RouletteWheel : IRouletteWheelSelector
  {
    /// <summary>
    /// Helper class representing a node and a probability of selection of that node from
    /// a non-specified, other node.
    ///
    /// Compares on probability.
    /// </summary>
    private struct ProbabilityNodeIndexPair
    {
      public int Probability { get; set; }
      public int NeighbourIndex { get; set; }
    }

    // Comparing probability doubles are expensive, rather scale to a relatively big integer range.
    private const int ProbabilityScaleFactor = 1000000000;

    private readonly Random _random;
    private readonly IProblemData _problemData;

    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="problemData">The problem specific <seealso cref="ProblemData"/> object containing distance,
    /// pheromone, heuristic and choice info information.</param>
    /// <param name="random">The global random number generator object.</param>
    public RouletteWheel(IProblemData problemData, Random random)
    {
      _problemData = problemData;
      _random = random;
    }

    /// <summary>
    /// Randomly selects the index of the next node based on the "roulette wheel
    /// selection" principle.
    /// </summary>
    /// <param name="notVisited">The indices of the neighbouring nodes that have not been visited.</param>
    /// <param name="currentNode"> The index of the node whose neighbours are being assessed.</param>
    /// <returns>The index of the next node to visit.</returns>
    public int SelectNextNode(int[] notVisited, int currentNode)
    {
      var selectedProbability = _random.Next(ProbabilityScaleFactor);
      var probabilities = CalculateProbabilities(notVisited, currentNode);

      var i = 0;
      var probabilityPair = probabilities[i];
      var probabilitySum = probabilityPair.Probability;

      while (probabilitySum < selectedProbability)
      {
        i++;
        probabilityPair = probabilities[i];
        probabilitySum += probabilityPair.Probability;
      }

      return probabilityPair.NeighbourIndex;
    }

    /// <summary>
    /// Determines the probabilities of selection of the neighbour nodes based on the
    /// random proportional rule (ACO, Dorigo, 2004 p70).
    /// </summary>
    /// <param name="notVisited">An array of node indices to neighbours that have not been visited.</param>
    /// <param name="currentNode"> The index of the node whose neighbours are being assessed.</param>
    /// <returns>An unordered list of ProbabilityNodeIndexPair.</returns>
    private List<ProbabilityNodeIndexPair> CalculateProbabilities(IReadOnlyList<int> notVisited, int currentNode)
    {
      // Denominator is the sum of the choice info values for the feasible neighbourhood.
      var denominator = notVisited.Sum(n => _problemData.ChoiceInfo(currentNode, n));
      var pairs = new List<ProbabilityNodeIndexPair>();

      // LINQ is not viable in this case due to the closure.  Performance
      // is increased significantly by using a basic foreach here instead.
      // ReSharper disable once LoopCanBeConvertedToQuery
      foreach (var neighbour in notVisited)
      {
        var numerator = _problemData.ChoiceInfo(currentNode, neighbour);
        var probability = (int)(ProbabilityScaleFactor * (numerator / denominator));
        pairs.Add(new ProbabilityNodeIndexPair { Probability = probability, NeighbourIndex = neighbour });
      }

      return pairs;
    }
  }
}