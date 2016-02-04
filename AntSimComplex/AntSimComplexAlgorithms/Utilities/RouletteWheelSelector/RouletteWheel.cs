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
    private class ProbabilityNodeIndexPair : IComparable<ProbabilityNodeIndexPair>, IComparable
    {
      public double Probability { get; set; }
      public int NeighbourIndex { get; set; }

      public int CompareTo(ProbabilityNodeIndexPair other)
      {
        if (other == null)
        {
          throw new ArgumentNullException(nameof(other));
        }

        return Probability.CompareTo(other.Probability);
      }

      public int CompareTo(object obj)
      {
        if (obj == null)
        {
          return 1;
        }

        var other = obj as ProbabilityNodeIndexPair;
        if (other == null)
        {
          throw new ArgumentException("Object is not of type 'ProbabilityNodeIndexPair'");
        }

        return Probability.CompareTo(other.Probability);
      }
    }

    private readonly Random _random;
    private readonly IDataStructures _dataStructures;

    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="dataStructures">The problem specific <seealso cref="Data"/> object containing distance,
    /// pheromone, heuristic and choice info information.</param>
    /// <param name="random">The global random number generator object.</param>
    public RouletteWheel(IDataStructures dataStructures, Random random)
    {
      _dataStructures = dataStructures;
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
      var selectedProbability = _random.NextDouble();
      var probabilities = CalculateProbabilities(notVisited, currentNode);

      // Find the first item with probability greater than the selected
      // probability, if no such item exists, return the last item (since
      // it has the greatest likelihood of selection).
      var selected = probabilities
                          .Where(p => p.Probability >= selectedProbability)
                          .DefaultIfEmpty(probabilities.Last())
                          .First();

      return selected.NeighbourIndex;
    }

    /// <summary>
    /// Determines the probabilities of selection of the "neighbour" nodes based on the
    /// "random proportional rule" (ACO, Dorigo, 2004 p70).
    /// </summary>
    /// <param name="notVisited">An array of node indices to neighbours that have not been visited.</param>
    /// <param name="currentNode"> The index of the node whose neighbours are being assessed.</param>
    /// <returns>An ordered list of ProbabilityNodeIndexPair sorted by probability.</returns>
    private IList<ProbabilityNodeIndexPair> CalculateProbabilities(IReadOnlyList<int> notVisited, int currentNode)
    {
      // Denominator is the sum of the choice info values for the feasible neighbourhood.
      var denominator = notVisited.Sum(n => _dataStructures.ChoiceInfo(currentNode, n));

      var pairs = from neighbour in notVisited
                  let numerator = _dataStructures.ChoiceInfo(currentNode, neighbour)
                  let probability = numerator / denominator
                  select new ProbabilityNodeIndexPair { Probability = probability, NeighbourIndex = neighbour };

      return pairs.OrderBy(pair => pair).ToList();
    }
  }
}