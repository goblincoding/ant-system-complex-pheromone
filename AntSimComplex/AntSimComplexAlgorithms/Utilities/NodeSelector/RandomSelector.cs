using AntSimComplexAlgorithms.Ants;
using System;
using System.Linq;

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
    /// Randomly selects the index of the next unvisited node to visit.
    /// </summary>
    /// <param name="ant"></param>
    public int SelectNextNode(IAnt ant)
    {
      var notVisited = Enumerable.Range(0, ant.Visited.Count).Where(n => !ant.Visited[n]).ToArray();
      var index = _random.Next(0, notVisited.Length);
      return notVisited[index];
    }
  }
}