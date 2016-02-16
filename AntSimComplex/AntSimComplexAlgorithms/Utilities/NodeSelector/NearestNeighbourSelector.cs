using AntSimComplexAlgorithms.Utilities.DataStructures;
using System.Collections.Generic;
using System.Linq;

namespace AntSimComplexAlgorithms.Utilities.NodeSelector
{
  internal class NearestNeighbourSelector : INodeSelector
  {
    private readonly IProblemData _problemData;

    public NearestNeighbourSelector(IProblemData problemData)
    {
      _problemData = problemData;
    }

    public int SelectNextNode(IReadOnlyList<int> notVisited, int currentNode)
    {
      var nearestNeighbours = _problemData.NearestNeighbours(currentNode);
      return nearestNeighbours.Where(notVisited.Contains).First();
    }
  }
}