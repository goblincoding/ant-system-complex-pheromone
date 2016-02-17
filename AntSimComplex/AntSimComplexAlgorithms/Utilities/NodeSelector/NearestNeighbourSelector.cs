using AntSimComplexAlgorithms.Utilities.DataStructures;
using System.Collections.Generic;
using System.Linq;

namespace AntSimComplexAlgorithms.Utilities.NodeSelector
{
  /// <summary>
  /// Selects the next nearest node to visit.
  /// </summary>
  internal class NearestNeighbourSelector : INodeSelector
  {
    private readonly IProblemData _problemData;

    public NearestNeighbourSelector(IProblemData problemData)
    {
      _problemData = problemData;
    }

    /// <summary>
    /// Selects the index of the nearest next node to visit.
    /// </summary>
    /// <param name="notVisited">The indices of the neighbouring nodes that have not been visited.</param>
    /// <param name="currentNode"> The index of the node whose neighbours are being assessed.</param>
    public int SelectNextNode(IReadOnlyList<int> notVisited, int currentNode)
    {
      var nearestNeighbours = _problemData.NearestNeighbours(currentNode);
      return nearestNeighbours.Where(notVisited.Contains).First();
    }
  }
}