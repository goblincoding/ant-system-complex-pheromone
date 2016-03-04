using System;

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
    /// <param name="ant"></param>
    public int SelectNextNode(IAnt ant)
    {
      var index = _random.Next(0, ant.NotVisited.Count);
      return ant.NotVisited[index];
    }
  }
}