using System;
using System.Collections.Generic;
using System.Linq;

namespace AntSimComplexAlgorithms.Utilities
{
  /// <summary>
  /// The probabilistic selection of the next node to be visited is done in accordance to the
  /// roulette wheel selection procedure: https://en.wikipedia.org/wiki/Fitness_proportionate_selection
  /// (ACO p107)
  /// </summary>
  public class RouletteWheelSelector
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
      public int NodeIndex { get; set; }

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
    private readonly DataStructures _dataStructures;

    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="dataStructures">The problem specific <seealso cref="DataStructures"/> object containing distance,
    /// pheromone, choice info, etc information.</param>
    /// <param name="random">The global random number generator object.</param>
    public RouletteWheelSelector(DataStructures dataStructures, Random random)
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
    public int MakeSelection(int[] notVisited, int currentNode)
    {
      var selectedProbability = _random.NextDouble();
      var probabilities = Probabilities(notVisited, currentNode);
      var probabilityPair = probabilities.First();
      var sum = probabilityPair.Probability;
      for (var i = 1; i < probabilities.Count; i++)
      {
        if (sum >= selectedProbability)
        {
          break;
        }

        probabilityPair = probabilities[i];
        sum += probabilityPair.Probability;
      }

      return probabilityPair.NodeIndex;
    }

    /// <summary>
    /// Determines the probabilities of selection of the "neighbour" nodes based on the
    /// "random proportional rule" (ACO, Dorigo, 2004 p70).
    /// </summary>
    /// <param name="notVisited">An array of neighbouring node indices that have not been visited.</param>
    /// <param name="currentNode"> The index of the node whose neighbours are being assessed.</param>
    /// <returns>A list of KeyValuePairs sorted by key (probability) with value being the index
    /// of the node corresponding to the probability of selection represented by the key.</returns>
    private IList<ProbabilityNodeIndexPair> Probabilities(IReadOnlyList<int> notVisited, int currentNode)
    {
      // Select all the probability/index pairs (we need this so that we do not
      // lose the position of the neighbour index once we sort by probability).
      // In this case, "key" is probability and "value" is the corresponding node index.
      var denominator = notVisited.Sum(n => _dataStructures.ChoiceInfo(currentNode, n));

      var pairs = (from neighbour in notVisited
                   let numerator = _dataStructures.ChoiceInfo(currentNode, neighbour)
                   let probability = numerator / denominator
                   select new ProbabilityNodeIndexPair { Probability = probability, NodeIndex = neighbour });

      return pairs.OrderBy(pair => pair).ToList();
    }
  }
}