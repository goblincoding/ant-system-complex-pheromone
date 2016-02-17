using System;
using System.Collections.Generic;

namespace AntSimComplexAlgorithms.Utilities.NodeSelector
{
  /// <summary>
  /// Randomly selects the next node to visit.
  /// </summary>
  internal class RandomSelector : INodeSelector
  {
    private readonly Random _random;

    /// <param name="random">The global random number generator object.</param>
    public RandomSelector(Random random)
    {
      _random = random;
    }

    /// <summary>
    /// Randomly selects the index of the next node to visit.
    /// </summary>
    /// <param name="notVisited">The indices of the neighbouring nodes that have not been visited.</param>
    /// <param name="currentNode"> The index of the node whose neighbours are being assessed.</param>
    public int SelectNextNode(IReadOnlyList<int> notVisited, int currentNode)
    {
      var index = _random.Next(0, notVisited.Count);
      return notVisited[index];
    }
  }
}