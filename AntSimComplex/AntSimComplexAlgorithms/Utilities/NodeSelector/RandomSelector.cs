using System;
using System.Collections.Generic;

namespace AntSimComplexAlgorithms.Utilities.NodeSelector
{
  internal class RandomSelector : INodeSelector
  {
    private readonly Random _random;

    /// <param name="random">The global random number generator object.</param>
    public RandomSelector(Random random)
    {
      _random = random;
    }

    public int SelectNextNode(IReadOnlyList<int> notVisited, int currentNode)
    {
      var index = _random.Next(0, notVisited.Count);
      return notVisited[index];
    }
  }
}