using AntSimComplexAlgorithms.Ants;
using AntSimComplexAlgorithms.Utilities.DataStructures;
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
    /// <param name="ant"></param>
    public int SelectNextNode(IAnt ant)
    {
      var nearestNeighbours = _problemData.NearestNeighbours(ant.CurrentNode);
      return nearestNeighbours.First(neighbour => !ant.Visited[neighbour]);
    }
  }
}